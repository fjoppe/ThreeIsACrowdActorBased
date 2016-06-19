namespace TrafficSim

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

type EndPoint =
    | [<EndPoint "/">] Home

module Templating =
    open WebSharper.UI.Next.Html

    type MainTemplate = Templating.Template<"Main.html">

    let Main ctx action title body =
        Content.Page(
            MainTemplate.Doc(
                title = title,
                body = body
            )
        )

module Site =
    open WebSharper.UI.Next.Html

    let HomePage ctx =
        Templating.Main ctx EndPoint.Home "Home" [
            h1 [text "Say Hi to the server!"]
            div [client <@ VisualTest.Client.Main() @>]
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
        )

    //[<Websocket>]

