namespace GameUI.Suave.ThreeIsACrowd

open System.IO
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/about">] About

module Templating =
    open WebSharper.UI.Next.Html

    type MainTemplate = Templating.Template<"Main.html">

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Context<EndPoint>) endpoint : Doc list =
        let ( => ) txt act =
             liAttr [if endpoint = act then yield attr.``class`` "active"] [
                aAttr [attr.href (ctx.Link act)] [text txt]
             ]
        [
            li ["Home" => EndPoint.Home]
            li ["About" => EndPoint.About]
        ]

    let Main ctx action title body =
        Content.Page(
            MainTemplate.Doc(
                title = title,
                menubar = MenuBar ctx action,
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

    let AboutPage ctx =
        Templating.Main ctx EndPoint.About "About" [
            h1 [text "About"]
            p [text "This is a template WebSharper client-server application."]
        ]

    let Main =
        WebSharper.Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
            | EndPoint.About -> AboutPage ctx
        )

    open Suave
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open System.IO
    open Suave.Web

    do 
        let myHomeFolder = Path.Combine(System.Environment.CurrentDirectory) 
        printf "%s" myHomeFolder 

        let cfg = {  defaultConfig with homeFolder = Some(myHomeFolder)}

        let routing = 
            choose [
                path "/App_Themes" >=> OK "test" ; Files.browseHome
                (WebSharper.Suave.WebSharperAdapter.ToWebPart Main)
                RequestErrors.NOT_FOUND "Page not found."
            ]

        startWebServer cfg routing // (WebSharperAdapter.ToWebPart Main)
