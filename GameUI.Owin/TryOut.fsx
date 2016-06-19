
type I = 
    abstract WhatsMyClass : unit -> string

type A() =
    interface I with
        member this.WhatsMyClass() = "A"

type B() =
    interface I with
        member this.WhatsMyClass() = "B"

//let mylist = [A(); A(); B()]

