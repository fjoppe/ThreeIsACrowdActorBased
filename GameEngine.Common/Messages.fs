namespace GameEngine.Common

open System
open Microsoft.FSharp.Reflection
open System.Runtime.Serialization
open System.Reflection
open Akka.Actor

open WebSharper

[<NamedUnionCases "type">]
type PlayerMessage =
    | Choice of int
    | WhatIsMyId

[<NamedUnionCases "type">]
type PlayerMessageResponse =
    | YourId of Guid
    | YouAreRegisterd of IActorRef
    | GameStarted of TileType * BoardSerializable
    | BoardHasChanged of TileColor list
    | ItIsYourTurn of int list
    | PlayerMadeChoice of TileType * int * bool
    | NoMoves
    | GameOver
    | Failed of String
    | Nothing


type RegisterPlayerMessage =
    | RegisterMe of IActorRef

