namespace TrafficSim

open System
open Akka
open Akka.FSharp
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
open WebSharper.Jatek
open GameEngine.Common

module VisualTest=

    let configuration = Configuration.ConfigurationFactory.ParseString(@"
        akka {
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            }
            remote {
                helios.tcp {
                    port = 8090
                    hostname = localhost
                }
            }
        }
        ")

    let ActorSystem = System.create "game-client" (configuration)

    let RegisterPlayer = ActorSystem.ActorSelection("akka.tcp://three-is-a-crowd@localhost:8080/user/RegisterPlayer")

    let PlayerActor = 
        async {
            return! RegisterPlayer <? RegisterMe
        } |> Async.RunSynchronously :> Actor.IActorRef


    module Server=

        [<Remote>]
        let RegisterUser() = 
            async {
                let! id = PlayerActor <? WhatIsMyId
                return unbox<string>(id)
            }

    [<JavaScript>]
    module Client =
        let playerGuid = Var.Create ""

        let GetPlayerId() = 
            async {
                let! result = Server.RegisterUser()
                playerGuid.Value <- result
                return ()
            }
            |> Async.Start

        let Main() =
            let block = Visual.Create "App_Themes/Standard/BlueHexagon.png" 50 50 60 52

            let scene = Scene.Create([block])

            let allScenes = [(1, scene)] |> Map.ofList

            let game = Game.Create 400 400 (fun p -> Console.Log "test") allScenes 1

            GetPlayerId()

            div [
                Doc.Input [] playerGuid
//                game.Visual
            ]
        
