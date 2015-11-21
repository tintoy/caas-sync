using Microsoft.ServiceFabric.Actors;
using System;
using System.Fabric;
using System.Threading;

namespace DD.Caas.SyncAgent
{
	/// <summary>
	///		Host process for the DD Compute synchronisation agent.
	/// </summary>
	static class Program
	{
		/// <summary>
		///     The entry point for the service host process.
		/// </summary>
		static void Main()
		{
			try
			{
				// Creating a FabricRuntime connects this host process to the Service Fabric runtime on this node.
				using (FabricRuntime fabricRuntime = FabricRuntime.Create())
				{
					// This line registers your actor class with the Fabric Runtime.
					// The contents of your ServiceManifest.xml and ApplicationManifest.xml files
					// are automatically populated when you build this project.
					// For more information, see http://aka.ms/servicefabricactorsplatform
					fabricRuntime.RegisterActor<ComputeApiClient>();

					Thread.Sleep(Timeout.Infinite); // Prevents this host process from terminating to keep the service host process running.
				}
			}
			catch (Exception unexpectedError)
			{
				ActorEventSource.Raise.ActorHostInitializationFailed(unexpectedError.ToString());

				throw;
			}
		}
	}
}