namespace GameEngine.Service

open System.IO
open System

open Akkling
open Akka.Remote

[<AutoOpen>]
module GameActorSystem =
    let configuration = Configuration.parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "akka.config")))

    let ActorSystem = System.create "GameStateServer" (configuration)

