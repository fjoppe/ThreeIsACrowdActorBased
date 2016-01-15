namespace GameEngine.Common

open System
open System.Runtime.Serialization
open GameEngine.Common

[<DataContract>]
type TileColor = {
    [<field: DataMember(Name="id") >]
    Id : int
    
    [<field: DataMember(Name="color") >]
    TileType : TileType

    [<field: DataMember(Name="Fortress") >]
    Fortress : bool
    }



