// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

import { App, Ref, inject, onMounted, onUnmounted, watch } from "vue"
import { DataFabricWorker } from "./DataFabricWorker"
import { DataFabricConfiguration } from "./DataFabricConfiguration"

// Create - need to create a global data fabric and somehow place in global scope

// Subscribe - need to subscribe to a stream from the data fabric
// - may fail if the stream does not exist
// - may fail if the stream is not accessible (eg network issues, security issues)
// -- needs timeout in case of failure as request is asynchronous

// TryUpdate - need to try to update an object in the data fabric
// - if the object is not in the data fabric, then it is created? - NO - use TryCreate
// - if the object is already in the data fabric, then the update is applied?
// - if the object is already in the data fabric, but has been updated since the update was created, then the update is rejected?
// - if the object is already in the data fabric, but has been deleted since the update was created, then the update is rejected?
// -- what happens if update fails? Need to tell the user

// TryDelete - need to try to delete an object in the data fabric

// TryCreate - need to try to create an object in the data fabric

// Unsubscribe - need to unsubscribe from a stream from the data fabric - stop sending updates

/// Use SignalR to maintain a connection to the data fabric
/// Use regular expression to validate the stream name
/// Can we share DataFabric objects between multiple tabs?
/// Need auth on SignalR connection

/// All requests to the data fabric must carry the auth token
/// Need to handle auth token expiry

export enum UpdateStreamMode {
    Snapshot = 1,
    ChangedFields = 2,
    NotificationOnly = 3,
    EntireObject = 4
}

export type DataFabricOptions = {
    updateStreamMode: UpdateStreamMode
}

export type DataFabricUpdate<T> = {
    data: T | Partial<T> | undefined,
    updateType: UpdateStreamMode
}

export type SubscriptionData<TDto> = {
    subscriptionData: TDto
}

export type OnData<T> = (seqNo: number, data: T) => void

export type OnUpdate<T> = (seqNo: number, update: DataFabricUpdate<T>) => void

export type OnError = (error: Error) => void

export type DataFabricSubscriptionOptions<T> = {
    onData?: OnData<SubscriptionData<T>>,
    onUpdate?: OnUpdate<T>,
    onError?: OnError,
    sortLists?: boolean
}

export abstract class DataFabricSubscription<T> implements Disposable {

    private _token: string = ""

    constructor(options?: DataFabricSubscriptionOptions<T>) {
    }

    [Symbol.dispose](): void {
        throw new Error("Method not implemented.")
    }

    get token() { return this._token }

    updateToken(token: string): void { this._token = token }

    abstract onData(seqNo: number, data: SubscriptionData<T>): void

    abstract onUpdate(seqNo: number, update: DataFabricUpdate<T>): void

    abstract onError(error: Error): void
}

/**
 * Represents an instance of the data fabric that enables subscriptions to be opened and closed.
 */
export class DataFabric {

    private _subscriptions: Map<string, DataFabricSubscription<any>> = new Map<string, DataFabricSubscription<any>>()
    private _dataFabricWorker: DataFabricWorker

    constructor(options: DataFabricOptions = { updateStreamMode: UpdateStreamMode.NotificationOnly }) {
        console.log("DataFabric created " + options.updateStreamMode)

        this._dataFabricWorker = new DataFabricWorker((seqNo, data) => {
            this._subscriptions.forEach((subscriber, stream) => subscriber.onData(seqNo, data))
        })
    }

    subscribe<T>(subscriber: DataFabricSubscription<T>, stream: string): void {
        this._subscriptions.set(stream, subscriber)
        console.log("Subscribed to data fabric for stream " + stream)
        this._dataFabricWorker.postMessage(stream)
    }

    close<T>(subscriber: DataFabricSubscription<T>): void {
        this._subscriptions.delete(subscriber.token)
        console.log("Unsubscribed from data fabric")
    }
}

// use... export for composition API to use plugin 
export function useDataFabric<T>(
    uri: Ref<string>,
    subscription: DataFabricSubscription<T>,
    options: DataFabricOptions = { updateStreamMode: UpdateStreamMode.NotificationOnly }
): DataFabricSubscription<T> {

    watch(uri, (newValue: string, oldValue: string) => {
        console.log("URI changed from " + oldValue + " to " + newValue)
    })

    const fabric = inject('dataFabric') as DataFabric

    onMounted(() => { fabric.subscribe<T>(subscription, uri.value) })
    onUnmounted(() => { fabric.close(subscription) })

    return subscription
}

// Default export for plugin install
export default {
    install(app: App, configuration: DataFabricConfiguration) {
        app.provide('dataFabric', new DataFabric())
    }
}