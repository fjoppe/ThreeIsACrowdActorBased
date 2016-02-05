namespace GameUI.Suave.ThreeIsACrowd

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
    let endpoint = Endpoint<PlayerMessage, PlayerMessageResponse>.Create("http://localhost", "/websocket")
    let rvInput = Var.Create ""


    let wsServer = 
        fun server ->
            0, fun state msg -> async {
                match msg with
                | Message data ->
                    match data with
                    | YourId(id)                ->  Var.Set rvInput (id.ToString())
                    | GameStarted(color, board) -> ()
                    | BoardHasChanged(colorList) -> ()
                    | ItIsYourTurn(possibleMoves) -> ()
                    | PlayerMadeChoice(color, tileId, withFortress) -> ()
                    | NoMoves                   -> ()
                    | GameOver                  -> ()
                    | Failed(message)           -> ()
                    | Nothing                   -> ()
                    | YouAreRegisterd(ref)      -> ()
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


    let WS (endpoint : Endpoint<PlayerMessageResponse, PlayerMessage>) = 
        async {
            let! server = ConnectStateful endpoint wsServer
            ()
        } |> Async.Start


    let Main () =
        let submit = Submitter.CreateOption rvInput.View
        let vReversed =
            submit.View.MapAsync(function
                | None -> async { return "" }
                | Some input -> Server.DoSomething input
            )
        div [
            Doc.Input [] rvInput
            Doc.Button "Send" [] submit.Trigger
            hr []
            h4Attr [attr.``class`` "text-muted"] [text "The server responded:"]
            divAttr [attr.``class`` "jumbotron"] [h1 [textView vReversed]]
        ]
