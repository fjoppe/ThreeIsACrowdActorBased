namespace GameEngine.Messages

open System
open Microsoft.FSharp.Reflection
open System.Runtime.Serialization
open System.Reflection

type RegisterPlayerMessage =
    | RegisterMe
//        | UnregisterMe of Guid


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

