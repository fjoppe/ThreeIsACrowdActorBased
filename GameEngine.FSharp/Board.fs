namespace GameEngine.FSharp


open System
open System.Runtime.Serialization
open System.Collections.Generic
open NLog;
open GameEngine.Common

module Map =
    let Values m = m |> Map.toSeq |> Seq.map(fun (a,b) -> b)


///<summary>
/// Direction of traversal
///</summary>
type Direction = 
    | NorthWest
    | North
    | NorthEast
    | SouthWest
    | South
    | SouthEast


///<summary>
/// The Board internal representation
///</summary>
[<DataContract>]
type Board =
    {
        [<field: DataMember(Name="TileList") >]
        TileList : Map<int, HexagonTile>

    }
    with
        //  =====================  Private Statics          =====================


        /// Logging tool
        static member logger = LogManager.GetLogger("debug"); 

        //  =====================  Private Instance members          =====================

        /// Retrieves list of all tiles
        member this.GetTileList() =
            this.TileList


        /// Get HexagonTile by tile id
        member private this.GetTileById(id) =
            this.TileList.Item(id)


        /// Retrieves the neighbour instance
        member private this.GetHexagonNodeForNeighbour(neighbour) =
            match neighbour with
            | None -> Empty
            | Some(x) -> Node(this.GetTileById(x))

        
        /// Retrieves the neighbour in the specified direction
        member private this.GetNeighbour tile direction = 
            match direction with
                | NorthWest -> this.GetHexagonNodeForNeighbour(tile.NorthWest)
                | North     -> this.GetHexagonNodeForNeighbour(tile.North)
                | NorthEast -> this.GetHexagonNodeForNeighbour(tile.NorthEast)
                | SouthWest -> this.GetHexagonNodeForNeighbour(tile.SouthWest)
                | South     -> this.GetHexagonNodeForNeighbour(tile.South)
                | SouthEast -> this.GetHexagonNodeForNeighbour(tile.SouthEast)


        ///<summary>
        ///  Retrieves a line from the given startTile, in the specified direction
        ///  The line always starts with the startTile and ends with a tile that either has no neighbour, or it complies to the stop condition.
        ///</summary>
        ///<returns>
        ///  Returns a sequence of tiles, the first element being the startTile and the last element is a tile compying to the stopcondition, or it has no neighbour in the specified direction.
        ///<returns>
        member private this.RetrieveLine startTile direction (stopCondition: HexagonTile -> bool) =
            let rec line currentElement skipFirst = seq {
                match currentElement with
                |   Empty           ->  yield!  Seq.empty                                           // terminate recursion with empty seq
                |   Node(element)   ->  yield   element                                             // return current element
                                        if skipFirst = true || stopCondition element = false then   // continue if stopcondition has not been met, or current is the start tile
                                            let next = this.GetNeighbour element direction
                                            yield! line next false                                  // recursive return the next tiles in the line
                                        // else, stop recursion
            }
            line startTile true


        ///<summary>
        ///     Generic function to find a line of tiles between a start tile and a searched TileType
        ///     Searches from a start tile in a specific direction, for a searchTileType, or stops when the stopCondition is met
        ///</summary>
        /// <returns>
        ///     Returns a sequence of HexagonTile if the searched tile type was found
        ///     Otherwise returns an empty sequence
        ///</returns>
        member private this.FindTileType startTile direction searchTileType (stopCondition: HexagonTile -> bool) =
            let startNode = HexagonNode.Node(startTile)                             //  convert to node, required input for this.RetrieveLine
            let stopFindCondition (elm:HexagonTile) = 
                elm.TileType = searchTileType ||            //  stop when tilecolor is found
                stopCondition(elm)                          //  stop when stopCondition is met
            let investigationSeq = 
                this.RetrieveLine startNode direction stopFindCondition             //  retrieve a sequence of tiles in the specified direction
            let result = 
                if Seq.length(investigationSeq) < 3 then Seq.empty                  //  in the case of no neighbour, or stopped at direct neighbour of start tile
                else
                    let candidate = Seq.last(investigationSeq)                      //  get the last element in the line
                    if stopCondition(candidate) = true then Seq.empty               //  return nothing when stopcondition was met
                    elif candidate.TileType = searchTileType then investigationSeq  //  return line if searched tile type was found
                    else Seq.empty
            result


        ///<summary>
        ///     Searches from a start tile in a specific direction, for a free board tile.
        ///     This free board tile is a choice candidate, when it may be used as a move.
        ///     When it is used as a move, it must enclose tiles from other players, between itself and the start tile.
        ///</summary>
        /// <returns>
        ///     Returns a HexagonNode, which is a Node with the found candidate, or Empty when no candidate was found.
        ///</returns>
        member private this.FindChoiceCandidateForDirection (startTile:HexagonTile) direction =
            let stopCondition (elm:HexagonTile) = 
                elm.TileType = startTile.TileType ||            //  stop when tilecolor is the same color as the start tile
                elm.Fortress=true                               //  or when a tile is fortressed
            let searchBoardLine = this.FindTileType startTile direction TileType.board stopCondition
            let result =
                if Seq.isEmpty(searchBoardLine) then Empty
                else
                    Node(Seq.last(searchBoardLine))
            result
        

        ///<summary>
        ///     Searches from a start tile in a specific direction, for a tile with color tileType
        ///     The tiles in between are turnable.
        ///</summary>
        /// <returns>
        ///     Returns a sequence of HexagonTile, which are turnable tiles
        ///     Returns an empty sequence when nothing was found
        ///</returns>
        member private this.FindTurnableTilesInLine startTile tileType direction =
            let stopCondition (elm:HexagonTile) = 
                elm.TileType = TileType.board     ||                //  stop when a board tile is found
                (elm.Fortress = true && elm.TileType <> tileType)   //  or when a tile is fortressed for a non searched color
            let searchBoardLine = this.FindTileType startTile direction tileType stopCondition
            let result = 
                if Seq.isEmpty(searchBoardLine) then Seq.empty                          //  Return empty sequence when nothing was found
                else
                    searchBoardLine
                    |> Seq.skip(1)                                                      //  Skip the start tile (so it doesn't matter which color it is!)
                    |> Seq.filter(fun elm -> elm.TileType <> tileType)                   //  Remove the searched tile color (which should be the last element)
            result


        ///<summary>
        /// Apply a function for all directions
        ///</summary>
        /// <returns>
        ///     Returns a sequence of element, for which the function was applied in all directions
        ///</returns>
        member private this.ApplyForAllDirections(processFunction:Direction->'a) =
            [   processFunction NorthWest;
                processFunction North;
                processFunction NorthEast;
                processFunction SouthWest;
                processFunction South;
                processFunction SouthEast;
            ]


        ///<summary>
        ///     Retrieve all potential choice candidates from a specific startTile, in all directions
        ///</summary>
        /// <returns>
        ///     Returns a sequence of HexagonNode, which are potential choices in regard to the start node
        ///</returns>
        member private this.FindChoiceCandidatesForTile (startTile:HexagonTile) =
            let findChoiceCandidate = this.FindChoiceCandidateForDirection startTile
            this.ApplyForAllDirections findChoiceCandidate
            |> Seq.filter(fun elm -> elm <> Empty)
            |> Seq.map(fun elm -> match elm with
                                    | Empty -> failwith "Empty is an illegal value, this should never occur"
                                    | Node(x) -> x.Id)

        ///<summary>
        ///     Retrieve all tiles which can be turned, which are enclose by tile choice and a tile with color tileType
        ///</summary>
        /// <returns>
        ///     Returns a sequence of HexagonTile, with tiles which may be turned.
        ///</returns>
        member private this.FindTurnableTilesForChoice choice tileType = 
            let findTurnableTiles = this.FindTurnableTilesInLine choice tileType
            this.ApplyForAllDirections findTurnableTiles


        ///<summary>
        ///     Retrieve all tiles which can be turned between choice and tiles with color tileType
        ///</summary>
        /// <returns>
        ///     Returns a sequence of HexagonTile, with tiles which may be turned.
        ///</returns>
        member private this.FindTurnableTiles choice tileType = 
            let allTurnabletiles = this.FindTurnableTilesForChoice choice tileType 
            allTurnabletiles
                |> Seq.collect(fun elm -> elm)
                |> Seq.distinct     // remove duplicates


        /// Turn tiles
        member this.TurnTiles(color, id, fortress) =
            let choiceTile = this.GetTileById(id)
            let affectedTiles = this.FindTurnableTiles choiceTile color
            let colorCapture = if Seq.length(affectedTiles) > 0 then Seq.last(affectedTiles).TileType
                               else TileType.none

            let allTiles = (this.GetTileList())
            let newTileList =  affectedTiles 
                               |> Seq.map(fun e -> e.Id)
                               |> Seq.fold(fun (m:Map<int, HexagonTile>) -> m.Remove) allTiles

            let newTileList = newTileList.Add(choiceTile.Id, { choiceTile with TileType = color; Fortress = fortress})                // add turn choice tile
            let newTileList = affectedTiles 
                              |> Seq.fold(fun (m:Map<int, HexagonTile>) e ->  m.Add(e.Id, { e with TileType = color })) newTileList   // add affected tiles
            
            let newBoard = {this with TileList    = newTileList;}
            (newBoard, colorCapture)


        //  =====================           Public Instance members          =====================


        ///<summary>
        ///     Retrieve all potential choice candidates for a specific color
        ///</summary>
        /// <returns>
        ///     Returns a sequence of HexagonNode, which are potential choices for the specified color
        ///</returns>
        member this.FindChoiceCandidates tileType = 
            this.GetTileList()
                |> Map.Values
                |> Seq.filter(fun elm -> elm.TileType = tileType)
                |> Seq.collect(fun elm -> this.FindChoiceCandidatesForTile elm)
                |> Seq.distinct     // remove duplicates


        ///<summary>
        ///     Retrieve the board data. Only use this function for game initialization.
        ///</summary>
        /// <returns>
        ///     Returns a BoardSerializable, containing all board data.
        ///</returns>
        member this.RetrieveBoardData() =
            let ConvertHexagonTileToSerializable(elm : HexagonTile) : HexagonTileSerializable =
                {Id = elm.Id; X=elm.X; Y=elm.Y; TileType=elm.TileType; TileValue=elm.TileValue; Fortress=elm.Fortress}
            let convertedList = this.GetTileList() |> Map.Values |> Seq.map(fun elm -> ConvertHexagonTileToSerializable(elm))
            Seq.toArray convertedList


        ///<summary>
        ///     Retrieve the current state of the board data.
        ///</summary>
        /// <returns>
        ///     A list TileColor, containing the current board data state
        ///</returns>
        member this.GetBoardState() =
            let ConvertHexagonTileToTileColor(elm : HexagonTile) : TileColor =
                { Id = elm.Id; TileType = elm.TileType; Fortress = elm.Fortress }
            this.GetTileList() |> Map.Values |> Seq.map (fun elm -> ConvertHexagonTileToTileColor(elm)) |> Seq.toList


        /// Retrieve a tile's neighbours
        member this.GetNeighbours id =
            let tile = this.GetTileById(id)
            [tile.NorthWest; tile.North; tile.NorthEast; tile.SouthWest; tile.South; tile.SouthEast]
            |> Seq.filter(fun element -> element.IsSome)
            |> Seq.map(fun id -> this.GetTileById(id.Value))


        //  =====================           Static members            =====================


        /// Converts a serialized representation to the internal format
        static member ConvertList(board : BoardSerializable) =
            //  Filter to get all valid hexagon tiles, which is everything except "none"
            let convertList = board.ActiveTileList |> Seq.filter(fun element -> element.TileType <> TileType.none)

            //  Convert serializable to internal format
            let hexagonList = convertList |> Seq.map(fun element -> HexagonTile.ConvertSerializable(convertList, element))

            //  Create mapping dictionary and return Board instance
            let dictionary = hexagonList |> Seq.fold(fun (m:Map<int, HexagonTile>) elm -> m.Add(elm.Id, elm)) Map.empty<int, HexagonTile>

            {
                TileList            = dictionary;
            }

