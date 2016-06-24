namespace WebSharper.Jatek

//  generate signature file for everything: Project properties -> Build -> Other flags = "--sig:$(OutputPath)\allsigs.fsi"

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript>]
type MouseEvent = (Dom.Element -> Dom.MouseEvent -> unit)

[<JavaScript; Sealed>]
type Visual = 
    class
        inherit Artefact
            member Visible : bool with get, set
            member Left    : int with get, set
            member Top     : int with get, set
    end

[<JavaScript; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Visual =
    val Create : string -> int -> int -> int -> int -> Visual

    val MouseEnter : MouseEvent -> Visual -> Visual
    val MouseLeave : MouseEvent -> Visual -> Visual
    val MouseOver  : MouseEvent -> Visual -> Visual
    val Click      : MouseEvent -> Visual -> Visual



