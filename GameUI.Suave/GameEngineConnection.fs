namespace GameUI.Suave.ThreeIsACrowd

open Akka
open Akkling
open System.IO
open GameEngine.Common

module GameEngineConnection =
    let configuration = Configuration.parse(File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/akka.config")))

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
        actor


    // this gives a warning which you can ignore
    let registerPlayer = select<RegisterPlayerMessage> ActorSystem "akka.tcp://GameStateServer@localhost:8080/user/RegisterPlayer"


    let playerReceiver = CreatePlayerCommunicator()
    let playerSender = ActorRefs.typed<PlayerMessage>(playerReceiver)

    registerPlayer <! GameEngine.Common.RegisterMe(ActorRefs.untyped(playerReceiver))

