namespace GameUI.Suave


open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client

[<JavaScript>]
module Client =

    let Start input k =
        async {
            let! data = Server.DoSomething input
            return k data
        }
        |> Async.Start

    let Main () =
        let inputVal = Var.Create ""
        let output = Var.Create ""
        div [
            Doc.Input [] inputVal

            Doc.Button "Send" [] (
                fun () ->
                    async {
                        let! data = Server.DoSomething inputVal.Value
                        Var.Set output data
                    } |> Async.Start
                )
            hr []
            h4Attr [attr.``class`` "text-muted"] [text "The server responded:"]
            divAttr [attr.``class`` "jumbotron"] [h1 [textView output.View]]
        ]
