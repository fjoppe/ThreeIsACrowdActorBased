namespace GameEngine.Service

open System
open Akka
open Akka.Logger.NLog
open Akkling

open GameEngine.Common
open GameEngine.FSharp

open Common
open GameRoom
open GameActorSystem
open InternalMessages

open Player

[<AutoOpen>]
module WaitingRoom =

    //  ========================== ACTOR STATE ==========================

    ///  Waiting room state, in which players may enter
    type WaitingRoomActorState = {
        GameId  : Guid
        Players : Set<PlayerIdentity>
    }
    with
        static member Create() = {GameId = (Guid.NewGuid()); Players = Set.empty<PlayerIdentity>}
        member this.AddPlayer    p = {this with Players = (this.Players.Add p)}
        member this.RemovePlayer p = {this with Players = (this.Players.Remove p)}


    /// AI Wainting state
    type AIWaitingState = {
        GameId  : Guid
    }


    /// A player actor connected to a human. Any human events needs to be send to the game via this actor. Also, humans receive messages from this actor.
    let PlayerActor receiver waitingRoom id (mailbox:Actor<DynamicConnection>) =
        let logger = Logging.logDebug mailbox
        let loggErr = Logging.logErrorf mailbox

        logger "Start Waitingroom PlayerActor"

        let handlePlayerMessage message =
            match message with
            | WhatIsMyId  -> receiver <! ToPlayer(YourId(id.ToString()))
            | Choice      -> receiver <! ToPlayer(Nothing)

        /// the player behavior while waiting in the waiting room for a game
        let rec waitForGameRoomResponse() = actor {
            let loop() = waitForGameRoomResponse()
            try
                logger "In Waiting room.."

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
        waitForGameRoomResponse()


    /// Creates a Waitingroom Player Actor
    let CreatePlayerActor receiver waitingRoom = 
        let id = Guid.NewGuid()
        spawn ActorSystem (CreateName Player id) (PlayerActor receiver waitingRoom id)


    /// Waiting room actor, which receives incoming player request for a game. Starts a game after three players entered the waiting room.
    let WaitingRoomActorFunc (mailbox:Actor<WaitingRoomMessage>) =
        let logger = Logging.logDebug mailbox

        logger "Start WaitingRoomActor"
        let rec waitingForRoomLoop (state:WaitingRoomActorState) = actor {
            let ignore()        = waitingForRoomLoop state
            let continueWith(p) = waitingForRoomLoop p

            if state.Players.Count = 3 then
                logger "Start Game"
                let gr = CreateGameRoomActor state.GameId state.Players
                return! continueWith(WaitingRoomActorState.Create())    // start over

            if state.Players.Count = 1 then
                logger "Spawn 2 AI players"
                [0..1] |> List.iter(fun e -> let ai = CreateAIPlayerActor mailbox.Self state.GameId in ())

            let! message = mailbox.Receive()
            logger (sprintf "WaitingRoomActor received message: %A" message)
            match message with
            | AddPlayer(candidate)    -> 
                if not(state.Players.Contains candidate) then 
                    logger (sprintf "Add Player: %A" candidate.Id)
                    let selfsupportive = CreatePlayerActor candidate.Ref mailbox.Self 
                    return! continueWith (state.AddPlayer candidate)
            | RemovePlayer(candidate) -> 
                if    (state.Players.Contains candidate) then 
                    logger (sprintf "Remove Player: %A" candidate.Id)
                    return! continueWith (state.RemovePlayer candidate)
            | AddAIPlayer(gameId, candidate) -> 
                if gameId <> state.GameId then 
                    logger (sprintf "Abandon AI Player: %A" candidate.Id)
                    candidate.Ref <! ToPlayer(Failed)
                if not(state.Players.Contains candidate) then 
                    logger (sprintf "Add AI Player: %A" candidate.Id)
                    return! continueWith (state.AddPlayer candidate)
            return! ignore()
        }
        waitingForRoomLoop (WaitingRoomActorState.Create())



    /// Creates the waiting room actor
    let WaitingRoom = spawn ActorSystem "WaitingRoom" WaitingRoomActorFunc

