namespace DD.Caas.SyncAgent.Interfaces
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.ServiceFabric.Actors;

	/// <summary>
	///     Represents an instance of the CaaS API client actor.
	/// </summary>
	public interface IComputeApiClient
		: IActor
	{
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
		Task<Guid> Initialize(string targetGeo, string userName, string password);

		// TODO: Expose various CaaS API methods here.
	}
}