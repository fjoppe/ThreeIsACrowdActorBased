//namespace GameEngine.FSharp
//
//open System
//open System.Runtime.Serialization
//open System.ServiceModel
//
//
//[<ServiceContract>]
//type IFSharpService =
//    [<OperationContract>]
//    /// <summary>
//    /// This creates a new identifier which may be used by a client
//    /// </summary>
//    /// <returns>A new Guid</returns>
//    abstract GetUniqueIdentifier: unit -> Guid
//
//    /// <summary>
//    /// This starts a new game from scratch.
//    /// </summary>
//    [<OperationContract>]
//    abstract StartNewGame: configuration:GameConfiguration -> unit
//
//    /// <summary>
//    /// Join a game.
//    /// </summary>
//    /// <param name="playerId">Identifies the player</param>
//    /// <returns>a Guid identifying a game</returns>
////    [<OperationContract>]
////    abstract JoinGame: gameId:Guid -> playerId:Guid -> unit
//
//
//    /// <summary>
//    /// Retrieve which color is it's turn
//    /// </summary>
//    /// <param name="gameId">Identifies the game</param>
//    /// <returns>the color of the player who's turn it is</returns>
//    [<OperationContract>]
//    abstract GetCurrentTurn: gameId:Guid -> TileType
//
//
//    /// <summary>
//    /// Retrieve possible moves for the specified game and player.
//    /// </summary>
//    /// <param name="gameId">Identifies the game</param>
//    /// <param name="playerId">Identifies the player</param>
//    /// <returns>List of tile-id's for which the player may possibly move</returns>
//    [<OperationContract>]
//    abstract GetPossibleMoves: gameId:Guid -> playerId:Guid -> System.Collections.Generic.List<int>
//
//
//    /// <summary>
//    /// Send choice for turn
//    /// </summary>
//    /// <param name="gameId">Identifies the game</param>
//    /// <param name="playerId">Identifies the player</param>
//    /// <param name="id">Identifies the chosen tile</param>
//    [<OperationContract>]
//    abstract ChooseTurn: gameId:Guid -> playerId:Guid -> id:int -> unit
//
//
//    /// <summary>
//    /// Send choice for fortressed turn
//    /// </summary>
//    /// <param name="gameId">Identifies the game</param>
//    /// <param name="playerId">Identifies the player</param>
//    /// <param name="id">Identifies the chosen tile with fotress</param>
//    [<OperationContract>]
//    abstract ChooseFortressedTurn: gameId:Guid -> playerId:Guid -> id:int -> unit
//
//
//    /// <summary>
//    /// Retrieve board data for specified game. Only used once at the beginning of the game, 
//    /// for the client to visualize and create references.
//    /// </summary>
//    /// <param name="gameId">Identifies the game</param>
//    /// <returns>Board data, including board shape, colors, tile positions and tile identifiers</returns>
//    [<OperationContract>]
//    abstract RetrieveBoardData: gameId:Guid -> BoardSerializable
//
//
//    /// <summary>
//    /// Retrieve the current board state. This function is used during the game.
//    /// </summary>
//    /// <param name="gameId">Identifies the game</param>
//    /// <returns>Returns board data, limited to which tile has which color.</returns>
//    [<OperationContract>]
//    abstract GetBoardState: gameId:Guid -> TileColor[]
//
//
//    /// <summary>
//    /// Retrieve current game statistics.
//    /// </summary>
//    /// <param name="gameId">Identifies the game</param>
//    /// <returns>Game statistics, amount of tiles per color</returns>
//    [<OperationContract>]
//    abstract GetGameStats: gameId:Guid -> GameStats
//
//
//    /// <summary>
//    /// Retrieve the color the player is playing with.
//    /// </summary>
//    /// <param name="gameId">Identifies the game</param>
//    /// <param name="playerId">Identifies the player</param>
//    /// <returns>The color the specified player is playing with, for the specified game</returns>
//    [<OperationContract>]
//    abstract WhatIsMyColor: gameId:Guid -> playerId:Guid -> TileType
//
//
//    /// <summary>
//    /// Retrieve the status for the specified player, which is actually a message:
//    /// * none      - the player should wait
//    /// * itsMyTurn - it's the player's turn
//    /// * noMoves   - no moves possible, turn is skipped
//    /// * gameOver  - the game is over (at least for this player)
//    /// </summary>
//    /// <param name="gameId">Identifies the game</param>
//    /// <param name="playerId">Identifies the player</param>
//    /// <returns></returns>
//    [<OperationContract>]
//    abstract WhatIsMyStatus: gameId:Guid -> playerId:Guid -> PlayerStatus
//
