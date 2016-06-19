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
type PlayerMessageResponseWebSocket =
    | YourId of Guid
    | GameStarted of TileType * BoardSerializable
    | BoardHasChanged of TileColor list
    | ItIsYourTurn of int list
    | PlayerMadeChoice of TileType * int * bool
    | NoMoves
    | GameOver
    | Failed of String
    | Nothing

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
    with
        member this.ToWebSocketType() =
            match this with
            | YourId a           ->  PlayerMessageResponseWebSocket.YourId(a)
            | GameStarted (a,b)  ->  PlayerMessageResponseWebSocket.GameStarted(a,b)
            | BoardHasChanged a  ->  PlayerMessageResponseWebSocket.BoardHasChanged(a)
            | ItIsYourTurn a     ->  PlayerMessageResponseWebSocket.ItIsYourTurn(a)
            | PlayerMadeChoice (a,b,c) -> PlayerMessageResponseWebSocket.PlayerMadeChoice(a,b,c)
            | NoMoves            ->  PlayerMessageResponseWebSocket.NoMoves
            | GameOver           ->  PlayerMessageResponseWebSocket.GameOver
            | Failed a           ->  PlayerMessageResponseWebSocket.Failed(a)
            | Nothing            ->  PlayerMessageResponseWebSocket.Nothing
            | YouAreRegisterd a  -> failwith "Cannot convert YouAreRegisterd "

type RegisterPlayerMessage =
    | RegisterMe of IActorRef

