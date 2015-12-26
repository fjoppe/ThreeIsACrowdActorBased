namespace WebSharper.Jatek

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<Measure>] type perc
type FrameInfo = float<perc>

type GameLoopFunction = FrameInfo -> unit

[<Sealed>]
type Game<'T when 'T : comparison> =
    member Start  : unit -> unit
    member Stop   : unit -> unit
    member Visual : Doc with get


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Game =
    val Create<'T when 'T : comparison> : int -> int -> GameLoopFunction -> Map<'T, Scene> -> 'T -> Game<'T> 


