open System.Reflection

[<assembly: AssemblyTitle("Game Service")>]
()

open System

open Topshelf
open Time
open NLog
open GameEngine.Service


module GameService =
    let log = LogManager.GetLogger("debug")

    try
        let sys = GameActorSystem.ActorSystem
        sys |> ignore
    with
    |   e -> log.Error e

    let regPlayer = EntryPoint.RegisterPlayer()

    let start ctx =
        log.Debug "GameService started"
        true

    let stop ctx =
        log.Debug "GameService stopped"
        true


[<EntryPoint>]
let main argv =
    Service.Default
    |> with_start GameService.start
//    |> with_recovery (ServiceRecovery.Default |> restart (min 10))
    |> with_stop GameService.stop
    |> run
