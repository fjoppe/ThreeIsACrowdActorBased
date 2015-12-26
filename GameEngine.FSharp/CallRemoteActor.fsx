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

let actorSystem = System.create "ThisIsMyClient" (configuration)

let registerPlayer = select actorSystem "akka.tcp://ThisIsMyServer@localhost:8080/user/RegisterPlayer"


let registerPlayer = TypedActorSelection<RegisterPlayerMessage>(actorSystem.ActorSelection "akka.tcp://ThisIsMyServer@localhost:8080/user/RegisterPlayer")




let player = 
    async {
        let! player = registerPlayer <? GameEngine.Messages.RegisterMe
        return player :> obj
    } |> Async.RunSynchronously



let myId = 
    async {
        let! response = Player <? GameEngine.Messages.WhatIsMyId
        return response :> obj
    } |> Async.RunSynchronously


myId.ToString()

unbox<PlayerMessageResponse> myId

