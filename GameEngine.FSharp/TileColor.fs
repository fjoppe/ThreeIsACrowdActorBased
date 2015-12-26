namespace GameEngine.FSharp

open System
open System.Runtime.Serialization


[<DataContract>]
type TileColor = {
    [<field: DataMember(Name="id") >]
    Id : int
    
    [<field: DataMember(Name="color") >]
    TileType : TileType

    [<field: DataMember(Name="Fortress") >]
    Fortress : bool
    }



