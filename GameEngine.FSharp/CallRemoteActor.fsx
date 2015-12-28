(* 
    Calling remote actors
*)

#r @"..\packages\Akka.1.0.5\lib\net45\Akka.dll"
#r @"..\packages\Akkling.0.2.1\lib\net45\Akkling.dll"
#r @"..\packages\Akka.Remote.1.0.5\lib\net45\Akka.Remote.dll"
#r @"..\packages\Helios.1.4.1\lib\net45\Helios.dll"
#r @"..\packages\Google.ProtocolBuffers.2.4.1.521\lib\net40\Google.ProtocolBuffers.dll"
#r @"..\packages\Google.ProtocolBuffers.2.4.1.521\lib\net40\Google.ProtocolBuffers.Serialization.dll"
#r @"..\GameEngine.Messages\bin\Debug\GameEngine.Messages.dll"
#r @"..\packages\Newtonsoft.Json.7.0.1\lib\net45\Newtonsoft.Json.dll"
#r @"..\packages\Wire.0.0.6\lib\Wire.dll"
#I @"..\packages\FsPickler.1.4.0\lib\net45"

open Akka
open Akkling
open GameEngine.Messages

exception TechError of string

let configuration = Configuration.parse(@"
    akka {
        actor {
            provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
        }
        remote {
            helios.tcp {
                port = 8090
                hostname = localhost
            }
        }
    }
    ")


let actorSystem = System.create "GameStateClient" (configuration)


// this gives a warning which you can ignore
let registerPlayer = select<RegisterPlayerMessage> actorSystem "akka.tcp://GameStateServer@localhost:8080/user/RegisterPlayer"



//  Send a "register me" message to the RegisterPlayer portal and wait until I receive an ActorRef, with which I will communicate from now.
//  This is a simple construct, very usable for sending messages. It does not work very well for receiving. Rewrite to support push, we need an actor here.
let player = 
    async {
        let! untypedMessage = registerPlayer <? GameEngine.Messages.RegisterMe
        let typedMessage = unbox<RegisterPlayerMessage>(untypedMessage)
        match  typedMessage with
        |   YouAreRegisterd (a) -> return ActorRefs.typed<PlayerMessage>(a) // ignore the warnings
        |   _   -> return  raise (TechError(sprintf "Unexpected message: %A" typedMessage ))
    } |> Async.RunSynchronously


//  lets get my id from the actor
let myId = 
    async {
        let! response = player <? GameEngine.Messages.WhatIsMyId
        match response with
        | YourId(id)    -> return id
        | _ -> return  raise (TechError(sprintf "Unexpected message: %A" response ))
    } |> Async.RunSynchronously

myId.ToString()


