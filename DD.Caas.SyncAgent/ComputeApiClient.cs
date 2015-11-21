using Microsoft.ServiceFabric.Actors;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DD.Caas.SyncAgent
{
	using ApiModels;
	using Cloud.WebApi.TemplateToolkit;
	using Interfaces;

	/// <remarks>
	///     The actor used as a client for the DD compute API.
	/// </remarks>
	class ComputeApiClient
		: StatefulActor<ComputeApiClient.ActorState>, IComputeApiClient
	{
		/// <summary>
		///     The request template for retrieving CaaS account details.
		/// </summary>
		static readonly HttpRequestBuilder<Unit> AccountRequest = StandardHttpRequest.Xml("myaccount", useXmlSerializer: true);

		/// <summary>
		///     The HTTP client message handler used to customise <see cref="HttpClient" /> behaviour.
		/// </summary>
		readonly HttpClientHandler  _clientHandler = new HttpClientHandler();

		/// <summary>
		///     The <see cref="HttpClient" /> used to make API requests.
		/// </summary>
		readonly HttpClient			_httpClient;

		/// <summary>
		///     Create a new Compute API client actor.
		/// </summary>
		public ComputeApiClient()
		{
			_httpClient = new HttpClient(_clientHandler);
		}

		/// <summary>
		///     Initialise the API client.
		/// </summary>
		/// <param name="targetGeo">
		///     The CaaS Geo (georgraphical region) whose API end-point will be used by the client.
		/// </param>
		/// <param name="userName">
		///     The user name to use when authenticating to the Compute API.
		/// </param>
		/// <param name="password">
		///     The password to use when authenticating to the Compute API.
		/// </param>
		/// <returns>
		///     The organisation Id associated with the target account.
		/// </returns>
		public async Task<Guid> Initialize(string targetGeo, string userName, string password)
		{
			if (String.IsNullOrWhiteSpace(targetGeo))
				throw new ArgumentException("Argument cannot be null, empty, or composed entirely of whitespace: 'targetGeo'.", nameof(targetGeo));

			if (State.IsInitialized)
				return State.ComputeOrganizationId;

			State.TargetGeo = targetGeo;
			State.ApiEndPoint = new Uri($"https://api-{targetGeo}.dimensiondata.com/oec/0.9");
			State.UserName = userName;
			State.Password = password;

			ConfigureHttpClient();

			Account accountDetail;
			try
			{
				accountDetail = await _httpClient.GetAsync<Account>(AccountRequest);
			}
			catch (Exception getClientDetailError)
			{
				// TODO: Define events with varying severities.
				ActorEventSource.Raise.ActorMessage(this, "Unexpected error during initialisation. " + getClientDetailError);

				throw;
			}

			State.ComputeOrganizationId = accountDetail.OrganizationId;
			State.IsInitialized = true;
			await SaveStateAsync();
			
			ActorEventSource.Raise.ActorMessage(this, "Initialisation complete (targeting organisation '{0}').", accountDetail.OrganizationId);

			return State.ComputeOrganizationId;
		}

		/// <summary>
		///     Called when an instance of the actor is activated.
		/// </summary>
		protected override Task OnActivateAsync()
		{
			if (State == null || !State.IsInitialized)
			{
				State = new ActorState
				{
					IsInitialized = false
				};
			}
			else
				ConfigureHttpClient();
			
			ActorEventSource.Raise.ActorMessage(this, "Compute API client configured: {0}", State);

			return Task.FromResult(true);
		}

		/// <summary>
		///     Called when the actor is deactivated.
		/// </summary>
		/// <returns>
		///     A <see cref="Task" /> that represents asynchronous operation.
		/// </returns>
		protected override async Task OnDeactivateAsync()
		{
			_httpClient.Dispose();
			_clientHandler.Dispose();

			await base.OnDeactivateAsync();
		}

		/// <summary>
		///		Configure the <see cref="HttpClient"/> for the current actor <see cref="ActorState"/>.
		/// </summary>
		void ConfigureHttpClient()
		{
			_clientHandler.Credentials = new NetworkCredential(State.UserName, State.Password);
			_httpClient.BaseAddress = State.ApiEndPoint;
		}

		/// <summary>
		///     The actor's replicated state.
		/// </summary>
		/// <remarks>
		///     Each instance of this class is serialized and replicated every time an actor's state is saved. For more information, see http://aka.ms/servicefabricactorsstateserialization.
		/// </remarks>
		[DataContract(Name = "ComputeApiClientState", Namespace = "http://schemas.tintoy.io/demos/service-fabric/caas-sync")]
		internal sealed class ActorState
		{
			/// <summary>
			///		Has the client been initialised?
			/// </summary>
			[DataMember(Name = "IsInitialized")]
			public bool IsInitialized
			{
				get;
				set;
			}

			/// <summary>
			///		The target Geo (DD Compute geographical region) targeted by the client.
			/// </summary>
			[DataMember(Name = "TargetGeo")]
			public string TargetGeo
			{
				get;
				set;
			}

			/// <summary>
			///		The API end-point being used by the client.
			/// </summary>
			[DataMember(Name = "ApiEndPoint")]
			public Uri ApiEndPoint
			{
				get;
				set;
			}

			/// <summary>
			///		The account user name used to authenticate to the DD Compute API.
			/// </summary>
			[DataMember(Name = "UserName")]
			public string UserName
			{
				get;
				set;
			}

			/// <summary>
			///		The account password used to authenticate to the DD Compute API.
			/// </summary>
			[DataMember(Name = "Password")]
			public string Password
			{
				get;
				set;
			}

			/// <summary>
			///		The account user name used to authenticate to the DD Compute API.
			/// </summary>
			[DataMember(Name = "ComputeOrganizationId")]
			public Guid ComputeOrganizationId
			{
				get;
				set;
			}

			/// <summary>
			///		Create a string representation of the <see cref="ActorState"/>.
			/// </summary>
			/// <returns>
			///		A string representing the <see cref="ActorState"/>.
			/// </returns>
			public override string ToString() => IsInitialized ? String.Format("ComputeApiClient.ActorState[OrgId = '{0}', ApiEndPoint = '{1}']", ComputeOrganizationId, ApiEndPoint) : "ComputeApiClient.ActorState[Uninitialized]";
		}
	}
}
