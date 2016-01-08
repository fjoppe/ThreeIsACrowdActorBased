namespace GameEngine.Service

open System

open Akkling
open InternalMessages

module Player =
    val CreateAIPlayerActor : ICanTell<WaitingRoomMessage> -> Guid -> IActorRef<GameServiceConnectionMessage>

