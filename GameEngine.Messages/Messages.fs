namespace GameEngine.Messages

open System
open Microsoft.FSharp.Reflection
open System.Runtime.Serialization
open System.Reflection
open Akka.Actor


type PlayerMessage =
    | Choice
    | WhatIsMyId


[<Serializable>]
type PlayerMessageResponse =
    | YourId of string
    | Nothing
//    static member GetKnownTypes()=
//        typedefof<PlayerMessageResponse>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic)
//        |> Array.filter FSharpType.IsUnion

type RegisterPlayerMessage =
    | RegisterMe
    | YouAreRegisterd of IActorRef
//        | UnregisterMe of Guid

