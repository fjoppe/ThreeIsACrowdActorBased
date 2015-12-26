
type Artefact() = class end

type Visual() = 
    inherit Artefact()

type Container(gameObjects : Visual list) =
    inherit Artefact()
    member private this.GameObjects = gameObjects

type Scene(gameObjects : Artefact list) = 
    member private this.GameObjects = gameObjects

type Game(scenes : Scene list) =
    member private this.Scenes = scenes


module GameObjectMod =
    let CreateVisual() = new Visual()
    let CreateContainer go = new Container(go)


module SceneMod =
    let Create go = new Scene(go)

module GameMod = 
    let Create scs = Game(scs)


let v1 = GameObjectMod.CreateVisual()
let v2 = GameObjectMod.CreateVisual()
let v3 = GameObjectMod.CreateVisual()
let c1 = GameObjectMod.CreateContainer([v1; v2])

let s = SceneMod.Create [c1; v3]

let g = GameMod.Create [s]

