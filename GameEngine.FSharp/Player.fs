namespace GameEngine.FSharp

open System

open GameEngine.Common

type PlayerStatus =
    | ItIsYourTurn of int list
    | PlayerMadeChoice of TileType * int * bool
    | NoMoves   
    | GameOver  
    | GameStarted of TileType * BoardSerializable
    | BoardHasChanged of TileColor list


[<CustomEquality; CustomComparison>]
type Player = {
        Id          : Guid;
        SendMessage : (PlayerStatus -> unit)
    }    
    with
        member this.IdentifiesWith i = this.Id = i
        member this.GetId() = this.Id
        override x.Equals(yobj) =
            match yobj with
            | :? Player as y -> (x.Id = y.Id)
            | _ -> false
 
        override x.GetHashCode() = hash x.Id
        override x.ToString() = x.Id.ToString()
        interface System.IComparable with
            member x.CompareTo yobj =
                match yobj with
                | :? Player as y -> compare x.Id y.Id
                | _ -> invalidArg "yobj" "cannot compare values of different types"

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Player =
    let Create i sf = {Id = i; SendMessage = sf}
    let SendMessage p m = p.SendMessage(m)


