namespace GameUI.Owin

open WebSharper.Html.Server
open WebSharper
open WebSharper.Sitelets

type EndPoint =
    | [<EndPoint "GET /">] Home
    | [<EndPoint "GET /about">] About

module Templating =
    open System.Web

    type Page =
        {
            Title : string
            MenuBar : list<Element>
            Body : list<Element>
        }

    let MainTemplate =
        Content.Template<Page>("~/Main.html")
            .With("title", fun x -> x.Title)
            .With("menubar", fun x -> x.MenuBar)
            .With("body", fun x -> x.Body)

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Context<EndPoint>) endpoint =
        let ( => ) txt act =
             LI [if endpoint = act then yield Attr.Class "active"] -< [
                A [Attr.HRef (ctx.Link act)] -< [Text txt]
             ]
        [
            LI ["Home" => EndPoint.Home]
            LI ["About" => EndPoint.About]
        ]

    let Main ctx endpoint title body : Async<Content<EndPoint>> =
        Content.WithTemplate MainTemplate
            {
                Title = title
                MenuBar = MenuBar ctx endpoint
                Body = body
            }

module Site =

    let HomePage ep ctx =
        Templating.Main ctx EndPoint.Home "Home" [
            H1 [Text "Three is a crowd - actor based"]
            Div [ClientSide <@ Client.Main ep @>]
        ]

    let AboutPage ep ctx =
        Templating.Main ctx EndPoint.About "About" [
            H1 [Text "About"]
            P [Text "This is a template self-hosted WebSharper client-server application."]
        ]

    [<Website>]
    let Main ep =
        Application.MultiPage (fun ctx action ->
            match action with
            | Home -> HomePage ep ctx
            | About -> AboutPage ep ctx
        )


module SelfHostedServer =

    open global.Owin
    open Microsoft.Owin.Hosting
    open Microsoft.Owin.StaticFiles
    open Microsoft.Owin.FileSystems
    open WebSharper.Owin
    open WebSharper.Owin.WebSocket

    [<EntryPoint>]
    let Main args =
        let rootDirectory, url =
            match args with
            | [| rootDirectory; url |] -> rootDirectory, url
            | [| url |] -> "..", url
            | [| |] -> "..", "http://localhost:9000/"
            | _ -> eprintfn "Usage: GameUI.Owin ROOT_DIRECTORY URL"; exit 1
        use server = WebApp.Start(url, fun appB ->
            let ep = Endpoint.Create(url, "/ws", JsonEncoding.Readable)
            let rootDirectory =
                    System.IO.Path.Combine(
                        System.IO.Directory.GetCurrentDirectory(),
                        rootDirectory)
            appB.UseStaticFiles(
                    StaticFileOptions(
                        FileSystem = PhysicalFileSystem(rootDirectory)))
                .UseWebSharper(
                    WebSharperOptions(
                            ServerRootDirectory = rootDirectory,
                            Sitelet = Some (Site.Main ep),
                            Debug = true)
                            .WithWebSocketServer(ep,  ThreeIsACrowd.GameEngineConnection.Start ep))
            |> ignore)
        stdout.WriteLine("Serving {0}", url)
        stdin.ReadLine() |> ignore
        0
