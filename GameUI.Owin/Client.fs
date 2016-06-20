namespace GameUI.Owin

open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
open WebSharper.Owin
open WebSharper.Owin.WebSocket
open WebSharper.Owin.WebSocket.Client

open GameEngine.Common

[<JavaScript>]
module Client =
//    let endpoint = Endpoint<PlayerMessage, PlayerMessageResponseWebSocket>.Create("http://localhost", "/websocket")
    let rvInput = Var.Create ""

    let wsServer = 
        fun server ->
            0, fun state msg -> async {
                match msg with
                | Message data ->
                    Console.Log "WebSocket Message."
                    match data with
                    | PlayerMessageResponseWebSocket.YourId(id)                ->  Var.Set rvInput (id.ToString())
                    | PlayerMessageResponseWebSocket.GameStarted(color, board) -> ()
                    | PlayerMessageResponseWebSocket.BoardHasChanged(colorList) -> ()
                    | PlayerMessageResponseWebSocket.ItIsYourTurn(possibleMoves) -> ()
                    | PlayerMessageResponseWebSocket.PlayerMadeChoice(color, tileId, withFortress) -> ()
                    | PlayerMessageResponseWebSocket.NoMoves                   -> ()
                    | PlayerMessageResponseWebSocket.GameOver                  -> ()
                    | PlayerMessageResponseWebSocket.Failed(message)           -> ()
                    | PlayerMessageResponseWebSocket.Nothing                   -> ()
//                    | YouAreRegisterd(ref)      -> ()
                    return (state + 1)
                | Close ->
                    Console.Log "Connection closed."
                    return state
                | Open ->
                    Console.Log "WebSocket connection open."
                    return state
                | Error ->
                    Console.Log "WebSocket connection error!"
                    return state
            }


//    let WS (endpoint : Endpoint<PlayerMessageResponse, PlayerMessage>) = 
//        async {
//            let! server = ConnectStateful endpoint wsServer
//            ()
//        } |> Async.Start


    let Start input k =
        async {
            let! data = Server.DoSomething input
            return k data
        }
        |> Async.Start

    let Main (endpoint : Endpoint<PlayerMessageResponseWebSocket, PlayerMessage>) = 
        async {
            let! server = ConnectStateful endpoint wsServer
            //do! Async.Sleep 5000
            server.Post (PlayerMessage.WhatIsMyId)
            do! Async.Sleep 10000
        } |> Async.Start


//        let submit = Submitter.CreateOption rvInput.View
//        let vReversed =
//            submit.View.MapAsync(function
//                | None -> async { return "" }
//                | Some input -> Server.DoSomething input
//            )
        div [
            Doc.Input [] rvInput
//            Doc.Button "Send" [] submit.Trigger
            hr []
            h4Attr [attr.``class`` "text-muted"] [text "The server responded:"]
//            divAttr [attr.``class`` "jumbotron"] [h1 [textView vReversed]]
        ]
