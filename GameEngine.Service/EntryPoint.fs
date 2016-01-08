namespace GameEngine.Service

open System
open Akka
open Akka.Logger.NLog
open Akkling

open GameEngine.Common
open GameActorSystem

open Common
open InternalMessages

module EntryPoint = 

    /// This function handles the connection with the current player service.
    /// Instead of making one player actor with many behaviors, we make various player-actors with few behaviors.
    /// This game service actor handles the connection between the playerConnection actor and the current player's behavior in the game service. 
    /// The connection destination is changed over time, at change, the player behavior changes (to what's inside the new actor)
    let GameServiceConnectionFunc (receiver:IActorRef<PlayerMessageResponse>) (mailbox:Actor<GameServiceConnectionMessage>) =         
        let logger = mailbox.Log.Value.Debug

        let rec loop currentDestination = actor {
            let! message = mailbox.Receive()
            logger "Received message to service connection"
            
            match currentDestination with
            |   Some(destination) -> 
                    match message with
                    | ToGame(content) -> 
                        destination <! content
                        logger "Send message to current destination.."
                        return! (loop currentDestination)
                    | ToPlayer(content) -> 
                        receiver <! content
                        logger "Send message to player.."
                    | ChangeConnection(nextDestination) -> 
                        logger "Previous destination loses connection.."
                        destination <! LooseConnection
                        logger "Change connection destination.."
                        return! loop (Some nextDestination)
            |   None    ->
                    logger "No connection destination.."
                    match message with
                    | ToGame(content) -> 
                        mailbox.Stash()
                        logger "Message to game stashed.."
                        return! (loop currentDestination)
                    | ToPlayer(content) -> 
                        mailbox.Stash()
                        logger "Message to player stashed.."
                    | ChangeConnection(next) -> 
                        mailbox.UnstashAll()
                        logger "Change connection destination and unstash all.."
                        return! loop (Some next)
        }
        loop None


    let CreateGameServiceConnectionActor id receiver =
        spawn ActorSystem (CreateName GameServiceConnection id) (GameServiceConnectionFunc receiver)


    /// This function contains the behavior of the player connection.
    /// This actor is an intermediate between the player (external via Akka.Remote) and the GameEngine
    let PlayerConnectionFunc (gameServiceConnection:IActorRef<GameServiceConnectionMessage>) (mailbox:Actor<PlayerMessage>) message =
        let logger = Logging.logDebug mailbox
        logger "Received message from player"
        gameServiceConnection <! ToGame(Data(message))


    /// Create a PlayerConnection Actor
    /// A player connection only excepts public/remotely known messages to/from the player.
    /// If you send a message to this connection, you should always get a response.
    let CreatePlayerConnectionActor id (gameServiceConnection:IActorRef<GameServiceConnectionMessage>) =
        spawn ActorSystem (CreateName PlayerConnection id) (actorOf2 (PlayerConnectionFunc gameServiceConnection))


    type ConnectionChannel = {
        PlayerConnection  : IActorRef<PlayerMessage>;
        ServiceConnection : IActorRef<GameServiceConnectionMessage>
    }
    with
        static member Create p s = {PlayerConnection = p; ServiceConnection = s}


    let ConnectionSupervisorActorFunc (mailbox:Actor<IActorRef<PlayerMessageResponse>>) =
        let logger = Logging.logDebug mailbox

        let rec loop connectionList = actor {
            let! ``registration requester`` = mailbox.Receive()
            logger "Register new player"
            let id = Guid.NewGuid()
            let gameServiceConnection = CreateGameServiceConnectionActor id ``registration requester``
            let playerConnection = CreatePlayerConnectionActor id gameServiceConnection
            logger "Send 'you are registered'"
            ``registration requester`` <! YouAreRegisterd(Akkling.ActorRefs.untyped(playerConnection))
            logger "Player is registered"

            WaitingRoom <! AddPlayer(PlayerIdentity.Create id gameServiceConnection)
            logger "Player is send to a waiting room"

            return! (loop ((ConnectionChannel.Create playerConnection gameServiceConnection)::connectionList))
        }
        loop []


    /// The ConnectionSupervisor actor :- TODO requires supervision strategy
    let ConnectionSupervisor =
        spawn ActorSystem "ConnectionSuperVisor" ConnectionSupervisorActorFunc


    /// Creates the register player actor. This is the entry-point.
    /// This actor puts the player in a waiting room, to await the game.
    /// To the caller a PlayerProxy ActorRef is returned via which he can communicate with the game.
    let RegisterPlayer() = spawn ActorSystem "RegisterPlayer" (actorOf2 (fun mailbox message ->
            match message with
            | RegisterMe(playerReceiver) ->
                ConnectionSupervisor <! ActorRefs.typed<PlayerMessageResponse>(playerReceiver)
            ))

