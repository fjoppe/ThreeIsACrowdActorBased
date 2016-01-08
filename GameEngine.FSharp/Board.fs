namespace GameEngine.FSharp


open System
open System.Runtime.Serialization
open System.Collections.Generic
open NLog;
open GameEngine.Common

module Map =
    let Values m = m |> Map.toSeq |> Seq.map(fun (a,b) -> b)


/// Type of player
type PlayerType = 
    | Human
    | Computer


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


/// Player information
[<DataContract>]
type PlayerInfo =
    {
        [<field: DataMember(Name="PlayerId") >]
        PlayerId    : Guid;

        [<field: DataMember(Name="Color") >]
        Color       : TileType;

        [<field: DataMember(Name="Fortresses") >]
        Fortresses  : int;

        [<field: DataMember(Name="PlayerType") >]
        PlayerType  : PlayerType

        [<field: DataMember(Name="Status") >]
        Status      : Queue<PlayerStatus>
    }
    with
         //  member variables
        static member private logger = LogManager.GetLogger("debug"); 

       
        //  =====================  Public Instance members          =====================

        /// Consume fortress
        member this.ConsumeFortress(consume) =
            if consume then
                if this.Fortresses = 0 then
                        let message = String.Format("No more fortresses: {0}", this.PlayerId)
                        PlayerInfo.logger.Error(message)
                        failwith message
                else
                    { this with Fortresses = this.Fortresses - 1 }
            else
                this


        /// Add a new status message
        member this.AddMessage(message) =
            let newQueue = new Queue<_>(this.Status)
            newQueue.Enqueue(message)
            { this with Status = newQueue }

        /// Peek message
        member this.ReadMessage() = 
            if this.Status.Count = 0 then PlayerStatus.none
            else this.Status.Peek()

        /// Remove message
        member this.RemoveMessage() = 
            let newQueue = new Queue<_>(this.Status)
            ignore(newQueue.Dequeue())
            { this with Status = newQueue }

        member this.SetGameOver() = 
            let newQueue = new Queue<_>()
            newQueue.Enqueue(PlayerStatus.gameOver)
            { this with Status = newQueue }


///<summary>
/// The Board internal representation
///</summary>
[<DataContract>]
type Board =
    {
        [<field: DataMember(Name="TileList") >]
        TileList : Map<int, HexagonTile>

        [<field: DataMember(Name="FortressesPerPlayer") >]
        FortressesPerPlayer : int

        [<field: DataMember(Name="Players") >]
        Players : PlayerInfo list

        [<field: DataMember(Name="CurrentTurn") >]
        CurrentTurn : int

        [<field: DataMember(Name="TurnOrder") >]
        TurnOrder : TileType list

        [<field: DataMember(Name="GameOver") >]
        GameOver    : bool
    }
    with
        //  =====================  Private Statics          =====================

        /// The amount of AI players
//        static member private numberOfAI = 2

        /// Logging tool
        static member logger = LogManager.GetLogger("debug"); 

        //  =====================  Private Instance members          =====================

        /// Retrieves list of all tiles
        member private this.GetTileList() =
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
            let stopFindCondition elm = elm.TileType = searchTileType ||            //  stop when tilecolor is found
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
        member private this.FindChoiceCandidateForDirection startTile direction =
            let stopCondition elm = elm.TileType = startTile.TileType ||            //  stop when tilecolor is the same color as the start tile
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
            let stopCondition elm = elm.TileType = TileType.board     ||                //  stop when a board tile is found
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
        member private this.FindChoiceCandidatesForTile startTile =
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


        /// Get player info by playerId
        member private this.GetPlayerInfo(playerId : Guid, expression) =
            let found = this.Players |> Seq.tryFind(fun elm -> elm.PlayerId = playerId)
            if found.IsSome then expression(found.Value)
                else
                    Board.logger.Error("Don't know this player: {0}", playerId);
                    raise(Exception("Unknown player"))


        /// Get player info by player color
        member private this.GetPlayerInfo(color:TileType, expression) =
            let found = this.Players |> Seq.tryFind(fun elm -> elm.Color = color)
            if found.IsSome then expression(found.Value)
                else
                    Board.logger.Error("Don't know this color: {0}", color);
                    raise(Exception("Unknown player"))


        ///     Get color for a new playerInfo, before it is added to the list
        member private this.GetColor(currentPlayers : PlayerInfo list) =
            match (List.length currentPlayers) with
            |   0   ->  TileType.yellow
            |   1   ->  TileType.blue
            |   2   ->  TileType.red
            |   _   ->  failwith "exceeds maximum index"


        ///     Fill the remaining seats of players with AI players
        member private this.FillWithAIPlayers(currentPlayers : PlayerInfo list, fortressesPerPlayer, numberOfAI : int) =
            Board.logger.Debug(String.Format("FillWithAIPlayers: {0}", numberOfAI));
            let playersNew = currentPlayers
            if (List.length playersNew) < 3 then 
                let newPlayer = 
                    {
                        PlayerId    = Guid.NewGuid() ; 
                        Color       = this.GetColor(playersNew); 
                        Fortresses  = fortressesPerPlayer; 
                        PlayerType  = PlayerType.Computer;
                        Status      = new Queue<_>();
                    }
                let playersNew = List.append playersNew [newPlayer]
                this.FillWithAIPlayers(playersNew, fortressesPerPlayer, numberOfAI - 1)
            else 
                playersNew


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


        /// update player
        member private this.UpdatePlayer(playerInfo) =
            let newPlayers = playerInfo :: (this.Players |> List.filter(fun e -> e.PlayerId <> playerInfo.PlayerId))
            { this with Players = newPlayers }


        /// Turn tiles
        member private this.TurnTiles(color, id, fortress) =
            let choiceTile = this.GetTileById(id)
            let affectedTiles = this.FindTurnableTiles choiceTile color
            let colorCapture = if Seq.length(affectedTiles) > 0 then Seq.last(affectedTiles).TileType
                               else TileType.none

            let allTiles = (this.GetTileList())
            let newTileList =  affectedTiles 
                               |> Seq.map(fun e -> e.Id)
                               |> Seq.fold(fun (m:Map<int, HexagonTile>) -> m.Remove) allTiles

            let newTileList = newTileList.Add(choiceTile.Id, { choiceTile with TileType = color; Fortress = fortress})              // add turn choice tile
            let newTileList = affectedTiles 
                              |> Seq.fold(fun (m:Map<int, HexagonTile>) e ->  m.Add(e.Id, { e with TileType = color })) newTileList                         // add affected tiles
            
            //this.GetTileList() |> Seq.iter(fun elm -> if not(newTileList.ContainsKey(elm.Id)) then newTileList.Add(elm.Id, elm))  // add remaining elements
            let newBoard = {this with TileList    = newTileList;}
            (newBoard, colorCapture)


        /// Update turn order if required
        member private this.UpdateTurnOrder(colorCapture) = 
            if (List.length this.TurnOrder) < 3 then
                let newTurnOrder = 
                    match colorCapture with
                    |   TileType.red  -> [TileType.red; TileType.blue] |> List.append this.TurnOrder
                    |   TileType.blue -> [TileType.blue;TileType.red]   |> List.append this.TurnOrder
                    | _ -> failwith "impossible capture color"
                { this with TurnOrder   = newTurnOrder; }
            else
                this


        /// Retrieves next current turn
        member private this.GetNextCurrentTurn() =
            let currentTurn = this.CurrentTurn + 1
            if currentTurn = 3 then
                { this with CurrentTurn = 0 }
            else 
                { this with CurrentTurn = currentTurn }


        /// Initialize next turn
        member private this.InitNextTurn() =
            let rec initNextTurnRec(instance:Board, skips) = 
                let nextTurn = instance.GetNextCurrentTurn()
                let possibilities = nextTurn.FindChoiceCandidates(nextTurn.GetCurrentTurn())
                if Seq.isEmpty possibilities then 
                    let nextTurnColor = nextTurn.GetCurrentTurn()
                    let nextTurnPlayerNoMoves = nextTurn.GetPlayerInfo(nextTurnColor, fun pi -> pi ).AddMessage(PlayerStatus.noMoves)
                    let nextTurn = nextTurn.UpdatePlayer(nextTurnPlayerNoMoves)
                    if skips < 3 then
                        initNextTurnRec(nextTurn, skips+1)
                    else
                        let newPlayers = nextTurn.Players |> List.map(fun elm -> elm.SetGameOver()) 
                        { nextTurn with Players = newPlayers; GameOver = true }
                else
                    nextTurn.SetTurn()
            initNextTurnRec(this, 0)


        /// Set the current turn
        member this.SetTurn() = 
            let color = this.GetCurrentTurn()
            let setTurn = this.GetPlayerInfo(color, fun pi -> pi).AddMessage(PlayerStatus.itsMyTurn)
            this.UpdatePlayer(setTurn)


        /// Turns tiles for the specified move
        member private this.InvokePlayerMove(color, id, fortress) =
            let (newBoard, colorCapture) = this.TurnTiles(color, id, fortress)
            let newBoard = newBoard.UpdateTurnOrder(colorCapture)
            let newBoard = newBoard.UpdatePlayer(this.GetPlayerInfo(color, fun pi -> pi ).ConsumeFortress(fortress))
            newBoard


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
            {
                FortressesPerPlayer = this.FortressesPerPlayer;
                ActiveTileList = Seq.toArray(convertedList);
            }


        ///<summary>
        ///     Retrieve the current state of the board data.
        ///</summary>
        /// <returns>
        ///     A list TileColor, containing the current board data state
        ///</returns>
        member this.GetBoardState() =
            let ConvertHexagonTileToTileColor(elm : HexagonTile) : TileColor =
                { Id = elm.Id; TileType = elm.TileType; Fortress = elm.Fortress }
            this.GetTileList() |> Map.Values |> Seq.map (fun elm -> ConvertHexagonTileToTileColor(elm)) |> Seq.toArray
        
        
        ///<summary>
        ///     Retrieve Board statistics
        ///</summary>
        /// <returns>
        ///     A BoardStats Record containing current board statistics.
        ///</returns>
        member this.GetStatistics() =
            let fortressCount color =  this.Players |> List.find(fun elm -> elm.Color = color) |> fun e -> e.Fortresses
            let countColor color = this.GetTileList() |> Map.Values |> Seq.filter(fun elm -> elm.TileType = color) |> Seq.length;
            {
                RedCount        = countColor TileType.red;
                BlueCount       = countColor TileType.blue;
                YellowCount     = countColor TileType.yellow;
                YellowFortress  = fortressCount TileType.yellow; 
                BlueFortress    = fortressCount TileType.blue;
                RedFortress     = fortressCount TileType.red;
            }


        ///     Choose turn for player
        member this.ChooseTurn(color, id, fortress) = 
            let newBoard = this.InvokePlayerMove(color, id, fortress)
            let newBoard = newBoard.InitNextTurn()
            newBoard


        ///     Register a player
        member this.RegisterPlayer(playerId:Guid, fortressesPerPlayer) =
            let playersNew = this.Players
            let newPlayer = 
                {
                    PlayerId    = playerId; 
                    Color       = this.GetColor(playersNew);
                    Fortresses  = fortressesPerPlayer; 
                    PlayerType  = PlayerType.Human
                    Status      = new Queue<_>();
                }

            let playersNew = List.append playersNew [newPlayer]

//            if (List.length playersNew) + numberOfAI = 3 then
//                // the board is fully initialized
//                let initializedBoard = { this with Players = this.FillWithAIPlayers(playersNew, fortressesPerPlayer, numberOfAI); }
//
//                //  initialize next turn
//                let runnableBoard = initializedBoard.SetTurn()
//                runnableBoard 
//            else
            { this with Players = playersNew; }


        ///<summary>
        ///     Retrieve color of the player, when available
        ///</summary>
        /// <returns>
        ///     When available, the player color
        ///     When unavailable the color "none"
        ///</returns>
        member this.GetPlayerColor(playerId:Guid) =
            if List.isEmpty this.Players then TileType.none
            else
                this.GetPlayerInfo(playerId, fun pi -> pi.Color)


        /// Retrieve Player information by Tile Color
        member this.GetPlayerInformation(tileColor:TileType) =
            if List.isEmpty this.Players then Option.None
            else
                Option.Some(this.GetPlayerInfo(tileColor, fun pi -> pi))


        /// Retrieve Player information by Tile Color
        member this.GetPlayerInformation(playerId : Guid) =
            if List.isEmpty this.Players then Option.None
            else
                Option.Some(this.GetPlayerInfo(playerId, fun pi -> pi))


        ///<summary>
        ///     Queries whether the player still has fortresses left
        ///</summary>
        /// <returns>
        ///     true    -> the player still has fortresses left
        ///     false   -> the player has no fortresses left
        ///</returns>
        member this.HasFortresses(playerId:Guid) =
            this.GetPlayerInfo(playerId, fun pi -> (pi.Fortresses > 0))


        /// Gets the color for the current turn
        member this.GetCurrentTurn() : TileType = 
            if not this.GameOver then
                this.TurnOrder.[this.CurrentTurn]
            else
                TileType.none
        

        /// Retrieve current status for player
        member this.GetStatus(playerId : Guid) =
            let playerInfo = this.GetPlayerInfo(playerId, fun pi ->  pi)

            let currentMessage = playerInfo.ReadMessage()
            if currentMessage <> PlayerStatus.none then 
                (currentMessage, this.UpdatePlayer(playerInfo.RemoveMessage()))
            else
                (currentMessage, this)

        //  TODO: REMOVE
        /// Clone board instance
        member this.Clone() =
            let clonedTileList = this.TileList               // no side effects because containing object are immutable
            let clonedPlayers = this.Players
            let clonedTurnOrder = this.TurnOrder
            {this with TileList = clonedTileList; Players = clonedPlayers; TurnOrder = clonedTurnOrder }


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
                FortressesPerPlayer = board.FortressesPerPlayer;
                TileList            = dictionary;
                Players             = List.empty<PlayerInfo>;
                CurrentTurn         = 0;
                TurnOrder           = [TileType.yellow];   //  has the first move
                GameOver            = false;
            }

