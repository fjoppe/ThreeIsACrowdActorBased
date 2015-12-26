open System.Reflection

[<assembly: AssemblyTitle("Game Service")>]
()

open System

open Topshelf
open Time
open NLog;
open GameEngine.FSharp

module GameService =
    let log = LogManager.GetLogger("debug")

    let WaitingRoom = lazy (GameActorSystem.WaitingRoom())
    let RegisterPlayer = lazy (GameActorSystem.RegisterPlayer WaitingRoom.Value)

    let start ctx =
        log.Debug "Starting GameService"
        WaitingRoom.Force() |> ignore
        RegisterPlayer.Force() |> ignore
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
