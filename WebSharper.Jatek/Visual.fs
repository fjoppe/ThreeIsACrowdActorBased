namespace WebSharper.Jatek

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Notation
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript; Sealed>]
type Visual(sourceUrl:string, x:int, y:int, width:int, height:int) = 
    inherit Artefact() 
    member private this.SourceUrl = sourceUrl
    member private this.X = Var.Create x
    member private this.Y = Var.Create y
    member private this.Width = Var.Create width
    member private this.Height = Var.Create height
    
    override this.Visual
        with get() =
            let styleView = 
                View.Const(fun w h x y -> sprintf "position: absolute; width: %dpx; height: %dpx; background-color: transparent; left: %dpx; top: %dpx" w h x y) 
                <*> this.Width.View
                <*> this.Height.View 
                <*> this.X.View 
                <*> this.Y.View

            let ctl = Doc.EmbedView(styleView |> View.Map(fun style -> divAttr [attr.style style] [imgAttr [attr.src this.SourceUrl][]]))
            ctl

[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Visual =
    let Create source x y w h = Visual(source, x, y, w, h)

