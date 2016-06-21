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
    }
    with
        static member Empty =
            let emptyFun = fun (el:Dom.Element) (ev:Dom.MouseEvent) -> ()
            { MouseEnter = emptyFun; MouseLeave = emptyFun; MouseOver = emptyFun}

[<JavaScript; Sealed>]
type Visual(sourceUrl:string, x:int, y:int, width:int, height:int, events: MouseEvents) = 
    inherit Artefact() 
    member internal this.SourceUrl = sourceUrl
    member internal this.X = Var.Create x
    member internal this.Y = Var.Create y
    member internal this.Width = Var.Create width
    member internal this.Height = Var.Create height
    member internal this.Events = events
    member private this.Hidden = Var.Create false

    member this.Left
        with get() = this.X.Value
        and  set v = Var.Set this.X v

    member this.Top
        with get() = this.Y.Value
        and  set v = Var.Set this.Y v

    member this.Visible 
        with get() = not(this.Hidden.Value)
        and  set show = Var.Set (this.Hidden) (not(show))

    internal new (sourceUrl:string, x:int, y:int, width:int, height:int) = Visual(sourceUrl,x,y,width,height,MouseEvents.Empty)

    member internal this.SetMouseEvents events = Visual(sourceUrl,x,y,width,height,events)

    override this.Visual
        with get() =
            let styleView = 
                View.Const(fun w h x y -> sprintf "position: absolute; width: %dpx; height: %dpx; background-color: transparent; left: %dpx; top: %dpx" w h x y) 
                <*> this.Width.View
                <*> this.Height.View 
                <*> this.X.View 
                <*> this.Y.View

            divAttr [attr.styleDyn styleView][
                imgAttr [
                    attr.src this.SourceUrl
                    attr.styleDyn (View.Map(fun b -> if b then "display: none" else "display: inherit") this.Hidden.View)
                    on.mouseEnter (events.MouseEnter)
                    on.mouseLeave (events.MouseLeave)
                    on.mouseMove  (events.MouseOver)
                    ][]
                ] :> Doc


[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Visual =
    let Create source x y w h = Visual(source, x, y, w, h)

    let MouseEnter (func: MouseEvent) (visual:Visual) = visual.SetMouseEvents ({visual.Events with MouseEnter = func})
    let MouseLeave (func: MouseEvent) (visual:Visual) = visual.SetMouseEvents ({visual.Events with MouseLeave = func})
    let MouseOver  (func: MouseEvent) (visual:Visual) = visual.SetMouseEvents ({visual.Events with MouseOver = func})

