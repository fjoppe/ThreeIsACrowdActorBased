namespace WebSharper.Jatek

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html


[<JavaScript; Sealed>]
type Scene(artefacts: Artefact list)  =
    member this.Artefacts = artefacts
    member this.Visual
        with get() =

            let artefactList = Doc.Concat(this.Artefacts |> List.map(fun a -> a.Visual))

            let ctl = div [artefactList]
            ctl :> Doc


[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Scene =
    let Create artefacts = Scene(artefacts)

