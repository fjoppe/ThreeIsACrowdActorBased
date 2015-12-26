namespace GameEngine.FSharp

open System
open System.Runtime.Serialization


///<summary>The Board representation used for file-storage
///</summary>
[<DataContract(Namespace ="urn:TIC")>]
type BoardSerializable = {
    [<field: DataMember(Name="FortressesPerPlayer") >]
    FortressesPerPlayer : int;

    [<field: DataMember(Name="ActiveTileList") >]
    ActiveTileList : HexagonTileSerializable[];
    }
