namespace GameEngine.FSharp


open System
open System.ServiceModel.Activation
open NLog;


[<AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)>]
type FService() = 
    static member private logger = LogManager.GetLogger("debug")

    
    /// Generic function to lock, load game data and execute a function on the game data, does not save gamedata, return function result
    static member private LockLoadAndExecute(gameId, (myFunction: Game -> 'a)) =
        try
            use lock = FServiceIO.lockGame(gameId)
            let gameData = FServiceIO.LoadGameData(gameId)
            if gameData.IsSome then
                myFunction gameData.Value
            else
                FService.logger.Error("No game data available for {0}", gameId)
                raise (Exception("No game data available"))
        with
            | e -> FService.logger.Error(e)
                   reraise()


    /// Generic function to lock, load game data, execute a functions which modifies the gameData and save the new game data
    static member private Execute(gameId, (myFunction: Game -> Game)) =
        let execute(gameData : Game) = 
            let newGameData = myFunction gameData
            FServiceIO.SaveGameData(gameId, newGameData)
        FService.LockLoadAndExecute( gameId, execute)


    /// Generic function to lock, load game data, execute a funtction which does not modify the gameData
    static member private Execute(gameId, (myFunction: Game -> 'a)) =
        let execute(gameData : Game) = 
            myFunction gameData
        FService.LockLoadAndExecute(gameId, execute)


    member private this.StartAI(gameId) =
        let ai = AI.Create(gameId)
        ai.aiStart()



    member this.GetUniqueIdentifier() = 
        FService.logger.Debug("GetUniqueIdentifier")
        Guid.NewGuid()


//        member this.StartNewGame configuration =
//            try
//                FService.logger.Debug("StartNewGame")
//                use lock = FServiceIO.lockGame(configuration.GameId)
//                let levelData = FServiceIO.LoadLevel
//                let gameData = Game.StartGame levelData configuration //  start the game by init game data
//                FServiceIO.CreateGameData(configuration.GameId, gameData)
//            with
//                | e -> FService.logger.Error(e)
//                       reraise()


//        member this.JoinGame gameId playerId =
//            FService.logger.Debug("JoinGame: {0}", playerId)
//            let execute(gameData:Game) =
//                gameData.JoinGame(playerId)
//            FService.Execute(gameId, execute)


    member this.GetCurrentTurn gameId = 
        FService.logger.Debug("GetCurrentTurn")
        let execute(gameData:Game) =
            gameData.GetCurrentTurn() 
        FService.Execute(gameId, execute)


    member this.GetPossibleMoves gameId playerId =
        FService.logger.Debug("GetPossibleMoves")
        let execute(gameData:Game) =
            gameData.GetPossibleMoves(playerId)
        FService.Execute(gameId, execute)


    member this.ChooseTurn gameId playerId id =
        FService.logger.Debug("ChooseTurn")
        let execute(gameData:Game) =
            let gameData = gameData.ChooseTurn(playerId, id)
            FService.logger.Debug(String.Format("Next turn: {0}", gameData.GetCurrentTurn()))
            gameData
        FService.Execute(gameId, execute)


    member this.ChooseFortressedTurn gameId playerId id =
        FService.logger.Debug("ChooseFortressedTurn ")
        let execute(gameData:Game) =
            gameData.ChooseFortressedTurn(playerId, id)
        FService.Execute(gameId, execute)


    member this.RetrieveBoardData gameId =
        FService.logger.Debug("RetrieveBoardData")
        let execute(gameData:Game) =
            gameData.RetrieveBoardData()
        FService.Execute(gameId, execute)


    member this.GetBoardState gameId =
//            FService.logger.Debug("GetBoardState")
        let execute(gameData:Game) =
            gameData.GetBoardState()
        FService.Execute(gameId, execute)


    member this.GetGameStats gameId =
//            FService.logger.Debug("GetGameStats")
        let execute(gameData:Game) =
            gameData.GetGameStats()
        FService.Execute(gameId, execute)


    member this.WhatIsMyColor gameId playerId =
        FService.logger.Debug("WhatIsMyColor")
        let execute(gameData:Game) =
            let result = gameData.WhatIsMyColor(playerId)
            result
        FService.Execute(gameId, execute)
            

    member this.WhatIsMyStatus gameId playerId =
//            FService.logger.Debug("WhatIsMyStatus")
        let execute(gameData:Game) =
            let (status, gameData) = gameData.WhatIsMyStatus(playerId)
            if status = PlayerStatus.NoStatus && gameData.CurrentTurnIsAI() && not gameData.AIProcessing then
                FService.logger.Debug("Current Turn is AI for processing")
                this.StartAI(gameId)
                let gameDataProcessingAI = { gameData with AIProcessing = true }
                FServiceIO.SaveGameData(gameId, gameDataProcessingAI )
            else
                FServiceIO.SaveGameData(gameId, gameData)
            status
        FService.Execute(gameId, execute)

