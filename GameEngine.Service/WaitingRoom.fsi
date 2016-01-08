namespace GameEngine.Service

open Akkling
open InternalMessages

[<AutoOpen>]
module WaitingRoom =
    /// Creates the waiting room actor
    val WaitingRoom : IActorRef<WaitingRoomMessage>


