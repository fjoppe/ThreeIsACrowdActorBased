namespace WebSharper.Jatek

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html


[<JavaScript; Sealed>]
type Scene  =
    member Visual : Doc with get


[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Scene =
    val Create : Artefact list -> Scene

