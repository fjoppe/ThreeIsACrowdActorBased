This was made in Visual Studio 2015 Community edition, instructions are accordingly.

How to Build and Run (version 2016-01-24)

1. Download Zip
2. Make sure the NuGet packages are restored
3. Set "GameEngine.Service" as Start project
4. In project "GameEngine.Service", edit the App.config and change the settings for:
  * nlog/logdirectory
  * appSettings/levelData (this should point to the folder "GameLevelData" found in the zip
5. Build everything


Running it:

1. Open "CallRemoteActor.fsx" in project "GameEngine.Service", in your editor
2. Open the logfile you configured in App.Config, in your editor (you can monitor activity)
3. Run the service, "GameEngine.Service"

When the service is ready with initialization (see logfile, or command window), you can run the lines from the beginning of  "CallRemoteActor.fsx" until (and including) this line:
```fsharp
registerPlayer <! GameEngine.Common.RegisterMe(ActorRefs.untyped(playerReceiver))
```

When the interactive returns a message that it is your move, you can send the choice (for level1.xml, the first choice  is always 7)

