﻿namespace GameEngine.FSharp

open System
open NLog
open GameEngine.Common

type AIEvaluation = {
    Choice : int;
    Value  : float;
}

type AI = {
        GameId: Guid;
    }

    with
        // =====================    Fields      =====================
        static member private maxTime = 3.0;
        static member private logger = LogManager.GetLogger("debug")
        static member private random = new Random()

        member private this.strategyList = [this.borderStrategy; this.gameOverStrategy; this.pointCountStrategy; this.killPlayerTurnStrategy]


        // =====================    Private members      =====================


        /// Determine choice to make
        member this.DetermineChoice (gameData : Game) (player : Guid) (possibleChoices : int list) = 
            AI.logger.Debug("DetermineChoice")

            let  playerColor = gameData.GetPlayerColor player

            AI.logger.Debug("possible choices: {0}", possibleChoices.Length)

            let valuedChoices = possibleChoices |> Seq.map(fun choice -> this.EvaluateStrategies gameData player choice)
            let maxValue = valuedChoices |> Seq.map(fun choice -> choice.Value) |> Seq.max
            let maxValuedChoices = Array.ofSeq(valuedChoices |> Seq.filter(fun elm -> elm.Value = maxValue))

            let choiceIndex = AI.random.Next(Array.length maxValuedChoices)
            let choice = Array.get maxValuedChoices choiceIndex
            AI.logger.Debug("DetermineChoice, choice for player {0} is: {1}", playerColor, choice.Choice)
            choice.Choice

        
        /// Evaluate strategies per choice
        member private this.EvaluateStrategies sourceGameData player choice =
            let divisor = this.strategyList.Length

            let afterChoiceGameData = sourceGameData.ChooseTurn(player, choice, false)

            let sumEvaluationPoints = this.strategyList 
                                        |> Seq.map(fun checkStrategy -> checkStrategy(sourceGameData, afterChoiceGameData, player, choice))
                                        |> Seq.map(fun evaluation -> evaluation.Value)
                                        |> Seq.sum
            { Choice = choice; Value = sumEvaluationPoints / float(divisor) }


        // =====================    AI non-fortress strategies to be checked for each possible move      =====================

        /// Border strategy, a tile at the board border is a better choice than a tile in the middle of the board
        member private this.borderStrategy(sourceGameData : Game, afterChoiceGameData : Game, player : Guid, choice : int) : AIEvaluation =
            let neighbourCount(gameData : Game, tile) =
                let eval = gameData.Board.TileList.[tile]
                eval.NeighbourCount
            let neighbourCount = neighbourCount(sourceGameData, choice)
            { Choice = choice; Value = float( (3 - neighbourCount) / 6 )}


        /// Game over strategy, if the choice invokes game over and I won the game, it is a good choice
        member private this.gameOverStrategy(sourceGameData : Game, afterChoiceGameData : Game, player : Guid, choice : int) : AIEvaluation =
            let didIWin playerColor gameStats = 
                match playerColor with
                    |   TileType.yellow ->  gameStats.YellowCount > gameStats.BlueCount && 
                                            gameStats.YellowCount > gameStats.RedCount
                    |   TileType.red    ->  gameStats.RedCount > gameStats.BlueCount && 
                                            gameStats.RedCount > gameStats.YellowCount
                    |   TileType.blue   ->  gameStats.BlueCount > gameStats.RedCount && 
                                            gameStats.BlueCount > gameStats.YellowCount
                    |   _   -> failwith "current color is not a real player color.."

            let highValue = { Choice = choice; Value = 1.0 }
            let neutralValue = { Choice = choice; Value = -1.0 }
            let lowValue = { Choice = choice; Value = -1.0 }

            if afterChoiceGameData.GameOver then
                let gameStats = afterChoiceGameData.GetGameStats()
                let playerColor = sourceGameData.GetPlayerColor(player) // player color from initial situation
                if didIWin playerColor gameStats then
                    highValue
                else
                    lowValue
            else    // no game over, not interesting for this strategy
                neutralValue


        /// Point count strategy, a choice that wins more points is better than a a choice which wins lesser points
        member private this.pointCountStrategy(sourceGameData : Game,  afterChoiceGameData : Game, player : Guid, choice : int) : AIEvaluation =
            let countPoints gameData playerColor =
                gameData.Board.TileList
                    |> Map.toSeq
                    |> Seq.map(fun (a, b) -> b)
                    |> Seq.filter(fun e -> e.TileType = playerColor)
                    |> Seq.map(fun e -> e.TileValue)
                    |> Seq.sum

            let playerColor = sourceGameData.GetPlayerColor(player)

            let preChoicePoints = countPoints sourceGameData playerColor
            let afterChoicePoints = countPoints afterChoiceGameData playerColor
            //  the more the points increase, the higher the percentage
            { Choice = choice ; Value = float((afterChoicePoints - preChoicePoints) / afterChoicePoints)}


        /// Kill player turn strategy, a choice that kills the turn of the next subsequent player is better than a choice which doesn't kill a player's turn.
        /// A choice which kills two player's turns, such that it is my turn again, is an excellent choice.
        member private this.killPlayerTurnStrategy(sourceGameData : Game, afterChoiceGameData : Game, player : Guid, choice : int) : AIEvaluation =
            let whoIsAfterMe gameData = 
                let newTurn = (gameData.CurrentTurn + 1) % (List.length gameData.TurnOrder)
                gameData.TurnOrder.[newTurn]
            
            let currentPlayer = sourceGameData.GetCurrentTurn()
            let playerAfterMe = whoIsAfterMe sourceGameData
            
            let afterChoicePlayer = afterChoiceGameData.GetCurrentTurn()


            if afterChoicePlayer = currentPlayer then       
                { Choice = choice; Value = 1.0 }            //  killed 2 turns, it is my turn again, excellent choice
            elif afterChoicePlayer <> playerAfterMe then    
                { Choice = choice; Value = 0.5 }            //  killed 1 turn, good choice
            else
                { Choice = choice; Value = 0.0 }            //  killed no choice, not interesting choice for this strategy


        // =====================    AI fortress strategies to be checked for each possible move      =====================
        
        /// The more clean neighbours, the better this move is for a fortress
//        member private this.cleanNeighbourStrategy(sourceGameData : Game, afterChoiceGameData : Game, playerId : Guid, choice : int) : AIEvaluation =
//            let neighboursList = afterChoiceGameData.Board.GetNeighbours choice
//            let fieldNeighbourList = neighboursList |> afterChoiceGameData.Board.GetNeighbours



        // =====================    Public members      =====================
//        member this.aiStart() =
//            Async.Start(this.aithread)


        // =====================    Static members      =====================

        static member Create(gameId:Guid) = 
            { GameId = gameId }

