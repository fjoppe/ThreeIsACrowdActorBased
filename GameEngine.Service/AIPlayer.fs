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
        Connection : IActorRef<DynamicConnection> option
    }
    with
        member this.SetGameState state = {this with GameState = state }
        member this.SetConnection conn = {this with Connection= conn }
        static member Create() = { GameState = None; Connection = None}


    /// Sends a message to the waiting room after a waiting period, and awaits the waiting room's response.
    /// If this response says that this actor is part of the game, the AI will play, otherwise it terminates.
    let AIPlayerActor waitingRoom id gameId (mailbox:Actor<GameServiceConnectionMessage>) =
        let logger = Logging.logDebug mailbox
        logger "Start AIPlayerActor"
        ActorSystem.Scheduler.ScheduleTellOnce(
            TimeSpan.FromSeconds(float(waitForAIToRegister)), 
            waitingRoom, 
            AddAIPlayer(gameId, PlayerIdentity.Create id mailbox.Self Computer)
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
                | PlayerMessageResponse.GameStarted(color, board) -> logger (sprintf "Game started and color: %A" color)
                | PlayerMessageResponse.GameOver ->  logger "Game Over"
                | PlayerMessageResponse.ItIsYourTurn(possibleMoves) -> logger (sprintf "It is your turn, possible: %A" possibleMoves)
                | PlayerMessageResponse.BoardHasChanged(state) -> logger (sprintf "Board has changed...")
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
        let id = Guid.NewGuid()
        spawn ActorSystem (CreateName "AIPlayer" id) (AIPlayerActor waitingRoom id gameId)
            

