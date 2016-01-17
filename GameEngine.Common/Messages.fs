namespace GameEngine.Common

open System
open Microsoft.FSharp.Reflection
open System.Runtime.Serialization
open System.Reflection
open Akka.Actor


type PlayerMessage =
    | Choice of int
    | WhatIsMyId


type PlayerMessageResponse =
    | YourId of Guid
    | YouAreRegisterd of IActorRef
    | GameStarted of TileType * BoardSerializable
    | BoardHasChanged of TileColor list
    | ItIsYourTurn of int list
    | NoMoves
    | GameOver
    | Failed of String
    | Nothing


type RegisterPlayerMessage =
    | RegisterMe of IActorRef

