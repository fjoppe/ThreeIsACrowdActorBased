namespace GameEngine.FSharp


open System
open System.Configuration
open System.IO
open System.Runtime.Serialization

open GameEngine.Common

type FServiceIO = 

    /// Load level data
    static member LoadLevel =
        use loadFile = File.Open(ConfigurationManager.AppSettings.["LevelData"], FileMode.Open)
        let formatter = new DataContractSerializer(typeof<BoardSerializable>);
        let boardData = formatter.ReadObject(loadFile);
        boardData :?> BoardSerializable

