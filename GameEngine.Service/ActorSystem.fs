namespace GameEngine.Service

open Akkling
open Akka.Remote

[<AutoOpen>]
module GameActorSystem =

    let configuration = Configuration.load()

    let ActorSystem = System.create "GameStateServer" (configuration)

