namespace GameEngine.Service

open System
open Akkling
open Common
open GameEngine.FSharp
open GameEngine.Common

module InternalMessages =
    type GameRoomMessage = class end

    type DynamicConnection =
        |   Data of PlayerMessage
        |   LooseConnection

    /// A message to/from the GameServiceConnection actor
    type GameServiceConnectionMessage =
        |   ToGame   of DynamicConnection
        |   ToPlayer of PlayerMessageResponse
        |   ChangeConnection of IActorRef<DynamicConnection>


    /// Contains extended/convenient player identity info
    [<CustomEquality; CustomComparison>]
    type PlayerIdentity = {
            Id         : Guid                                     // how he/she should be remembered
            Ref        : IActorRef<GameServiceConnectionMessage>  // how one should talk to him/her
            PlayerType : PlayerType
        }
        with
            override x.Equals(yobj) =
                match yobj with
                | :? PlayerIdentity as y -> (x.Id = y.Id)
                | _ -> false
 
            override x.GetHashCode() = hash x.Id
            interface System.IComparable with
                member x.CompareTo yobj =
                    match yobj with
                    | :? PlayerIdentity as y -> compare x.Id y.Id
                    | _ -> invalidArg "yobj" "cannot compare values of different types"
            static member Create i r pt = {Id = i; Ref = r; PlayerType = pt}


    /// A message from the (ai) player to a Waitingroom
    type WaitingRoomMessage =
        | AddPlayer    of PlayerIdentity
        | RemovePlayer of PlayerIdentity
        | AddAIPlayer  of Guid * PlayerIdentity


