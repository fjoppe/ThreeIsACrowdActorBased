namespace GameEngine.FSharp

open GameEngine.Common

type PlayerStatus =
    | NoStatus      
    | ItsMyTurn of int list
    | TriggerAI 
    | NoMoves   
    | GameOver  
    | GameStarted of TileType
    | BoardHasChanged

[<Sealed>]
type Player =
      interface System.IComparable
      override Equals : yobj:obj -> bool
      override GetHashCode : unit -> int

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Player = 
    val Create      : System.Guid -> (PlayerStatus -> unit) -> Player
    val SendMessage : Player -> PlayerStatus -> unit

