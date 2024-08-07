//
// Copyright (c) 2023-2024 Pure Software Ltd.  All rights reserved.    
//                                                               
// This source code is the intellectual property of Pure Software 
// Ltd and for information security purposes is classified as     
// COMPANY CONFIDENTIAL.

import { Ref } from "vue"
import { DataFabricSubscription, DataFabricSubscriptionOptions, DataFabricUpdate, SubscriptionData } from "./DataFabric"
import { Factory } from "./core/Factory"

export class PanelSubscription<T, TDto> extends DataFabricSubscription<TDto> {

    private _target: Ref<T>
    private _factory: Factory<T, TDto>

    constructor(
        target: Ref<T>,
        factory: Factory<T, TDto>,
        options?: DataFabricSubscriptionOptions<TDto>) {
        super(options)

        this._target = target
        this._factory = factory

        this._target.value = this._factory.createEmpty()

        if (options?.onData) this.onData = options.onData
    }

    onData(seqNo: number, data: SubscriptionData<TDto>) {
        console.log("PanelSubscription.onData:", data.subscriptionData)
        this._target.value = this._factory.createFromDto(data.subscriptionData)
    }

    onUpdate(seqNo: number, update: DataFabricUpdate<TDto>): void {
        if (update.data) {
            console.log("PanelSubscription.onUpdate:", update.data)
            //            this._target.value = { ...this._target.value, ...update.update }
        }
    }

    onError(error: Error): void {
        console.error(error)
    }
}