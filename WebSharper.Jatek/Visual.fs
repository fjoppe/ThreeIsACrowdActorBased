namespace WebSharper.Jatek

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Notation
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html


[<JavaScript>]
type MouseEvent = (Dom.Element -> Dom.MouseEvent -> unit)

[<JavaScript>]
type MouseEvents = {
        MouseEnter  : MouseEvent 
        MouseLeave  : MouseEvent 
        MouseOver   : MouseEvent
        Click       : MouseEvent
    }
    with
        static member Empty =
            let emptyFun = fun (el:Dom.Element) (ev:Dom.MouseEvent) -> ()
            { MouseEnter = emptyFun; MouseLeave = emptyFun; MouseOver = emptyFun; Click = emptyFun}

[<JavaScript; Sealed>]
type Visual(sourceUrl:string, x:int, y:int, width:int, height:int) = 
    inherit Artefact() 
    let SourceUrl = sourceUrl
    let X = Var.Create x
    let Y = Var.Create y
    let Width = Var.Create width
    let Height = Var.Create height
    let events = Var.Create MouseEvents.Empty
    let Hidden = Var.Create false

    member this.Left
        with get() = X.Value
        and  set v = Var.Set X v

    member this.Top
        with get() = Y.Value
        and  set v = Var.Set Y v

    member this.Visible 
        with get() = not(Hidden.Value)
        and  set show = Var.Set (Hidden) (not(show))

    member this.Events = events

    member internal this.SetMouseEvents e = Var.Set events e

    override this.Visual
        with get() =
            let styleView = 
                View.Const(fun w h x y -> sprintf "position: absolute; width: %dpx; height: %dpx; background-color: transparent; left: %dpx; top: %dpx" w h x y) 
                <*> Width.View
                <*> Height.View 
                <*> X.View 
                <*> Y.View

            divAttr [attr.styleDyn styleView][
                imgAttr [
                    attr.src SourceUrl
                    attr.styleDyn (View.Map(fun b -> sprintf "pointer-events:visible; %s" (if b then "display: none" else "display: inherit")) Hidden.View)
                    on.mouseLeave (events.Value.MouseEnter)
                    on.mouseEnter (events.Value.MouseLeave)
                    on.mouseOver  (events.Value.MouseOver)
                    on.click      (events.Value.Click)
                    ][]
                ] :> Doc


[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Visual =
    let Create source x y w h = Visual(source, x, y, w, h)

    let MouseEnter (func: MouseEvent) (visual:Visual) = visual.SetMouseEvents ({visual.Events.Value with MouseEnter = func}); visual
    let MouseLeave (func: MouseEvent) (visual:Visual) = visual.SetMouseEvents ({visual.Events.Value with MouseLeave = func}); visual
    let MouseOver  (func: MouseEvent) (visual:Visual) = visual.SetMouseEvents ({visual.Events.Value with MouseOver = func}); visual
    let Click      (func: MouseEvent) (visual:Visual) = visual.SetMouseEvents ({visual.Events.Value with Click = func}); visual

