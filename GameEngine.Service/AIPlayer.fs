namespace GameEngine.Service

open System
open Akka
open Akka.Logger.NLog
open Akkling

open Common
open GameActorSystem
open InternalMessages

open GameEngine.Common
open GameEngine.FSharp


module AIPlayer =

    type AIState = {
        GameState  : Game option
        Player     : Guid
        Connection : IActorRef<DynamicConnection> option
    }
    with
        member this.SetGameState state = {this with GameState = state }
        member this.SetConnection conn = {this with Connection= conn }
        member this.SetPlayer p = { this with Player = p }
        static member Create() = { GameState = None; Player = Guid.NewGuid(); Connection = None}

    let Wait() =
        let rnd = Random(31)
        int(4.0 + (rnd.NextDouble() * 3.0))

    /// Sends a message to the waiting room after a waiting period, and awaits the waiting room's response.
    /// If this response says that this actor is part of the game, the AI will play, otherwise it terminates.
    let AIPlayerActor waitingRoom playerId gameId (mailbox:Actor<GameServiceConnectionMessage>) =
        let logger = Logging.logDebug mailbox
        logger "Start AIPlayerActor"
        ActorSystem.Scheduler.ScheduleTellOnce(
            TimeSpan.FromSeconds(float(waitForAIToRegister)), 
            waitingRoom, 
            AddAIPlayer(gameId, PlayerIdentity.Create playerId mailbox.Self Computer)
        )

        let rec loop (state: AIState) = actor {
            let! message = mailbox.Receive()
            logger "AI loop received message"
            match message with
            | ToPlayer(playerMessage) -> 
                logger "Message was send to AI player.."
                match playerMessage with
                | PlayerMessageResponse.YourId(id) ->  logger (sprintf "Your Id: %A" id)
                | PlayerMessageResponse.YouAreRegisterd(ref) -> logger "Should not get this message: YouAreRegisterd"
                | PlayerMessageResponse.GameStarted(color, board) -> 
                    logger (sprintf "Game started and color: %A" color)
                    let gameData = Game.LoadGame board

                    //  just add 3 guid's / players to the game, later we will identify them via their colors.
                    let gameData = 
                        [0..2] 
                        |> List.fold(
                            fun (gd:Game) e -> 
                                let sendMessageToPlayerFunc m  = ()
                                let player = GameEngine.FSharp.Player.Create (Guid.NewGuid()) sendMessageToPlayerFunc
                                gd.JoinGame player 
                        ) gameData

                    let player = gameData.GetPlayerInfo(color)
                    let state = state.SetGameState (Some gameData)
                    let state = state.SetPlayer(player.Player.GetId())

                    mailbox.UnstashAll()
                    return! (loop state)
                | PlayerMessageResponse.GameOver ->  logger "Game Over"
                | PlayerMessageResponse.PlayerMadeChoice(c,i,f) ->
                    // via this match-case, the AI plays the game parallel to the original. Even its own moves are processed here.
                    logger "Player made choice"
                    match state.GameState with
                    | Some (gameState) ->
                        let player = gameState.GetPlayerInfo(c).Player
                        let newState = gameState.ChooseTurn(player, i, f)
                        return! (loop (state.SetGameState(Some newState)))
                    | None -> mailbox.Stash()
                | PlayerMessageResponse.ItIsYourTurn(possibleMoves) -> 
                    logger (sprintf "It is your turn, possible: %A" possibleMoves)
                    Async.Sleep (Wait() * 1000) |> Async.RunSynchronously
                    match state.GameState with
                    | Some (gameState) ->
                        let ai = AI.Create gameId
                        let choice = ai.DetermineChoice gameState (state.Player) possibleMoves
                        logger (sprintf "choice : %d" choice)
                        match state.Connection with
                        | Some(conn) -> conn <! Data(Choice(choice))
                        | None       -> logger "ERROR: this is very bad, made a choice but cannot send it"
                    | None -> mailbox.Stash()
                | PlayerMessageResponse.BoardHasChanged(newState) -> 
                    logger (sprintf "Board has changed...")                        
                    return! (loop state)
                | PlayerMessageResponse.NoMoves -> logger "No possible moves"
                | PlayerMessageResponse.Failed err ->  logger (sprintf "Failed: %s" err)
                | PlayerMessageResponse.Nothing ->  logger "Nothing"
            | ChangeConnection(nextDestination) -> 
                logger "Previous destination loses connection.."
                let currentConnection = state.Connection
                match currentConnection with
                | Some(destination) -> destination <! LooseConnection
                | None -> ()
                logger "Change connection destination.."
                return! loop (state.SetConnection (Some nextDestination))
            | ToGame(content) -> 
                logger "Impossible, cannot send something to game because I am the beginning.."
                return! (loop state)
            return! loop state
        }
        loop (AIState.Create())


    /// Create an AI player
    let CreateAIPlayerActor waitingRoom gameId = 
        let playerId = Guid.NewGuid()
        spawn ActorSystem (CreateName "AIPlayer" playerId) (AIPlayerActor waitingRoom playerId gameId)



