namespace ThreeIsACrowd

open Akka
open Akkling
open System.IO
open GameEngine.Common
open WebSharper.Owin.WebSocket.Server

module GameEngineConnection =
    let configuration = Configuration.parse(File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/akka.config")))

    let ActorSystem = System.create "GameStateClient" (configuration)

    let PlayerCommunicator (client : WebSocketClient<PlayerMessageResponseWebSocket, PlayerMessage>) =
        let playerCommsActor (mailbox:Actor<obj>) =
            let rec loop senderActor = actor {
                    let! untypeMessage = mailbox.Receive()
                    match untypeMessage with
                    | :? PlayerMessage as message ->
                        match senderActor with
                        |   Some(dest) -> dest <! message
                        |   None       -> mailbox.Stash()
                    | :? PlayerMessageResponse as message ->
                        match message with
                        | YouAreRegisterd(ref) -> 
                            printfn "I am registered"
                            mailbox.UnstashAll()
                            return! (loop (Some (ActorRefs.typed(ref))))
                        | _ -> client.PostAsync (message.ToWebSocketType())
                    | _ -> ()

                    return! loop senderActor
                }
            loop None
            
        let actor = spawn ActorSystem "playerReceiver" playerCommsActor
        ActorRefs.typed<PlayerMessage>(actor)


    let Start route : StatefulAgent<PlayerMessageResponseWebSocket, PlayerMessage, int> =
        fun client ->
            let clientIp = client.Connection.Context.Request.RemoteIpAddress
            let playerActor = PlayerCommunicator client

            let registerPlayer = select<RegisterPlayerMessage> ActorSystem "akka.tcp://GameStateServer@localhost:8080/user/RegisterPlayer"
            registerPlayer <! GameEngine.Common.RegisterMe(ActorRefs.untyped(playerActor))

            0, fun state message -> async {
                match message with
                |   Message data -> playerActor <! data
                |   Error   err  -> printf "Error"
                |   Close        -> printf "Connection closed"
                return state + 1
            }


//                            | YourId(id) ->  printfn "Your Id: %A" id
//                            | GameStarted(color, board) -> printfn "Game started and color: %A, board: %A" color board
//                            | GameOver ->  printfn "Game Over"
//                            | PlayerMadeChoice(c,i,f) -> printfn "Player made choice: %A, %d" c i
//                            | ItIsYourTurn(possibleMoves) -> printfn "It is your turn, possible: %A" possibleMoves
//                            | BoardHasChanged(state) -> printfn "Board has changed"
//                            | NoMoves -> printfn "No possible moves"
//                            | Failed err ->  printfn "Failed: %s" err
//                            | Nothing ->  printfn "Nothing"

