# caas-sync
Demo using Azure Service Fabric stateful actors to build a synchronisation mechanism that spans multiple DD Compute Geos (geographical regions) to provide a unified view of compute resources.

This is a work-in-progress; unlike (say) Akka actors, SF actors are virtualised and so things will probably be modelled differently than they are in the equivalent Akka / Scala demo. The 2 demos are also focusing on different things; the Akka one focuses on the use of streams and transformations to express connectivity to the DD Compute API, whereas this one is focused on modeling individual Compute entities as separate actors (and locating the point of diminishing returns when it comes to actor granularity).
