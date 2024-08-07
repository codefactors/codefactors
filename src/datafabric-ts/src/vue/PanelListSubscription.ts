//
// Copyright (c) 2023-2024 Pure Software Ltd.  All rights reserved.    
//                                                               
// This source code is the intellectual property of Pure Software 
// Ltd and for information security purposes is classified as     
// COMPANY CONFIDENTIAL.

import { Ref } from "vue"
import { DataFabricSubscription, DataFabricSubscriptionOptions, DataFabricUpdate, SubscriptionData } from "./DataFabric"
import { Factory } from "./core/Factory"

export class PanelListSubscription<T, TDto> extends DataFabricSubscription<TDto[]> {

    private _sortFunction?: (a: any, b: any) => number
    private _target: Ref<T[]>
    private _factory: Factory<T, TDto>

    constructor(
        target: Ref<T[]>,
        factory: Factory<T, TDto>,
        sortFunction?: (a: any, b: any) => number,
        dataFabricOptions?: DataFabricSubscriptionOptions<TDto[]>) {
        super(dataFabricOptions)

        this._target = target
        this._factory = factory
        this._sortFunction = sortFunction

        this._target.value = [] as T[]

        if (dataFabricOptions?.onData) this.onData = dataFabricOptions.onData
    }

    onData(seqNo: number, data: SubscriptionData<TDto[]>) {
        console.log("PanelSubscription.onData:", data.subscriptionData)
        if (Array.isArray(data.subscriptionData) && data.subscriptionData.length > 0) {
            var entities = [] as T[]
            data.subscriptionData.forEach(dto => {
                entities.push(this._factory.createFromDto(dto))
            })
            this._target.value = this._sortFunction == undefined ? entities : entities.sort(this._sortFunction)
        }
    }

    onUpdate(seqNo: number, update: DataFabricUpdate<TDto[]>): void {
        if (update.data) {
            console.log("PanelSubscription.onUpdate:", update.data)
            //            this._target.value = { ...this._target.value, ...update.update }
        }
    }

    onError(error: Error): void {
        console.error(error)
    }
}