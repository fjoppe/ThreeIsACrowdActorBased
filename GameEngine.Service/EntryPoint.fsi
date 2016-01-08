namespace GameEngine.Service

open Akkling
open GameEngine.Common

[<AutoOpen>]
module EntryPoint =
    val RegisterPlayer : unit -> IActorRef<RegisterPlayerMessage>

