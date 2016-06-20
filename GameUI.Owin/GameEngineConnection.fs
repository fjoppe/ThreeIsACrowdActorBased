namespace ThreeIsACrowd

open Akka
open Akkling
open System.IO
open GameEngine.Common
open WebSharper.Owin.WebSocket.Server
open NLog

module GameEngineConnection =
    let log = LogManager.GetLogger("debug")

    let currentDir = Directory.GetCurrentDirectory()
    let configuration = Configuration.parse(File.ReadAllText(Path.Combine(currentDir, "..",  "akka.config")))

    let ActorSystem = System.create "GameStateClient" (configuration)

    let PlayerCommunicator (client : WebSocketClient<PlayerMessageResponseWebSocket, PlayerMessage>) =
        try
            let playerCommsActor (client : WebSocketClient<PlayerMessageResponseWebSocket, PlayerMessage>) (mailbox:Actor<obj>) =
                let rec loop senderActor = actor {
                        let! untypeMessage = mailbox.Receive()
                        match untypeMessage with
                        | :? PlayerMessage as message ->
                            match senderActor with
                            |   Some(dest) -> 
                                log.Debug "Send message to game"
                                dest <! message
                            |   None       -> mailbox.Stash()
                        | :? PlayerMessageResponse as message ->
                            match message with
                            | YouAreRegisterd(ref) -> 
                                log.Debug "I am registered"
                                mailbox.UnstashAll()
                                return! (loop (Some (ActorRefs.typed(ref))))
                            | _ -> 
                                log.Debug "Send message to client"
                                client.PostAsync (message.ToWebSocketType()) |> Async.Start
                        | _ -> ()

                        return! loop senderActor
                    }
                loop None
            
            spawn ActorSystem "playerReceiver" (playerCommsActor client)
            
        with
        | e -> printfn "%A" e
               reraise()


    let Start route : StatefulAgent<PlayerMessageResponseWebSocket, PlayerMessage, int> =
        fun client ->
            let clientIp = client.Connection.Context.Request.RemoteIpAddress
            
            let playerReceiver = PlayerCommunicator client
            let playerSender = ActorRefs.typed<PlayerMessage>(playerReceiver)

            let registerPlayer = select<RegisterPlayerMessage> ActorSystem "akka.tcp://GameStateServer@localhost:8080/user/RegisterPlayer"
            registerPlayer <! GameEngine.Common.RegisterMe(ActorRefs.untyped(playerReceiver))

            0, fun state message -> async {
                try
                    match message with
                    |   Message data -> 
                            log.Info "Websocket Server received Message"
                            playerSender <! data
                    |   Error   err  -> log.Error (sprintf "Websocket Server %A" err)
                    |   Close        -> log.Info "Websocket Server connection closed"
                    return state + 1
                with
                |   e -> printf "%A" e
                         return raise(e)
            }

