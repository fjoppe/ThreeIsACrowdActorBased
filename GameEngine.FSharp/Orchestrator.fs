namespace GameEngine.FSharp

open System
open NLog;
open Akka
open Akkling

open GameEngine.Messages

[<AutoOpen>]
module GameActorSystem =
    let logger = LogManager.GetLogger("debug")

    [<Literal>]
    let waitForAIToRegister = 5

    //  ========================== MESSAGE TYPES ==========================

    type GameRoomMessage = class end


    /// The personalized game-info for the player
    type PlayerGameInfo = {
        GameRoom : IActorRef<GameRoomMessage>
        Color    : TileType 
    }
    with
        static member Create gr col = { GameRoom = gr; Color = col}


    /// A message to the player about the Game
    type PlayerGameMessage =
        | GameStarted of PlayerGameInfo
        | Failed


    /// Contains extended/convenient player identity info
    [<CustomEquality; CustomComparison>]
    type PlayerIdentity = {
            Id  : Guid                          // how he/she should be remembered
            Ref : IActorRef<PlayerGameMessage>  // how one should talk to him/her
        }
        with
            override x.Equals(yobj) =
                match yobj with
                | :? PlayerIdentity as y -> (x.Id = y.Id)
                | _ -> false
 
            override x.GetHashCode() = hash x.Id
            interface System.IComparable with
                member x.CompareTo yobj =
                    match yobj with
                    | :? PlayerIdentity as y -> compare x.Id y.Id
                    | _ -> invalidArg "yobj" "cannot compare values of different types"
            static member Create i r = {Id = i; Ref = r}


    /// A message from the (ai) player to a Waitingroom
    type WaitingRoomMessage =
        | AddPlayer    of PlayerIdentity
        | RemovePlayer of PlayerIdentity
        | AddAIPlayer  of Guid * PlayerIdentity


    //  ========================== UTIL FUNCTIONS ==========================

    
    [<Literal>]
    let Player = "Player"

    [<Literal>]
    let ProxyPlayer = "ProxyPlayer"

    [<Literal>]
    let GameRoom = "GameRoom"

    /// Create a readable Actor name
    let CreateName s g = sprintf "%s_%A" s g


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


    //  ========================== ACTORS ==========================

    let configuration = Configuration.parse(@"
        akka {
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            }

            remote {
                helios.tcp {
                    port = 8080
                    hostname = localhost
                }
            }
        }")


    let ActorSystem = System.create "GameStateServer" (configuration)

    /// Game room actor, requires a GameId, three players
    let GameRoomActor (id:Guid) (players:Set<PlayerIdentity>) (mailbox:Actor<GameRoomMessage>) = 
        logger.Debug "Start GameRoomActor"

        let playerList = players |> Set.toList

        let rec gameLoop (gameState:Game) = actor {
            let! message = mailbox.Receive()
            return! gameLoop gameState
        }

        // === Init game room ===              
        let levelData = FServiceIO.LoadLevel
        logger.Debug "loaded level data"

        let gameData = Game.StartGame levelData id
        logger.Debug "started game"

        let gameData = playerList  |> List.fold (fun (gd:Game) player -> gd.JoinGame (player.Id)) gameData
        logger.Debug "added all players to the game"

        let gameData = gameData.SetTurn()

        playerList |> List.iter(fun plAct -> 
            let myColor = gameData.GetPlayerColor(plAct.Id)
            plAct.Ref <! GameStarted(PlayerGameInfo.Create mailbox.Self myColor
            ))

        // === Start game room ===
        gameLoop gameData


    /// Creates a game room actor
    let CreateGameRoomActor gameId players =
        spawn ActorSystem (CreateName GameRoom gameId) (GameRoomActor gameId players)


    /// Sends a message to the waiting room after a waiting period, and awaits the waiting room's response.
    /// If this response says that this actor is part of the game, the AI will play, otherwise it terminates.
    let AIPlayerActor waitingRoom id gameId (mailbox:Actor<PlayerGameMessage>) =
        logger.Debug "Start AIPlayerActor"
        ActorSystem.Scheduler.ScheduleTellOnce(
            TimeSpan.FromSeconds(float(waitForAIToRegister)), 
            waitingRoom, 
            AddAIPlayer(gameId, PlayerIdentity.Create id mailbox.Self)
        )

        let rec aiPlayerLoop() = actor {
            let! message = mailbox.Receive()
            logger.Debug (sprintf "AI loop received message: %A" message)
            return! aiPlayerLoop()
        }
        aiPlayerLoop()


    /// Create an AI player
    let CreateAIPlayerActor waitingRoom gameId = 
        let id = Guid.NewGuid()
        spawn ActorSystem (CreateName "AIPlayer" id) (AIPlayerActor waitingRoom id gameId)
            

    /// Waiting room actor, which receives incoming player request for a game. Starts a game after three players entered the waiting room.
    let WaitingRoomActor(mailbox:Actor<WaitingRoomMessage>) =
        logger.Debug "Start WaitingRoomActor"
        let rec waitingForRoomLoop (state:WaitingRoomActorState) = actor {
            let ignore()        = waitingForRoomLoop state
            let continueWith(p) = waitingForRoomLoop p

            if state.Players.Count = 3 then
                logger.Debug "Start Game"
                let gr = CreateGameRoomActor state.GameId state.Players
                return! continueWith(WaitingRoomActorState.Create())    // start over

            if state.Players.Count = 1 then
                logger.Debug "Spawn 2 AI players"
                [0..1] |> List.iter(fun e -> let ai = CreateAIPlayerActor mailbox.Self state.GameId in ())

            let! message = mailbox.Receive()
            logger.Debug (sprintf "WaitingRoomActor received message: %A" message)
            match message with
            | AddPlayer(candidate)    -> if not(state.Players.Contains candidate) then 
                                            logger.Debug (sprintf "Add Player: %A" candidate.Id)
                                            return! continueWith (state.AddPlayer candidate)
            | RemovePlayer(candidate) -> if    (state.Players.Contains candidate) then 
                                            logger.Debug (sprintf "Remove Player: %A" candidate.Id)
                                            return! continueWith (state.RemovePlayer candidate)
                
            | AddAIPlayer(gameId, candidate) -> if gameId <> state.GameId then 
                                                    logger.Debug (sprintf "Abandon AI Player: %A" candidate.Id)
                                                    candidate.Ref <! Failed
                                                if not(state.Players.Contains candidate) then 
                                                    logger.Debug (sprintf "Add AI Player: %A" candidate.Id)
                                                    return! continueWith (state.AddPlayer candidate)
            return! ignore()
        }
        waitingForRoomLoop (WaitingRoomActorState.Create())


    /// A player actor connected to a human. Any human events needs to be send to the game via this actor. Also, humans receive messages from this actor.
    let PlayerActor waitingRoom id (mailbox:Actor<obj>) =
        logger.Debug "Start PlayerActor"

        let handlePlayerMessage message =
            match message with
            | WhatIsMyId  -> mailbox.Sender() <! YourId(id.ToString())
            | Choice      -> mailbox.Sender() <! Nothing

        /// the player behavior during the game
        let rec gamePlay (gameRoom:PlayerGameInfo) = actor {
            let loop() = gamePlay gameRoom
            try
                logger.Debug  "Player in GameRoom waiting for message"

                let! message = mailbox.Receive()

                match message with
                | :? PlayerMessage as  pm  -> handlePlayerMessage pm
                                              return! loop()
                //  here any in-game messages
                | _ ->  mailbox.Sender() <! ActorEffect.Unhandled
                        return! loop()
            with
            |   e -> logger.Error e
        }


        /// the player at initialization
        let requestGameRoom() = 
            logger.Debug  "Player Waiting for GameRoom"
            waitingRoom <! AddPlayer(PlayerIdentity.Create id (mailbox.Self.Retype<PlayerGameMessage>()))


        /// the player behavior while waiting in the waiting room for a game
        let rec waitForGameRoomResponse() = actor {
            let loop() = waitForGameRoomResponse()
            try
                logger.Debug  "Player Waiting for GameRoom Response"

                let! message = mailbox.Receive()

                match message with
                | :? PlayerMessage as  pm  -> 
                    logger.Debug  "Handle message from client"
                    handlePlayerMessage pm
                    return! loop()
                | :? PlayerGameMessage as pgm -> 
                    match pgm with
                    | GameStarted (gr) -> 
                        logger.Debug (sprintf "Received Game Started message, gameroom and player info: %A" gr)
                        return! gamePlay gr
                    | Failed           -> return! loop()
                | :? Akkling.Actors.LifecycleEvent as lce -> 
                    logger.Debug  "Received actor-lifecycle event... ignore"
                    return! loop() //  ignore..
                | _ ->  mailbox.Sender() <! ActorEffect.Unhandled
                        return! loop()
            with
            |   e -> logger.Error e
        }

        requestGameRoom()
        waitForGameRoomResponse()


    /// Creates a Player Actor
    let CreatePlayerActor waitingRoom = 
        let id = Guid.NewGuid()
        spawn ActorSystem (CreateName Player id) (PlayerActor waitingRoom id)


    /// Create a PlayerProxy Actor
    /// A playerproxy only excepts public/remotely known messages to/from the player.
    /// If you send a message to this proxy, you should always get a response.
    let CreatePlayerProxy (playerActor:IActorRef<obj>) =
        let id = Guid.NewGuid()
        spawn ActorSystem (CreateName ProxyPlayer id) (actorOf2 
            (fun (mailbox:Actor<PlayerMessage>) message -> 
                let response = 
                    async {
                        let! response = playerActor <? box message
                        return unbox<PlayerMessageResponse> response
                    } |> Async.RunSynchronously
                mailbox.Sender() <! response
            ))


    /// Creates the waiting room actor
    let WaitingRoom() = spawn ActorSystem "WaitingRoom" WaitingRoomActor

    /// Creates the register player actor. This is the entry-point.
    /// This actor puts the player in a waiting room, to await the game.
    /// To the caller a PlayerProxy ActorRef is returned via which he can communicate with the game.
    let RegisterPlayer waitingRoom = spawn ActorSystem "RegisterPlayer" (actorOf2 (fun mailbox message ->
            match message with
            | RegisterMe    ->
                let playerActor = CreatePlayerActor waitingRoom
                let playerActorProxy = CreatePlayerProxy playerActor
                let sender = mailbox.Sender()
                sender <! YouAreRegisterd(Akkling.ActorRefs.untyped(playerActorProxy))
            | YouAreRegisterd(a) ->
                mailbox.Sender() <! Unhandled
        ))
