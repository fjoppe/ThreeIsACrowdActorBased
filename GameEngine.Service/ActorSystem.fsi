namespace GameEngine.Service

open Akka

[<AutoOpen>]
module GameActorSystem =
    val ActorSystem : Actor.ActorSystem
