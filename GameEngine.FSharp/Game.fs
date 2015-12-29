namespace GameEngine.FSharp


open System
open System.Runtime.Serialization
open System.Collections.Generic
open NLog;


[<DataContract>]
type Game =
    {
        [<field: DataMember(Name="Id") >]
        Id      : Guid;

        [<field: DataMember(Name="Board") >]
        Board   : Board;

        [<field: DataMember(Name="AIProcessing") >]
        AIProcessing : bool;
    }
    with
        //  member variables
        static member logger = LogManager.GetLogger("debug"); 


        member private this.GetPlayerColorAndCheckItIsCurrentPlayer(playerId : Guid) =
            let playerColor = this.Board.GetPlayerColor(playerId) 
            let currentColor = this.GetCurrentTurn()
            if playerColor = currentColor then
                playerColor
            else
                Game.logger.Debug("playerColor: {0}, currentColor: {1}", playerColor, currentColor)
                raise(Exception("It is not your turn!"))
        

        //  =====================  Public members          =====================
        

        /// Starts a new game
        static member StartGame board id =
            { Board = Board.ConvertList(board); Id = id; AIProcessing = false}


        /// Let a player join a game
        member this.JoinGame (playerId : Guid) =
            { this with Board = this.Board.RegisterPlayer(playerId, this.Board.FortressesPerPlayer)}


        /// Retrieve color of the player for the current turn
        member this.GetCurrentTurn() =
            this.Board.GetCurrentTurn()


        /// Retrieve possible moves for the current turn
        member this.GetPossibleMoves(playerId : Guid) = 
            Game.logger.Debug("GetPossibleMoves({0})", playerId)
            let playerColor = this.GetPlayerColorAndCheckItIsCurrentPlayer(playerId)
            let candidates = this.Board.FindChoiceCandidates(playerColor)
            new List<_>(candidates)


        /// Choose normal turn
        member this.ChooseTurn(playerId : Guid, id) =
            let playerColor = this.GetPlayerColorAndCheckItIsCurrentPlayer(playerId)
            let newBoard = this.Board.ChooseTurn(playerColor, id, false)
            { this with Board = newBoard }


        /// Choose fortressed turn
        member this.ChooseFortressedTurn(playerId : Guid, id) = 
            let playerColor = this.GetPlayerColorAndCheckItIsCurrentPlayer(playerId)
            let newBoard = this.Board.ChooseTurn(playerColor, id, true)
            { this with Board = newBoard }


        /// Retrieve the board data. Only use this function for game initialization - this provides initial data for the client.
        member this.RetrieveBoardData() =
            this.Board.RetrieveBoardData()


        /// Retrieve the current board state. Use this function for state updates - provides current state data for the client
        member this.GetBoardState() =
            this.Board.GetBoardState()


        /// Retrieve game statistics
        member this.GetGameStats() =
            this.Board.GetStatistics()


        /// Retrieve the color for the requested player
        member this.WhatIsMyColor(playerId : Guid) =
            this.Board.GetPlayerColor(playerId)


        /// Retrieve current status for player
        member this.WhatIsMyStatus(playerId : Guid) =
            let (status, board) = this.Board.GetStatus(playerId)
            (status, {this with Board = board})

        /// Clone this instance
        member this.Clone() =
            { this with Board = this.Board.Clone()}

        /// Get the player's color
        member this.GetPlayerColor(playerId:Guid) =
            this.Board.GetPlayerColor(playerId)

        /// Get player info by player id
        member this.GetPlayerInfo(playerId : Guid) =
            this.Board.GetPlayerInformation(playerId)

        /// Get player info by tile color
        member this.GetPlayerInfo(tileColor : TileType) =
            this.Board.GetPlayerInformation(tileColor)

        member this.SetTurn()=
            { this with Board = this.Board.SetTurn()}

        member this.CurrentTurnIsAI() =
            if this.Board.GameOver then
                false
            else
                let tileColor = this.GetCurrentTurn()
                let playerInfo = this.GetPlayerInfo(tileColor)
                if playerInfo.IsSome then
                    let playerInfo = playerInfo.Value
                    if playerInfo.PlayerType = PlayerType.Computer then true
                    else false
                else
                    raise(Exception("Unknown player"))

