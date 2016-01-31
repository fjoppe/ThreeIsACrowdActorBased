namespace GameUI.Suave.ThreeIsACrowd

open Akka
open Akkling
open System.IO
open GameEngine.Common
open NLog
open Suave.Utils
open Suave.Sockets.Control
open Suave.WebSocket

module GameEngineConnection =
    let logger = LogManager.GetLogger("debug"); 

    let configuration = Configuration.parse(File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/akka.config")))

    let ActorSystem = System.create "GameStateClient" (configuration)

    let WebSocketServer (webSocket : WebSocket) =
        let SendToWebsocket = new MailboxProcessor<PlayerMessageResponse>(fun inbox ->
            let rec loop () = async {
                    let! message = inbox.Receive()
                    let data = UTF8.bytes (WebSharper.Json.Serialize message)                    
                    socket {
                        do! webSocket.send Text data true
                    } |> ignore
                    return! loop()
                }
            loop()
        )

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
                        | YourId(id) ->  
                            logger.Debug (sprintf "Your Id: %A" id)
                            SendToWebsocket.Post message
                        | YouAreRegisterd(ref) -> 
                            logger.Debug (sprintf "I am registered")
                            SendToWebsocket.Post message
                            mailbox.UnstashAll()
                            return! (loop (Some (ActorRefs.typed(ref))))
                        | GameStarted(color, board) -> 
                            logger.Debug (sprintf "Game started and color: %A, board: %A" color board)
                            SendToWebsocket.Post message
                        | GameOver ->  
                            logger.Debug (sprintf "Game Over")
                            SendToWebsocket.Post message
                        | PlayerMadeChoice(c,i,f) -> 
                            logger.Debug (sprintf "Player made choice: %A, %d" c i)
                            SendToWebsocket.Post message
                        | ItIsYourTurn(possibleMoves) -> 
                            logger.Debug (sprintf "It is your turn, possible: %A" possibleMoves)
                            SendToWebsocket.Post message
                        | BoardHasChanged(state) -> 
                            logger.Debug (sprintf "Board has changed")
                            SendToWebsocket.Post message
                        | NoMoves -> 
                            logger.Debug (sprintf "No possible moves")
                            SendToWebsocket.Post message
                        | Failed err ->  
                            logger.Debug (sprintf "Failed: %s" err)
                            SendToWebsocket.Post message
                        | Nothing ->  
                            logger.Debug (sprintf "Nothing")
                            SendToWebsocket.Post message
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

        fun cx -> 
            //  infinite loop to send message from socket to player actor
            let rec loop()=
                socket {
                    let! msg = webSocket.read()
                    match msg with
                    | (Text, data, true) ->
                        let str = UTF8.toString data
                        let playerMessage = WebSharper.Json.Deserialize str
                        playerSender <! playerMessage
                        return! loop()
                    | (Close, _, _) ->
                        do! webSocket.send Close [||] true                    
                    | _ -> return! loop()
                }
            loop()


