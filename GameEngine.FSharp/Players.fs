#light 

namespace GameBoard

open System
open System.Runtime.Serialization
open System.Collections.Generic
open TileType
open NLog;


/// Type of player
type PlayerType = 
    | Human
    | Computer


/// Player statistics
type PlayerStats = {
    YellowFortress  : int;
    RedFortress     : int;
    BlueFortress    : int;
}

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
        member this.ConsumeFortress() =
            if this.Fortresses = 0 then
                    let message = String.Format("No more fortresses: {0}", this.PlayerId)
                    PlayerInfo.logger.Error(message)
                    failwith message
            else
                { this with Fortresses = this.Fortresses - 1 }


[<DataContract>]
type Players = 
    {
        [<field: DataMember(Name="Players") >]
        Players : List<PlayerInfo>;

        [<field: DataMember(Name="CurrentTurn") >]
        CurrentTurn : int

        [<field: DataMember(Name="TurnOrder") >]
        TurnOrder : List<TileType>
    }
    with
        static member private numberOfAI = 2
        
        //  member variables
        static member private logger = LogManager.GetLogger("debug"); 

        //  =====================  Private Instance members          =====================

        member private this.GetPlayerInfoById(playerId : Guid, expression) =
            let found = this.Players |> Seq.tryFind(fun elm -> elm.PlayerId = playerId)
            if found.IsSome then expression(found.Value)
                else
                    Players.logger.Error("Don't know this player: {0}", playerId);
                    failwith "Unknown player"

        ///<summary>
        ///     Get color for a new playerInfo, before it is added to the list
        ///</summary>
        member private this.GetColor(currentPlayers : List<PlayerInfo>) =
            match currentPlayers.Count with
            |   0   ->  TileType.yellow
            |   1   ->  TileType.blue
            |   2   ->  TileType.red
            |   _   ->  failwith "exceeds maximum index"


        ///<summary>
        ///     Fill the remaining seats of players with AI players
        ///</summary>
        member private this.FillWithAIPlayers(currentPlayers : List<PlayerInfo>, fortressesPerPlayer) =
            Players.logger.Debug(String.Format("FillWithAIPlayers: {0}", Players.numberOfAI));
            let PlayersNew = new List<PlayerInfo>(currentPlayers)
            let newPlayer = 
                {
                    PlayerId    = Guid.NewGuid() ; 
                    Color       = this.GetColor(PlayersNew); 
                    Fortresses  = fortressesPerPlayer; 
                    PlayerType  = PlayerType.Computer;
                    Status      = new Queue<_,_>();
                }
            PlayersNew.Add(newPlayer)
            if PlayersNew.Count < 3 then this.FillWithAIPlayers(PlayersNew, fortressesPerPlayer)
            else PlayersNew


        //  =====================  Public Instance members          =====================


        ///<summary>
        ///     Register a player
        ///</summary>
        member this.RegisterPlayer(playerId:Guid, fortressesPerPlayer) =
            let PlayersNew = new List<PlayerInfo>(this.Players)
            let newPlayer = 
                {
                    PlayerId = Guid.NewGuid() ; 
                    Color= this.GetColor(PlayersNew);
                    Fortresses = fortressesPerPlayer; 
                    PlayerType = PlayerType.Computer
                    Status      = new Queue<_,_>();
                }
            PlayersNew.Add(newPlayer)

            if PlayersNew.Count - Players.numberOfAI = 3 then
                { 
                    this with Players = this.FillWithAIPlayers(PlayersNew, fortressesPerPlayer);
                }
            else
                {
                    this with Players = PlayersNew;
                }


        ///<summary>
        ///     Retrieve color of the player, when available
        ///</summary>
        /// <returns>
        ///     When available, the player color
        ///     When unavailable the color "none"
        ///</returns>
        member this.GetPlayerColor(playerId:Guid) =
            if this.Players.Count = 0 then TileType.none
            else
                this.GetPlayerInfoById(playerId, fun pi -> pi.Color)

        ///<summary>
        ///     Queries whether the player still has fortresses left
        ///</summary>
        /// <returns>
        ///     true    -> the player still has fortresses left
        ///     false   -> the player has no fortresses left
        ///</returns>
        member this.HasFortresses(playerId:Guid) =
            this.GetPlayerInfoById(playerId, fun pi -> pi.Fortresses > 0)


        /// Gets the color for the current turn
        member this.GetCurrentTurn() = 
            this.TurnOrder.[this.CurrentTurn]


        /// Retrieve player statistics
        member this.GetStats() =
            let fortressCount color = this.Players.Find(fun elm -> elm.Color = color).Fortresses
            {
                YellowFortress  = fortressCount TileType.yellow; 
                BlueFortress    = fortressCount TileType.blue;
                RedFortress     = fortressCount TileType.red;
            }
        

        /// Retrieve current status for player
        member this.GetStatus(playerId : Guid) =
            this.GetPlayerInfoById(playerId, fun pi ->  if pi.Status.Count = 0 then PlayerStatus.none
                                                        else pi.Status.Dequeue())


        /// Create Initial state instance
        static member CreateInitial() =
            let turnOrder = new List<TileType>()
            turnOrder.Add(TileType.yellow)                  //  has the first move
            {
                Players         = new List<PlayerInfo>();
                CurrentTurn     = 0;
                TurnOrder       = turnOrder;
            }
