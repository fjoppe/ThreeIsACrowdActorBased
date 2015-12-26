namespace WebSharper.Jatek

//  generate signature file for everything: Project properties -> Build -> Other flags = "--sig:$(OutputPath)\allsigs.fsi"

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript; Sealed>]
type Visual = 
    class
        inherit Artefact
    end

[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Visual =
    val Create : string -> int -> int -> int -> int -> Visual


