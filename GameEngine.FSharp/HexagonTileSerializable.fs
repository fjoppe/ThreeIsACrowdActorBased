namespace GameEngine.FSharp

open System
open System.Runtime.Serialization

///<summary>The Hexagon representation used for file-storage
///</summary>
[<DataContract(Namespace ="urn:TIC")>]
type HexagonTileSerializable = {
    [<field: DataMember(Name="Id") >]
    Id : int

    [<field: DataMember(Name="X") >]
    X : int

    [<field: DataMember(Name="Y") >]
    Y : int

    [<field: DataMember(Name="TileType") >]
    TileType : TileType

    [<field: DataMember(Name="TileValue") >]
    TileValue : int
    
    [<field: DataMember(Name="Fortress") >]
    Fortress : bool
    }
