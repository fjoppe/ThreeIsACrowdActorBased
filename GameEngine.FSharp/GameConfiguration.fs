namespace GameEngine.FSharp

open System
open System.Runtime.Serialization

[<DataContract>]
type GameConfiguration = {
        [<field: DataMember(Name="GameId") >]
        GameId : Guid;
    }


