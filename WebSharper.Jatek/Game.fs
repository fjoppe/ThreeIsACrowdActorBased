namespace WebSharper.Jatek

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
open System.Threading

[<Measure>] type perc
type FrameInfo = float<perc>

type GameLoopFunction = (FrameInfo -> unit)


[<JavaScript>]
module GameInternals =
    let rec frameRefresh updateFunction =
        async {
            do! Async.Sleep 1000
            do updateFunction (0.0<perc>)
            return! frameRefresh updateFunction
        }


[<JavaScript>]
type GameState = 
    |   Pauzed  of (GameLoopFunction)
    |   Running of (GameLoopFunction * CancellationTokenSource)


[<JavaScript; Sealed>]
type Game<'T when 'T : comparison>(width:int, height:int, gameLoop:GameLoopFunction, scenes: Map<'T, Scene>, startScene : 'T) =
    member private this.Width = width
    member private this.Height = height
    member private this.GameState = Var.Create (Pauzed(gameLoop))
    member private this.Scenes = scenes
    member private this.CurrentScene = Var.Create startScene
    member this.Start() = match this.GameState.Value with
                          | Pauzed(lf)  -> 
                                let cancellationSource = new CancellationTokenSource()
                                let cancellationToken = cancellationSource.Token
                                Async.Start ((GameInternals.frameRefresh lf), cancellationToken)
                                this.GameState.Value <- (Running(lf, cancellationSource))
                          | Running(lf, cs) ->  ()
    member this.Stop() = match this.GameState.Value with
                         | Pauzed(lf)      -> ()
                         | Running(lf, cs) -> cs.Cancel()
                                              this.GameState.Value <- Pauzed(lf)

    /// Convert to Websharper's Doc
    member this.Visual
        with get() = 
            let style = sprintf "position: relative; width: %dpx; height: %dpx; background-color: lightgray" this.Width this.Height
            let currentScene = this.Scenes.[this.CurrentScene.Value]
            divAttr [attr.style style] [currentScene.Visual] :> Doc


[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Game =
    let Create width height gameLoop scenes startScene = Game(width, height, gameLoop, scenes, startScene)

