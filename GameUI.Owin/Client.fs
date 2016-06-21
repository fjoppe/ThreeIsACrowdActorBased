namespace GameUI.Owin

open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Notation
open WebSharper.Owin
open WebSharper.Owin.WebSocket
open WebSharper.Owin.WebSocket.Client
open WebSharper.Jatek
open GameEngine.Common

[<JavaScript>]
module Client =
    let playerId = Var.Create ""
    let playerColor = Var.Create ""
    

    [<Literal>]
    let xOffset = 230
    [<Literal>]
    let yOffset = 230

    let tileBoard = Var.Create List.empty<HexagonTileSerializable>
    let possibleMovesBoard = Var.Create List.empty<int>

    let wsServer = 
        fun server ->
            0, fun state msg -> async {
                match msg with
                | Message data ->
                    Console.Log "WebSocket Message."
                    match data with
                    | PlayerMessageResponseWebSocket.YourId(id)                ->  Var.Set playerId (id.ToString())
                    | PlayerMessageResponseWebSocket.GameStarted(color, board) ->
                        let colorString =
                             match color with
                            | TileType.blue   -> "Blue"
                            | TileType.red    -> "Red"
                            | TileType.yellow -> "Yellow"
                            | _               -> "Unknown"
                        Var.Set playerColor (sprintf "%s" colorString)
                        Var.Set tileBoard (board.ActiveTileList |> List.ofArray)
                    | PlayerMessageResponseWebSocket.BoardHasChanged(colorList) -> ()
                    | PlayerMessageResponseWebSocket.ItIsYourTurn(possibleMoves) -> Var.Set possibleMovesBoard possibleMoves
                    | PlayerMessageResponseWebSocket.PlayerMadeChoice(color, tileId, withFortress) -> ()
                    | PlayerMessageResponseWebSocket.NoMoves                   -> ()
                    | PlayerMessageResponseWebSocket.GameOver                  -> ()
                    | PlayerMessageResponseWebSocket.Failed(message)           -> ()
                    | PlayerMessageResponseWebSocket.Nothing                   -> ()
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

    let Bevel = 
        let visual = Visual.Create "App_Themes/Standard/HexagonMouseOver.svg" 0 0 60 52
        visual.Visible <- false;
        visual 


    let BoardVisual =
        View.Const(
            fun tileList possibleMoves-> 
                let allVisuals = 
                    tileList 
                    |> List.map(
                        fun tile ->
                            let possibleMoveMarked = 
                                possibleMoves  
                                |> List.tryFind(fun e -> e = tile.Id)

                            let color =
                                if possibleMoveMarked.IsNone then
                                    match tile.TileType with
                                    | TileType.blue   -> "BlueHexagon.svg"
                                    | TileType.red    -> "RedHexagon.svg"
                                    | TileType.yellow -> "YellowHexagon.svg"
                                    | TileType.board  -> "Hexagon.svg"
                                    | _               -> ""
                                else
                                    "HexagonPotentialChoice.svg"
                            let visual = Visual.Create (sprintf "App_Themes/Standard/%s" color) (xOffset + tile.X) (yOffset + tile.Y) 60 52 
                            visual
                            |>  Visual.MouseEnter (fun el ev -> Bevel.Visible <- true)
                            |>  Visual.MouseLeave (fun el ev -> Bevel.Visible <- false)
                            |>  Visual.MouseOver  (fun el ev -> Bevel.Left <- visual.Left; Bevel.Top <- visual.Top)
                            :> Artefact
                    )
                allVisuals
        )
        <*> tileBoard.View
        <*> possibleMovesBoard.View

    let FortressVisual = 
        View.Const(
            fun tileList possibleMoves-> 
                let allVisuals = 
                    tileList 
                    |> List.filter(fun tile -> tile.Fortress)
                    |> List.map(fun tile -> Visual.Create "App_Themes/Standard/Fortress.svg" (xOffset + tile.X) (yOffset + tile.Y) 60 52 :> Artefact)
                allVisuals
        )
        <*> tileBoard.View
        <*> possibleMovesBoard.View


    let Main (endpoint : Endpoint<PlayerMessageResponseWebSocket, PlayerMessage>) = 
        async {
            let! server = ConnectStateful endpoint wsServer
            server.Post (PlayerMessage.WhatIsMyId)
            do! Async.Sleep 10000
        } |> Async.Start

        let allVisuals = View.Map2(fun a b -> [Bevel:> Artefact] |> List.append b |> List.append a) BoardVisual FortressVisual

        let scene = allVisuals |> View.Map(fun av -> Scene.Create(av)) 
        let allScenes = scene |> View.Map(fun s -> [(1, s)] |> Map.ofList)
        let game = allScenes |> View.Map(fun asc -> (Game.Create 500 500 (fun p -> Console.Log "test") asc 1).Visual)

        div [
            p [
                label [text "PlayerId:"]
                Doc.TextView playerId.View
                Doc.TextNode " / "
                Doc.TextView playerColor.View
            ]
            hr []
            Doc.EmbedView game
        ]
