namespace GameEngine.Service

open System
open Akka
open Akka.Logger.NLog
open Akkling

open GameEngine.Common
open GameEngine.FSharp

open Common
open InternalMessages
open GameActorSystem

module GameRoom =

    type GameRoomMessage =
        |   PlayerChoice of Guid * int



    /// A player actor connected to a human. Any human events needs to be send to the game via this actor. Also, humans receive messages from this actor.
    let PlayerActor receiver playerId gameRoom (mailbox:Actor<DynamicConnection>) =
        let logger = Logging.logDebug mailbox
        let loggErr = Logging.logErrorf mailbox

        logger "Start Gameroom PlayerActor"

        let handlePlayerMessage message =
            match message with
            | WhatIsMyId      -> receiver <! ToPlayer(YourId playerId)
            | Choice(tileId)  -> gameRoom <! PlayerChoice(playerId, tileId)

        let rec loop() = actor {
            try
                logger "In Gameroom.."

                let! message = mailbox.Receive()

                match message with
                | Data(pm)  -> 
                    logger "Handle message from player"
                    handlePlayerMessage pm
                    return! loop()
                | LooseConnection -> 
                    logger "Waiting room losing connection with player (finished waiting)"
                    ()
            with
            |   e -> loggErr "%A" e
        }
        receiver <! ChangeConnection(mailbox.Self)
        loop()


    /// Creates a Waitingroom Player Actor
    let CreatePlayerActor playerId receiver gameRoom = 
        spawn ActorSystem (CreateName GamePlayer playerId) (PlayerActor receiver playerId gameRoom)


    /// Send and message to the player
    let SendMessageToPlayer (gameData:Game) playerIdentity (message:PlayerStatus) =
        let messageToSend = 
            match message with
            | PlayerStatus.ItsMyTurn(x)       -> PlayerMessageResponse.ItIsYourTurn(x)
            | PlayerStatus.NoMoves            -> PlayerMessageResponse.NoMoves
            | PlayerStatus.GameOver           -> PlayerMessageResponse.GameOver
            | PlayerStatus.GameStarted(x)     -> PlayerMessageResponse.GameStarted(x, (gameData.RetrieveBoardData()))
            | PlayerStatus.BoardHasChanged(x) -> PlayerMessageResponse.BoardHasChanged(x)
            | _         -> failwith "Unrecognized message"
        playerIdentity.Ref <! ToPlayer(messageToSend)


    /// Game room actor, requires a GameId, three players
    let GameRoomActor (gameId:Guid) (players:Set<PlayerIdentity>) (mailbox:Actor<GameRoomMessage>) = 
        let logger = Logging.logDebug mailbox
        logger "Start GameRoomActor"

        let playerList = players |> Set.toList |> List.sortBy(fun p -> p.PlayerType)

        let rec gameLoop (gameState:Game) = actor {
            let! message = mailbox.Receive()
            match message with
            |   PlayerChoice(playerId, tileId) -> 
                let newGameState = gameState.ChooseTurn(playerId, tileId, false)
                return! gameLoop newGameState
            |   _          -> () // do nothing
            return! gameLoop gameState
        }

        //  initialize game-room player actors
        playerList |> List.iter(fun plAct -> let selfSupporting = CreatePlayerActor plAct.Id plAct.Ref mailbox.Self in ())
        logger "initialized player actors"

        // Init game room
        let levelData = FServiceIO.LoadLevel
        logger "loaded level data"

        let gameData = Game.LoadGame levelData
        logger "loaded game"

        // these are players who enter the game, we add the handler when the game wants to send a message to the player
        let gamePlayerList = 
            playerList 
            |> List.map (fun playerIdentity ->
                            let sendMessageToPlayerFunc = SendMessageToPlayer gameData playerIdentity
                            let player = GameEngine.FSharp.Player.Create playerIdentity.Id sendMessageToPlayerFunc
                            player)

        let gameData = gamePlayerList |> List.fold (fun (gd:Game) player -> gd.JoinGame player) gameData
        logger "added all players to the game"

        let gameData = gameData.InformGameStartedToPlayers()
        logger "send game started to all players"

        gameData.SetTurn() 
        logger "set the first turn"

        // === Start game room ===
        gameLoop gameData


    /// Creates a game room actor
    let CreateGameRoomActor gameId players =
        spawn ActorSystem (CreateName GameRoom gameId) (GameRoomActor gameId players)
