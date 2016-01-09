namespace GameEngine.Service

open System

open Akkling
open InternalMessages

module AIPlayer =
    val CreateAIPlayerActor : ICanTell<WaitingRoomMessage> -> Guid -> IActorRef<GameServiceConnectionMessage>

