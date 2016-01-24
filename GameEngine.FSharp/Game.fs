namespace GameEngine.FSharp


open System
open System.Runtime.Serialization
open System.Collections.Generic
open NLog;
open GameEngine.Common

/// Player information
[<DataContract>]
type PlayerInfo =
    {
        [<field: DataMember(Name="PlayerId") >]
        Player      : Player;

        [<field: DataMember(Name="Color") >]
        Color       : TileType;

        [<field: DataMember(Name="Fortresses") >]
        Fortresses  : int;

    }
    with
         //  member variables
        static member private logger = LogManager.GetLogger("debug"); 

       
        //  =====================  Public Instance members          =====================

        /// Consume fortress
        member this.ConsumeFortress(consume) =
            if consume then
                if this.Fortresses = 0 then
                        let message = String.Format("No more fortresses: {0}", this.Player)
                        PlayerInfo.logger.Error(message)
                        failwith message
                else
                    { this with Fortresses = this.Fortresses - 1 }
            else
                this


[<DataContract>]
type Game =
    {
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

        [<field: DataMember(Name="Board") >]
        Board   : Board;
    }
    with
        //  member variables
        static member logger = LogManager.GetLogger("debug"); 


        member private this.GetPlayerColorAndCheckItIsCurrentPlayer(playerId : Guid) =
            let playerColor = this.GetPlayerColor(playerId) 
            let currentColor = this.GetCurrentTurn()
            if playerColor = currentColor then
                playerColor
            else
                Game.logger.Debug("playerColor: {0}, currentColor: {1}", playerColor, currentColor)
                raise(Exception("It is not your turn!"))

        member private this.GetPlayerColorAndCheckItIsCurrentPlayer(player : Player) =
            let playerColor = this.GetPlayerColor(player) 
            let currentColor = this.GetCurrentTurn()
            if playerColor = currentColor then
                playerColor
            else
                Game.logger.Debug("playerColor: {0}, currentColor: {1}", playerColor, currentColor)
                raise(Exception("It is not your turn!"))
        

        //  =====================  Public members          =====================
        

        /// Starts a new game
        static member LoadGame (board : BoardSerializable) =
            { 
                FortressesPerPlayer = board.FortressesPerPlayer;
                Players             = List.empty<PlayerInfo>;
                CurrentTurn         = 0;
                TurnOrder           = [TileType.yellow];   //  has the first move
                GameOver            = false;            
                Board = Board.ConvertList(board)
            }


        ///     Get color for a new playerInfo, before it is added to the list
        member private this.GetColor(currentPlayers : PlayerInfo list) =
            match (List.length currentPlayers) with
            |   0   ->  TileType.yellow
            |   1   ->  TileType.blue
            |   2   ->  TileType.red
            |   _   ->  failwith "exceeds maximum index"


        /// Let a player join a game
        member this.JoinGame (player : Player) =
            let playersNew = this.Players
            let newPlayer = 
                {
                    Player      = player; 
                    Color       = this.GetColor(playersNew);
                    Fortresses  = this.FortressesPerPlayer; 
                }
            let playersNew = List.append playersNew [newPlayer]
            { this with Players = playersNew; }


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


        /// Retrieve color of the player for the current turn
        member this.GetCurrentTurn() =
            if not this.GameOver then
                this.TurnOrder.[this.CurrentTurn]
            else
                TileType.none


        /// Retrieve possible moves for the current turn
        member this.GetPossibleMoves(player : Player) = 
            Game.logger.Debug("GetPossibleMoves({0})", player)
            let playerColor = this.GetPlayerColorAndCheckItIsCurrentPlayer(player)
            let candidates = this.Board.FindChoiceCandidates(playerColor)
            Seq.toList candidates

        /// Sends BoardHasChanged message to all players
        member this.NotifyPlayersBoardHasChanged() =
            this.Players |> List.iter
                (fun p -> 
                    let boardState = this.GetBoardState()
                    Player.SendMessage p.Player (PlayerStatus.BoardHasChanged(boardState))
                )

        /// Retrieve the turn that is next
        member private this.GetNextCurrentTurn() =
            let currentTurn = this.CurrentTurn + 1
            if currentTurn = 3 then
                { this with CurrentTurn = 0 }
            else 
                { this with CurrentTurn = currentTurn }


        /// Initialize the next turn
        member private this.InitNextTurn() =
            let rec initNextTurnRec(instance:Game, skips) = 
                let nextTurn = instance.GetNextCurrentTurn()
                let possibilities = nextTurn.Board.FindChoiceCandidates(nextTurn.GetCurrentTurn())
                if Seq.isEmpty possibilities then 
                    let nextTurnColor = nextTurn.GetCurrentTurn()
                    let skipPlayer = nextTurn.GetPlayerInfo(nextTurnColor).Player
                    Player.SendMessage skipPlayer PlayerStatus.NoMoves
                    if skips < 3 then
                        initNextTurnRec(nextTurn, skips+1)
                    else
                        nextTurn.Players |> List.iter(fun elm -> Player.SendMessage elm.Player PlayerStatus.GameOver) 
                        nextTurn
                else
                    nextTurn.SetTurn()
                    nextTurn
            initNextTurnRec(this, 0)


        /// Invoke player move, turn tiles, consume fortress if necesary
        member private this.InvokePlayerMove(color, id, fortress) =
            let (newBoard, colorCapture) = this.Board.TurnTiles(color, id, fortress)
            let newGame = this.UpdateTurnOrder(colorCapture)
            let newGame = newGame.UpdatePlayer(this.GetPlayerInfo(color).ConsumeFortress(fortress))
            { newGame with Board = newBoard }


        /// Generic make choice, realises player's choice
        member private this.MakeChoice playerColor id fortress =
            this.Players |> List.iter(fun p -> Player.SendMessage (p.Player) (PlayerStatus.PlayerMadeChoice(playerColor, id, fortress)))
            let newGame = this.InvokePlayerMove(playerColor, id, fortress)
            newGame.NotifyPlayersBoardHasChanged()
            let newGame = newGame.InitNextTurn()
            newGame


        /// Choose turn by player
        member this.ChooseTurn(player : Player, id, fortress) =
            let playerColor = this.GetPlayerColorAndCheckItIsCurrentPlayer(player)
            this.MakeChoice playerColor id fortress 


        /// Choose turn by Guid
        member this.ChooseTurn(playerId : Guid, id, fortress) = 
            let playerColor = this.GetPlayerColorAndCheckItIsCurrentPlayer(playerId)
            this.MakeChoice playerColor id fortress 


        /// Retrieve the board data. Only use this function for game initialization - this provides initial data for the client.
        member this.RetrieveBoardData() =
            {
                FortressesPerPlayer = this.FortressesPerPlayer
                ActiveTileList = this.Board.RetrieveBoardData()
            }


        /// Retrieve the current board state. Use this function for state updates - provides current state data for the client
        member this.GetBoardState() =
            this.Board.GetBoardState()


        /// Retrieve game statistics
        member this.GetGameStats() =
            let fortressCount color =  this.Players |> List.find(fun elm -> elm.Color = color) |> fun e -> e.Fortresses
            let countColor color = this.Board.GetTileList() |> Map.Values |> Seq.filter(fun elm -> elm.TileType = color) |> Seq.length;
            {
                RedCount        = countColor TileType.red;
                BlueCount       = countColor TileType.blue;
                YellowCount     = countColor TileType.yellow;
                YellowFortress  = fortressCount TileType.yellow; 
                BlueFortress    = fortressCount TileType.blue;
                RedFortress     = fortressCount TileType.red;
            }


        /// Retrieve the player's color by Player
        member this.GetPlayerColor(player:Player) =
            if List.isEmpty this.Players then TileType.none
            else
                this.GetPlayerInfo(player).Color

        /// Retrieve the player's color by Guid
        member this.GetPlayerColor(playerId:Guid) =
            if List.isEmpty this.Players then TileType.none
            else
                this.GetPlayerInfo(playerId).Color


        /// Generic GetPlayerInfo processor
        member private this.ProcessPlayerInfo<'a> (player:'a) = function
            | Some(found) -> found
            | None        ->
                    Board.logger.Error("Don't know this player: {0}", player);
                    raise(Exception("Unknown player"))

        /// Get player info by player
        member this.GetPlayerInfo(player : Player) =
            let found = this.Players |> Seq.tryFind(fun elm -> elm.Player = player)
            this.ProcessPlayerInfo player found

        /// Get player info by playerId
        member this.GetPlayerInfo(playerId : Guid) =
            let found = this.Players |> Seq.tryFind(fun elm -> elm.Player.IdentifiesWith playerId)
            this.ProcessPlayerInfo playerId found

        /// Get player info by player color
        member this.GetPlayerInfo(color:TileType) =
            let found = this.Players |> Seq.tryFind(fun elm -> elm.Color = color)
            this.ProcessPlayerInfo color found


        /// Update player
        member private this.UpdatePlayer(playerInfo) =
            let newPlayers = playerInfo :: (this.Players |> List.filter(fun e -> e.Player <> playerInfo.Player))
            { this with Players = newPlayers }


        /// Inform all players that the game has started
        member this.InformGameStartedToPlayers() =
            this.Players |> List.iter(fun p -> Player.SendMessage (p.Player) (PlayerStatus.GameStarted(p.Color)))
            this

        /// Send "It is your turn" to the player who has the current move
        member this.SetTurn() =
            let color = this.GetCurrentTurn()
            let currentPlayer =  this.GetPlayerInfo(color)
            Player.SendMessage (currentPlayer.Player) (PlayerStatus.ItsMyTurn(this.Board.FindChoiceCandidates(color) |> Seq.toList))

