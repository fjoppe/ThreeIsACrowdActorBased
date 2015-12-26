#r @"..\packages\Akka.1.0.5\lib\net45\Akka.dll"
#r @"..\packages\Akkling.0.2.1\lib\net45\Akkling.dll"
#I @"..\packages\FsPickler.1.4.0\lib\net45"
#I @"..\packages\Newtonsoft.Json.7.0.1\lib\net45"

open System
open Akka
open Akkling

[<CustomEquality; CustomComparison>]
type PlayerIdentity = {
        Id  : Guid
        Ref : Actor.IActorRef 
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

let ActorSystem = System.create "three-is-a-crowd" (Configuration.defaultConfig())


let GameRoomActor (players:Set<PlayerIdentity>) = 
    actorOf (fun msg ->
        printfn "%A" msg
    )

type WaitingRoomMessage =
    | AddPlayer of PlayerIdentity
    | RemovePlayer of PlayerIdentity


type PlayerGameInfo = {
    GameRoom : Actor.IActorRef

}

type PlayerMessage = 
    | GameStarted of PlayerGameInfo
    | Failed
            

let WaitingRoomActor(mailbox:Actor<WaitingRoomMessage>) =
    let log = printfn
    let rec waitingForRoomToFill (players:Set<PlayerIdentity>) = actor {
        let ignore() = 
            log "Ignored.."
            waitingForRoomToFill players
        let continueWith(p) = 
            log "State mutated.."
            waitingForRoomToFill p

        if Set.count players = 3 then
            //  start the game by notifying all players the gameRoom has been initialized
            let gameRoom = spawn ActorSystem "GameRoom" (GameRoomActor players)
            players |> Set.toList |> List.iter(fun p -> p.Ref <! Ready(gameRoom))
            log "Game Started"

        let! message = mailbox.Receive()
        match message with
        | AddPlayer (candidate) ->
            if players.Contains candidate then return! ignore()
            else return! continueWith (players.Add candidate)
        | RemovePlayer (candidate) ->
            if not(players.Contains candidate) then return! ignore()
            else return! continueWith (players.Remove candidate)
    }
    waitingForRoomToFill Set.empty<PlayerIdentity>


let PlayerActor waitingRoom id (mailbox:Actor<PlayerMessage>) =
    let log = printfn
    let loge = printfn "%A"
    let rec gamePlay (gameRoom:Actor.IActorRef) = actor {
        log "Playing games!!"
        gameRoom <! (sprintf "I am playing: %A" id)
    }

    let rec requestGameRoom() = actor {
        try
            log  "Waiting room"
            waitingRoom <! AddPlayer(PlayerIdentity.Create id (mailbox.Self))
            let! response = mailbox.Receive()
            match response with
            | Ready (a) -> return! gamePlay a
            | Failed    -> return! requestGameRoom()

        with
        |   e -> loge e
    }
    requestGameRoom()


let CreatePlayerActor waintingRoom = 
    let id = Guid.NewGuid()
    spawn ActorSystem (id.ToString()) (PlayerActor waintingRoom id)


let waitingRoom = spawn ActorSystem "WaitingRoom" WaitingRoomActor


let p1 = CreatePlayerActor waitingRoom
let p2 = CreatePlayerActor waitingRoom
let p3 = CreatePlayerActor waitingRoom





