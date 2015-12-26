namespace WebSharper.Jatek

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Notation
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript; Sealed>]
type Container(x:int, y:int, width:int, height:int, artefacts : Artefact list) = 
    inherit Artefact() 
    member private this.X = Var.Create x
    member private this.Y = Var.Create y
    member private this.Width = Var.Create width
    member private this.Height = Var.Create height
    member private this.Artefacts = artefacts
    
    override this.Visual
        with get() =
            let styleView = 
                View.Const(fun w h x y -> sprintf "position: absolute; width: %dpx; height: %dpx; background-color: lightgray; left: %dpx; top: %dpx" w h x y) 
                <*> this.Width.View
                <*> this.Height.View 
                <*> this.X.View 
                <*> this.Y.View

            let artefactList = Doc.Concat(this.Artefacts |> List.map(fun a -> a.Visual))

            let ctl = Doc.EmbedView(styleView |> View.Map(fun style -> divAttr [attr.style style] [artefactList]))
            ctl

[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Container =
    let Create x y w h a = Container(x, y, w, h, a)


