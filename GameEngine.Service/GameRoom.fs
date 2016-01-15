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


    /// A player actor connected to a human. Any human events needs to be send to the game via this actor. Also, humans receive messages from this actor.
    let PlayerActor receiver id gameRoom (mailbox:Actor<DynamicConnection>) =
        let logger = Logging.logDebug mailbox
        let loggErr = Logging.logErrorf mailbox

        logger "Start Gameroom PlayerActor"

        let handlePlayerMessage message =
            match message with
            | WhatIsMyId  -> receiver <! ToPlayer(YourId(id.ToString()))
            | Choice      -> receiver <! ToPlayer(Nothing)

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
    let CreatePlayerActor id receiver gameRoom = 
        spawn ActorSystem (CreateName GamePlayer id) (PlayerActor receiver id gameRoom)


    /// Game room actor, requires a GameId, three players
    let GameRoomActor (id:Guid) (players:Set<PlayerIdentity>) (mailbox:Actor<GameRoomMessage>) = 
        let logger = Logging.logDebug mailbox
        logger "Start GameRoomActor"

        let playerList = players |> Set.toList |> List.sortBy(fun p -> p.PlayerType)

        let rec gameLoop (gameState:Game) = actor {
            let! message = mailbox.Receive()
            return! gameLoop gameState
        }

        //  initialize game-room player actors
        playerList |> List.iter(fun plAct -> let selfSupporting = CreatePlayerActor plAct.Id plAct.Ref mailbox.Self in ())
        logger "initialized player actors"

        // Init game room
        let levelData = FServiceIO.LoadLevel
        logger "loaded level data"

        let gameData = Game.StartGame levelData id
        logger "started game"

        // these are players who enter the game, we add the handler when the game wants to send a message to the player
        let gamePlayerList = 
            playerList 
            |> List.map (fun playerIdentity ->
                            let sendMessageToPlayer (message:PlayerStatus) =
                                let messageToSend = 
                                    match message with
                                    | PlayerStatus.ItsMyTurn(x)    -> PlayerMessageResponse.ItIsYourTurn(x)
                                    | PlayerStatus.NoMoves         -> PlayerMessageResponse.NoMoves
                                    | PlayerStatus.GameOver        -> PlayerMessageResponse.GameOver
                                    | PlayerStatus.GameStarted(x)  -> PlayerMessageResponse.GameStarted(x, (gameData.RetrieveBoardData()))
                                    | PlayerStatus.BoardHasChanged -> PlayerMessageResponse.BoardHasChanged(gameData.GetBoardState())
                                    | _         -> failwith "Unrecognized message"

                                playerIdentity.Ref <! ToPlayer(messageToSend)
                            let player = GameEngine.FSharp.Player.Create playerIdentity.Id sendMessageToPlayer
                            player)

        let gameData = gamePlayerList |> List.fold (fun (gd:Game) player -> gd.JoinGame player) gameData
        logger "added all players to the game"

        let gameData = gameData.InformGameStartedToPlayers()
        logger "send game started to all players"

        gameData.SetTurn() |> ignore
        logger "set the first turn"

        // === Start game room ===
        gameLoop gameData


    /// Creates a game room actor
    let CreateGameRoomActor gameId players =
        spawn ActorSystem (CreateName GameRoom gameId) (GameRoomActor gameId players)
