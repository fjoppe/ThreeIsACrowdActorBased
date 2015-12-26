namespace WebSharper.Jatek

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript; AbstractClass>]
type Artefact() =
    abstract member Visual : Doc with get

