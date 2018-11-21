// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open Akka.FSharp
open Actors

[<EntryPoint>]
let main argv = 
    let system = System.create "system" <| Configuration.load () 
    let masterRef = spawn system "masterActor" masterActor 
    0 // return an integer exit code
