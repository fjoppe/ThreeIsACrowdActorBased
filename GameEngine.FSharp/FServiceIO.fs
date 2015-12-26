namespace GameEngine.FSharp


open System
open System.Web.Hosting
open System.Configuration
open System.IO
open System.Threading
open System.Runtime.Serialization
open System.ServiceModel.Activation
open NLog;


type FServiceIO = 
    static member private logger = LogManager.GetLogger("debug")
    /// Locks the game with the specified gameId
    static member lockGame(gameId : Guid) =
        let lockFileName(gameId : Guid) =
            HostingEnvironment.MapPath(String.Format("{1}/{0}.lck", gameId, ConfigurationManager.AppSettings.["DataFolder"]))
        let rnd = new Random()
        let lockName = lockFileName(gameId)
        try
            let lockFile = File.Open(lockName, FileMode.Create)
            {
                new IDisposable with
                    member this.Dispose() =
                        lockFile.Close()
            }
        with
            | _ -> Thread.Sleep(TimeSpan.FromMilliseconds(rnd.NextDouble()*0.2))
                   FServiceIO.lockGame(gameId)    // retry lock


    /// Retrieves the filename for the game data
    static member Filename(gameId:Guid) =
        HostingEnvironment.MapPath(String.Format("/App_Data/{0}.xml", gameId))


    /// Load level data
    static member LoadLevel =
        use loadFile = File.Open(ConfigurationManager.AppSettings.["LevelData"], FileMode.Open)
        let formatter = new DataContractSerializer(typeof<BoardSerializable>);
        let boardData = formatter.ReadObject(loadFile);
        boardData :?> BoardSerializable


    /// Create game data
    static member CreateGameData(gameId: Guid, gameData) =
        if File.Exists(FServiceIO.Filename(gameId)) then raise (Exception("Game data is already created"))
        FServiceIO.SaveGameData(gameId, gameData)


    /// Save game data
    static member SaveGameData(gameId:Guid, gameData) =
        use saveFile = File.Open(FServiceIO.Filename(gameId), FileMode.Create)
        let formatter = new DataContractSerializer(typeof<Game>)
        formatter.WriteObject(saveFile, gameData);


    /// Load game data
    static member LoadGameData(gameId:Guid) =
        use loadFile = File.Open(FServiceIO.Filename(gameId), FileMode.Open)
        let formatter = new DataContractSerializer(typeof<Game>);
        let gameData = formatter.ReadObject(loadFile)
        if gameData = null then
            Option.None
        else
            Option.Some(gameData :?> Game)

