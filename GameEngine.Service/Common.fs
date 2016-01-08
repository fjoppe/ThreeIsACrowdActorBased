namespace GameEngine.Service

open System
open NLog;

module Common =
//    let logger = LogManager.GetLogger("debug")

    [<Literal>] 
    let waitForAIToRegister = 5

    [<Literal>] 
    let Player = "Player"

    [<Literal>] 
    let GamePlayer = "GamePlayer"

    [<Literal>] 
    let PlayerConnection = "PlayerConnection"

    [<Literal>] 
    let GameServiceConnection = "GameServiceConnection"


    [<Literal>] 
    let GameRoom = "GameRoom"

    /// Create a readable Actor name
    let CreateName s g = sprintf "%s_%A" s g



