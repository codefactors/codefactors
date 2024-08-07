// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

import { SubscriptionData } from "./DataFabric"

export class DataFabricWorker {

    private _worker: SharedWorker
    // private _port: MessagePort
    // private _onData: (seqNo: number, data: object) => void

    constructor(onData: (seqNo: number, data: SubscriptionData<object>) => void) {
        console.log("DataFabricWorker created")

        this._worker = new SharedWorker("src/lib/DataFabricWorker.ts", { type: "module" })

        this._worker.port.addEventListener("message", function (e: MessageEvent) {
            console.log('DataFabricWorker.onmessage:', e.data)
            onData(1, e.data)
        }, false)

        this._worker.port.start()
    }

    addSubscription(): void {
        console.log("DataFabricWorker.addSubscription")
        //this.postMessage("addSubscription")
    }

    postMessage(message: any): void {
        this._worker.port.postMessage(message)
    }

    terminate(): void {
        //this._worker.terminate()
    }
}