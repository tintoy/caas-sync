using JetBrains.Annotations;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Threading.Tasks;

namespace DD.Caas.SyncAgent
{
	/// <summary>
	///		Event source for the Compute synchronisation agent.
	/// </summary>
	[EventSource(Name = "DDCloud-Compute-SyncAgent")]
	sealed class ActorEventSource
		: EventSource
	{
		/// <summary>
		///		A singleton instance of the event source that can be used to raise events.
		/// </summary>
		public static readonly ActorEventSource Raise = new ActorEventSource();

		#region Initialisation

		/// <summary>
		///		Type initialiser for <see cref="ActorEventSource"/>.
		/// </summary>
		static ActorEventSource()
		{
			// A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
			// This problem will be fixed in .NET Framework 4.6.2.
			Task.Run(() => { }).Wait();
		}

		/// <summary>
		///		Create a new <see cref="ActorEventSource"/>.
		/// </summary>
		ActorEventSource()
		{
		}

		#endregion // Initialisation

		#region Events

		/// <summary>
		///		Raise an event representing a logged message.
		/// </summary>
		/// <param name="messageOrFormat">
		///		The message or message-format specifier.
		/// </param>
		/// <param name="formatArguments">
		///		Optional message-format arguments.
		/// </param>
		[NonEvent]
		[StringFormatMethod("messageOrFormat")]
		public void Message(string messageOrFormat, params object[] formatArguments)
		{
			if (IsEnabled())
			{
				string finalMessage = string.Format(messageOrFormat, formatArguments);
				Message(finalMessage);
			}
		}

		/// <summary>
		///		Raise an event representing a logged message.
		/// </summary>
		/// <param name="message">
		///		The message.
		/// </param>
		[Event(EventIds.Message, Level = EventLevel.Informational, Message = "{0}")]
		public void Message(string message)
		{
			if (IsEnabled())
				WriteEvent(EventIds.Message, message);
		}

		/// <summary>
		///		Raise an event representing a message logged by an actor.
		/// </summary>
		/// <param name="actor">
		///		The actor.
		/// </param>
		/// <param name="messageOrFormat">
		///		The message or message-format specifier.
		/// </param>
		/// <param name="formatArguments">
		///		Optional message-format arguments.
		/// </param>
		[NonEvent]
		[StringFormatMethod("messageOrFormat")]
		public void ActorMessage(StatelessActor actor, string messageOrFormat, params object[] formatArguments)
		{
			if (IsEnabled())
			{
				string finalMessage = String.Format(messageOrFormat, formatArguments);
				ActorMessage(
					actor.GetType().ToString(),
					actor.Id.ToString(),
					actor.ActorService.ServiceInitializationParameters.CodePackageActivationContext.ApplicationTypeName,
					actor.ActorService.ServiceInitializationParameters.CodePackageActivationContext.ApplicationName,
					actor.ActorService.ServiceInitializationParameters.ServiceTypeName,
					actor.ActorService.ServiceInitializationParameters.ServiceName.ToString(),
					actor.ActorService.ServiceInitializationParameters.PartitionId,
					actor.ActorService.ServiceInitializationParameters.InstanceId,
					FabricRuntime.GetNodeContext().NodeName,
					finalMessage
				);
			}
		}

		/// <summary>
		///		Raise an event representing a message logged by an actor.
		/// </summary>
		/// <param name="actor">
		///		The actor.
		/// </param>
		/// <param name="messageOrFormat">
		///		The message or message-format specifier.
		/// </param>
		/// <param name="formatArguments">
		///		Optional message-format arguments.
		/// </param>
		[NonEvent]
		[StringFormatMethod("messageOrFormat")]
		public void ActorMessage(StatefulActorBase actor, string messageOrFormat, params object[] formatArguments)
		{
			if (IsEnabled())
			{
				string finalMessage = String.Format(messageOrFormat, formatArguments);

				ActorMessage(
					actor.GetType().ToString(),
					actor.Id.ToString(),
					actor.ActorService.ServiceInitializationParameters.CodePackageActivationContext.ApplicationTypeName,
					actor.ActorService.ServiceInitializationParameters.CodePackageActivationContext.ApplicationName,
					actor.ActorService.ServiceInitializationParameters.ServiceTypeName,
					actor.ActorService.ServiceInitializationParameters.ServiceName.ToString(),
					actor.ActorService.ServiceInitializationParameters.PartitionId,
					actor.ActorService.ServiceInitializationParameters.ReplicaId,
					FabricRuntime.GetNodeContext().NodeName,
					finalMessage
				);
			}
		}

		/// <summary>
		///		Raise an event representing a message logged by an actor.
		/// </summary>
		/// <param name="actorType">
		///		The actor type name.
		/// </param>
		/// <param name="actorId">
		///		The actor Id.
		/// </param>
		/// <param name="applicationTypeName">
		///		The name of the actor's containing application type.
		/// </param>
		/// <param name="applicationName">
		///		The name of the actor's containing application.
		/// </param>
		/// <param name="serviceTypeName">
		///		The name of the actor's host service type.
		/// </param>
		/// <param name="serviceName">
		///		The name of the actor's hosting service.
		/// </param>
		/// <param name="partitionId">
		///		The Id of the partition where the actor is hosted.
		/// </param>
		/// <param name="replicaOrInstanceId">
		///		The instance or replica Id of the actor (stateful actors are usually replicated and run as multiple instances).
		/// </param>
		/// <param name="nodeName">
		///		The name of the cluster node on which the actor is running.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		[Event(EventIds.ActorMessage, Level = EventLevel.Informational, Message = "{9}")]
		void ActorMessage(
			string actorType,
			string actorId,
			string applicationTypeName,
			string applicationName,
			string serviceTypeName,
			string serviceName,
			Guid partitionId,
			long replicaOrInstanceId,
			string nodeName,
			string message
		)
		{
			WriteEvent(
				EventIds.ActorMessage,
				actorType,
				actorId,
				applicationTypeName,
				applicationName,
				serviceTypeName,
				serviceName,
				partitionId,
				replicaOrInstanceId,
				nodeName,
				message
			);
		}

		/// <summary>
		///		Raise an event representing an error encountered while initialising an actor host process.
		/// </summary>
		/// <param name="exceptionDetail">
		///		Detailed information about the exception.
		/// </param>
		[Event(EventIds.ActorHostInitializationFailed, Level = EventLevel.Error, Message = "Actor host initialization failed", Keywords = Keywords.HostInitialization)]
		public void ActorHostInitializationFailed(string exceptionDetail)
		{
			WriteEvent(EventIds.ActorHostInitializationFailed, exceptionDetail);
		}

		#endregion

		#region Event Ids

		/// <summary>
		///		Well-known event Ids used by the event source.
		/// </summary>
		public static class EventIds
		{
			/// <summary>
			///		A log message.
			/// </summary>
			public const int Message                        = 1;

			/// <summary>
			///		A log message from an actor instance.
			/// </summary>
			public const int ActorMessage                   = 2;

			/// <summary>
			///		An error occurred while initialising the actor host process.
			/// </summary>
			public const int ActorHostInitializationFailed  = 3;
		}

		#endregion // Event Ids

		#region Keywords

		/// <summary>
		///		Well-known event keywords.
		/// </summary>
		public static class Keywords
		{
			/// <summary>
			///		Keyword for events relating to host initialisation.
			/// </summary>
			public const EventKeywords HostInitialization = (EventKeywords)0x1L;
		}

		#endregion // Keywords
	}
}