module Actors

open Akka.FSharp
open Akka.Actor

// ............. Messages ...................... // 
type MasterMessages = 
    | RegisterWorker 
    | UnregisterWorker 
    | GimmeWork

type WorkerMessages  =
    | WorkAvailable
    | Work

// ............. Actors ...................... // 
let masterActor (mailbox : Actor<_>) = 
    let rec loop workers = actor {
        let! message = mailbox.Receive () 
        match message with 
        | RegisterWorker -> 
            let current = mailbox.Sender ()
            let workersWithoutCurrent = workers |> List.filter(fun x -> x <> current)
            return! loop (current::workersWithoutCurrent)
        
        | UnregisterWorker -> 
            let current = mailbox.Sender () 
            let workersWithoutCurrent = workers |> List.filter(fun x -> x <> current)
            return! loop workersWithoutCurrent

        | GimmeWork -> 
            printf "TODO: Pending Give work to worker"

        return! loop workers
    }
    loop [] 


let workerActor (master: IActorRef) (mailbox: Actor<_>) =
    // pre-start 
    master <! RegisterWorker 
    master <! GimmeWork
    
    // this registers a function to be called on post-stop
    mailbox.Defer (fun () -> master <! UnregisterWorker)

    let rec loop () = actor {
        let! message = mailbox.Receive () 
        match message with 
        | WorkAvailable -> master <! GimmeWork
        | Work -> printf "TODO: Pending processing Work!!!"
        
        return! loop()
    }
    loop () 

