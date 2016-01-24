namespace GameUI.Suave

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Server

type EndPoint =
    | [<EndPoint "GET /">] Home
    | [<EndPoint "GET /about">] About


module Templating =
    open System.Web
    open WebSharper.UI.Next.Html


    type Page =
        {
            Title : string
            MenuBar : list<Elt>
            Body : list<Elt>
        }

    type MainTemplate = Templating.Template<"Main.html">


    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar ctx endpoint =
        let ( => ) txt act =
             liAttr [if endpoint = act then yield attr.``class`` "active"] [
                aAttr [attr.href (ctx.Link act)] [text txt]
             ]
        [
            li ["Home" => EndPoint.Home]
            li ["About" => EndPoint.About]
        ] |> List.map(fun e -> e :> Doc)

    let Main ctx endpoint title body =
        let doc = 
            MainTemplate.Doc(
                title = title,
                menubar = MenuBar ctx endpoint,
                body = body)
        Content.Page(doc)

module Site =

    let HomePage ctx =
        Templating.Main ctx EndPoint.Home "Home" [
            h1 [text "Say Hi to the server!"]
            div [client <@ Client.Main() @>]
        ]

    let AboutPage ctx =
        Templating.Main ctx EndPoint.About "About" [
            h1 [text "About"]
            p [text "This is a template self-hosted WebSharper client-server application."]
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx action ->
            match action with
            | Home -> HomePage ctx
            | About -> AboutPage ctx
        )


module SelfHostedServer =

    open global.Owin
    open Microsoft.Owin.Hosting
    open Microsoft.Owin.StaticFiles
    open Microsoft.Owin.FileSystems
    open WebSharper.Owin

    [<EntryPoint>]
    let Main = function
        | [| rootDirectory; url |] ->
            use server = WebApp.Start(url, fun appB ->
                appB.UseStaticFiles(
                        StaticFileOptions(
                            FileSystem = PhysicalFileSystem(rootDirectory)))
                    .UseSitelet(rootDirectory, Site.Main)
                |> ignore)
            stdout.WriteLine("Serving {0}", url)
            stdin.ReadLine() |> ignore
            0
        | _ ->
            eprintfn "Usage: GameUI.Suave ROOT_DIRECTORY URL"
            1
