(* 
    Calling remote actors
*)

#r @"..\packages\Akka.1.0.5\lib\net45\Akka.dll"
#r @"..\packages\Akkling.0.2.1\lib\net45\Akkling.dll"
#r @"..\packages\Akka.Remote.1.0.5\lib\net45\Akka.Remote.dll"
#r @"..\packages\Helios.1.4.1\lib\net45\Helios.dll"
#r @"..\packages\Google.ProtocolBuffers.2.4.1.521\lib\net40\Google.ProtocolBuffers.dll"
#r @"..\packages\Google.ProtocolBuffers.2.4.1.521\lib\net40\Google.ProtocolBuffers.Serialization.dll"
#r @"..\GameEngine.Common\bin\Debug\GameEngine.Common.dll"
#r @"..\packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll"
#r @"..\packages\Wire.0.0.6\lib\Wire.dll"
#I @"..\packages\FsPickler.1.7.1\lib\net45"

open Akka
open Akkling
open GameEngine.Common

exception TechError of string

let configuration = Configuration.parse(@"
    akka {
        actor {
            provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
        }
        remote {
            helios.tcp {
                port = 8090
                hostname = localhost
            }
        }
    }
    ")


type CommsFunc = IActorRef<PlayerMessage> -> unit

let ActorSystem = System.create "GameStateClient" (configuration)

let PlayerCommunicator (mailbox:Actor<obj>) =
    let rec loop senderActor = actor {
            let! untypeMessage = mailbox.Receive()
            match untypeMessage with
            | :? PlayerMessage as message ->
                match senderActor with
                |   Some(dest) -> dest <! message
                |   None       -> mailbox.Stash()
            | :? PlayerMessageResponse as message ->
                match message with
                | YourId(id) ->  printfn "Your Id: %A" id
                | YouAreRegisterd(ref) -> 
                    printfn "I am registered"
                    mailbox.UnstashAll()
                    return! (loop (Some (ActorRefs.typed(ref))))
                | GameStarted(color, board) -> printfn "Game started and color: %A, board: %A" color board
                | GameOver ->  printfn "Game Over"
                | PlayerMadeChoice(c,i,f) -> printfn "Player made choice: %A, %d" c i
                | ItIsYourTurn(possibleMoves) -> printfn "It is your turn, possible: %A" possibleMoves
                | BoardHasChanged(state) -> printfn "Board has changed"
                | NoMoves -> printfn "No possible moves"
                | Failed err ->  printfn "Failed: %s" err
                | Nothing ->  printfn "Nothing"
            | _ -> ()

            return! loop senderActor
        }
    loop None
    
let CreatePlayerCommunicator() = 
    let actor = spawn ActorSystem "playerReceiver" PlayerCommunicator
//    ActorRefs.typed<PlayerMessage>(actor)
    actor


// this gives a warning which you can ignore
let registerPlayer = select<RegisterPlayerMessage> ActorSystem "akka.tcp://GameStateServer@localhost:8080/user/RegisterPlayer"


let playerReceiver = CreatePlayerCommunicator()
let playerSender = ActorRefs.typed<PlayerMessage>(playerReceiver)

playerSender <! WhatIsMyId

registerPlayer <! GameEngine.Common.RegisterMe(ActorRefs.untyped(playerReceiver))

playerSender <! WhatIsMyId

playerSender <! Choice 7

// other moves - if possible, ai is not predictable
//playerSender <! Choice 13
//playerSender <! Choice 24
