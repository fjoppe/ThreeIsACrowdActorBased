namespace GameEngine.FSharp

open System
open System.Runtime.Serialization
open System.Collections.Generic


///<summary>
/// Neighbour type, either None or Tile id
///</summary>
type Neighbour = int option

///<summary>
/// HexagonNode is a reference to HexagonTile, can be Empty or a Node of HexagonTile
///</summary>
[<DataContract>]
type HexagonNode = Empty | Node of HexagonTile


///<summary>
/// Hexagon Tile record
///</summary>
and HexagonTile = {

    [<field: DataMember(Name="NorthWest") >]
    NorthWest : Neighbour;

    [<field: DataMember(Name="North") >]
    North : Neighbour;

    [<field: DataMember(Name="NorthEast") >]
    NorthEast : Neighbour;

    [<field: DataMember(Name="SouthWest") >]
    SouthWest : Neighbour;

    [<field: DataMember(Name="South") >]
    South : Neighbour;

    [<field: DataMember(Name="SouthEast") >]
    SouthEast : Neighbour;

    [<field: DataMember(Name="Id") >]
    Id : int;

    [<field: DataMember(Name="X") >]
    X : int;

    [<field: DataMember(Name="Y") >]
    Y : int;

    [<field: DataMember(Name="TileType") >]
    TileType : TileType;

    [<field: DataMember(Name="TileValue") >]
    TileValue : int;
    
    [<field: DataMember(Name="Fortress") >]
    Fortress : bool;
    }
    with
        //  =====================           Instance members          =====================

        ///<summary>
        /// Retrieves the amount of linked neighbours
        ///</summary>
        member this.NeighbourCount =
            [this.NorthWest; this.North; this.NorthEast; this.SouthWest; this.South; this.SouthEast]
            |> Seq.filter(fun element -> element.IsSome)  
            |> Seq.length


        //  =====================           Static members          =====================

        static member private xMarge = 43
        static member private yMarge = 26
        static member private yFull = 52
        static member private xFull = 60


        static member private FindNeighbour(sourceList :  seq<HexagonTileSerializable>, x, y) =
            let element = sourceList |> Seq.tryFind(fun element -> element.X = x && element.Y = y) 
            match element with
                | Option.None -> None
                | Option.Some(x) -> Some(x.Id)


        ///<summary>
        /// Converts a serializable hexagon to internal format
        ///</summary>
        static member ConvertSerializable(sourceList : seq<HexagonTileSerializable>, source : HexagonTileSerializable ) =
            let FindByRelativePosition relX relY = 
                HexagonTile.FindNeighbour(sourceList, source.X + relX , source.Y + relY)
            {
                Id = source.Id; 
                NorthWest = FindByRelativePosition -HexagonTile.xMarge  -HexagonTile.yMarge;
                North =     FindByRelativePosition -0                   -HexagonTile.yFull;
                NorthEast = FindByRelativePosition +HexagonTile.xMarge  -HexagonTile.yMarge;
                SouthWest = FindByRelativePosition -HexagonTile.xMarge  +HexagonTile.yMarge;
                South =     FindByRelativePosition -0                   +HexagonTile.yFull; 
                SouthEast = FindByRelativePosition +HexagonTile.xMarge  +HexagonTile.yMarge;
                X = source.X; 
                Y = source.Y; 
                TileType = source.TileType; 
                TileValue = source.TileValue; 
                Fortress = source.Fortress
            }            

