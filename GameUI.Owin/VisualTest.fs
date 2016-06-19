namespace ThreeIsACrowd

open System
open Akka
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
open WebSharper.Jatek
open GameEngine.Common

open Akkling

module VisualTest=

    module Server=

//        [<Remote>]
//        let RegisterUser() = async {
//                GameEngineConnection.playerSender <! WhatIsMyId
//            }

    [<JavaScript>]
    module Client =
        let playerGuid = Var.Create ""

//        let GetPlayerId() = 
//            async {
//                do! Server.RegisterUser()
//            }
//            |> Async.Start

        let Main() =
            let block = Visual.Create "App_Themes/Standard/BlueHexagon.png" 50 50 60 52

            let scene = Scene.Create([block])

            let allScenes = [(1, scene)] |> Map.ofList

            let game = Game.Create 400 400 (fun p -> Console.Log "test") allScenes 1

//            GetPlayerId()

            let a = div [] :> Doc

            div [
                Doc.Input [] playerGuid
//                game.Visual
            ]
        
