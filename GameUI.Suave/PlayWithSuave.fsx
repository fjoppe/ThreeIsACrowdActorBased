#r @"..\packages\Suave.1.1.0\lib\net40\Suave.dll"

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open System.IO
open Suave.Web


let staticFile = "http://localhost:8083/App_Themes/Standard/BlueHexagon.png"

let myHomeFolder = __SOURCE_DIRECTORY__

printf "%s" myHomeFolder 

let cfg = {  defaultConfig with homeFolder = Some(myHomeFolder)}

let routing = 
    choose [
        path "/App_Themes" >=> OK "test" ; Files.browseHome
        RequestErrors.NOT_FOUND "Page not found."
    ]

startWebServer cfg routing // (WebSharperAdapter.ToWebPart Main)
