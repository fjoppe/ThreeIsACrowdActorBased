namespace GameEngine.Service

open System
open Akka
open Akka.Logger.NLog
open Akkling

open Common
open GameActorSystem
open InternalMessages


module Player =
    /// Sends a message to the waiting room after a waiting period, and awaits the waiting room's response.
    /// If this response says that this actor is part of the game, the AI will play, otherwise it terminates.
    let AIPlayerActor waitingRoom id gameId (mailbox:Actor<GameServiceConnectionMessage>) =
        let logger = Logging.logDebug mailbox
        logger "Start AIPlayerActor"
        ActorSystem.Scheduler.ScheduleTellOnce(
            TimeSpan.FromSeconds(float(waitForAIToRegister)), 
            waitingRoom, 
            AddAIPlayer(gameId, PlayerIdentity.Create id mailbox.Self)
        )

        let rec aiPlayerLoop() = actor {
            let! message = mailbox.Receive()
            logger (sprintf "AI loop received message: %A" message)
            return! aiPlayerLoop()
        }
        aiPlayerLoop()


    /// Create an AI player
    let CreateAIPlayerActor waitingRoom gameId = 
        let id = Guid.NewGuid()
        spawn ActorSystem (CreateName "AIPlayer" id) (AIPlayerActor waitingRoom id gameId)
            

