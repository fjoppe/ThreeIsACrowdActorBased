namespace WebSharper.Jatek

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Notation
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript; Sealed>]
type Container = 
    class
        inherit Artefact
    end


[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Container =
    val Create : int -> int -> int -> int -> Artefact list -> Container


