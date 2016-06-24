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

    type SendToServerMessage =
    |   SetServer of WebSocketServer<PlayerMessageResponseWebSocket, PlayerMessage>
    |   SendMessageToServer of PlayerMessage
    
    let SendToServer = MailboxProcessor<SendToServerMessage>.Start(fun inbox ->
        let rec loop server = async {
                let! msg = inbox.Receive()
                match msg with
                | SetServer(ws) -> Console.Log "Server Set"; return! loop(Some ws)
                | SendMessageToServer(pm) ->
                    match server with                    
                    |   None    ->  Console.Log "No server.. ignored"
                    |   Some(s) ->  s.Post pm; Console.Log "Send to server"
                return! (loop server)
            }
        loop None
        )

    [<Literal>]
    let xOffset = 230
    [<Literal>]
    let yOffset = 230

    let tileBoard = Var.Create List.empty<HexagonTileSerializable>
    let possibleMovesBoard = Var.Create List.empty<int>

    let GameMessage = Var.Create ""

    let ItsYourTurn = Var.Create false;

    let ColorToString color =
        match color with
        | TileType.blue   -> "Blue"
        | TileType.red    -> "Red"
        | TileType.yellow -> "Yellow"
        | _               -> "Unknown"


    let wsServer = 
        fun server ->
            0, fun state msg -> async {
                match msg with
                | Message data ->
                    Console.Log "WebSocket Message."
                    match data with
                    | PlayerMessageResponseWebSocket.YourId(id)                ->  
                        Var.Set playerId (id.ToString())
                        GameMessage.Value <- "Registered, waiting for game to start"
                    | PlayerMessageResponseWebSocket.GameStarted(color, board) ->
                        Var.Set playerColor (sprintf "%s" (ColorToString color))
                        Var.Set tileBoard (board.ActiveTileList |> List.ofArray)
                        GameMessage.Value <- "Game Started.. initializing board"
                    | PlayerMessageResponseWebSocket.BoardHasChanged(colorList) ->
                        let newTileList =
                            colorList |> List.fold(
                                fun st changeElement ->
                                    st |> List.map(
                                        fun tile ->
                                            if tile.Id = changeElement.Id then {tile with TileType = changeElement.TileType; Fortress = changeElement.Fortress}
                                            else tile
                                        )
                                ) tileBoard.Value
                        tileBoard.Value <- newTileList
                    | PlayerMessageResponseWebSocket.ItIsYourTurn(possibleMoves) -> 
                        Var.Set possibleMovesBoard possibleMoves
                        ItsYourTurn.Value <- true
                        GameMessage.Value <- "It is your turn"
                    | PlayerMessageResponseWebSocket.PlayerMadeChoice(color, tileId, withFortress) -> 
                        GameMessage.Value <- (sprintf "Player %s made a choice." (ColorToString color))
                    | PlayerMessageResponseWebSocket.NoMoves                   -> GameMessage.Value <- "No moves"
                    | PlayerMessageResponseWebSocket.GameOver                  -> GameMessage.Value <- "Game Over"
                    | PlayerMessageResponseWebSocket.Failed(message)           -> GameMessage.Value <- (sprintf "Failure: %s" message)
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

                            let color, clickable =
                                if possibleMoveMarked.IsNone then
                                    let svg = 
                                        match tile.TileType with
                                        | TileType.blue   -> "BlueHexagon.svg"
                                        | TileType.red    -> "RedHexagon.svg"
                                        | TileType.yellow -> "YellowHexagon.svg"
                                        | TileType.board  -> "Hexagon.svg"
                                        | _               -> ""
                                    (svg, false)
                                else
                                    ("HexagonPotentialChoice.svg", true)
                            let visual = Visual.Create (sprintf "App_Themes/Standard/%s" color) (xOffset + tile.X) (yOffset + tile.Y) 60 52
                           
                            let visual = 
                                if clickable then 
                                    visual 
                                    |> Visual.Click (
                                        fun el ev -> 
                                            possibleMovesBoard.Value <- List.empty<int>
                                            SendToServer.Post (SendMessageToServer(Choice(tile.Id)))
                                            )
                                else visual

                            visual :> Artefact
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
            SendToServer.Post (SetServer(server))
            server.Post (PlayerMessage.WhatIsMyId)
        } |> Async.Start

        let allVisuals = View.Map2(fun a b -> (* [Bevel:> Artefact] |> List.append *) b |> List.append a) BoardVisual FortressVisual

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
            Doc.TextView GameMessage.View
        ]
