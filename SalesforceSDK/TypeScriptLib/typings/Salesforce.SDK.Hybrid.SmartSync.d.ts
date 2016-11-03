 /*
 * Copyright (c) 2015-present, salesforce.com, inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided
 * that the following conditions are met:
 *
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the
 * following disclaimer.
 *
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
 * the following disclaimer in the documentation and/or other materials provided with the distribution.
 *
 * Neither the name of salesforce.com, inc. nor the names of its contributors may be used to endorse or
 * promote products derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */



declare module Salesforce.SDK.Hybrid.SmartSync {

    export interface ISyncManagerClass {
        getSyncStatus(syncId: number): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        reSync(syncId: number, callback: string): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        runSync(sync: Salesforce.SDK.Hybrid.SmartSync.Models.SyncState, callback: string): void;
        sendRestRequest(request: Salesforce.SDK.Hybrid.Rest.RestRequest): Windows.Foundation.IAsyncOperation<Salesforce.SDK.Hybrid.Rest.RestResponse>;
        syncDown(target: string, soupName: string, callback: string, options: Salesforce.SDK.Hybrid.SmartSync.Models.SyncOptions): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        syncUp(target: Salesforce.SDK.Hybrid.SmartSync.Models.SyncUpTarget, options: Salesforce.SDK.Hybrid.SmartSync.Models.SyncOptions, soupName: string, callback: string): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
    }

    export class SyncManager implements Salesforce.SDK.Hybrid.SmartSync.ISyncManagerClass {
        constructor();
        static getInstance(): Salesforce.SDK.Hybrid.SmartSync.SyncManager;
        static getInstance(account: Salesforce.SDK.Hybrid.Auth.Account, communityId: string): Salesforce.SDK.Hybrid.SmartSync.SyncManager;
        static getInstance(account: Salesforce.SDK.Hybrid.Auth.Account, communityId: string, smartStore: Salesforce.SDK.Hybrid.SmartStore.SmartStore): Salesforce.SDK.Hybrid.SmartSync.SyncManager;
        static pluck(jArray: string, key: string): Windows.Foundation.Collections.IVector<string>;
        static reset(): void;
        getSyncStatus(syncId: number): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        reSync(syncId: number, callback: string): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        runSync(sync: Salesforce.SDK.Hybrid.SmartSync.Models.SyncState, callback: string): void;
        sendRestRequest(request: Salesforce.SDK.Hybrid.Rest.RestRequest): Windows.Foundation.IAsyncOperation<Salesforce.SDK.Hybrid.Rest.RestResponse>;
        syncDown(target: string, soupName: string, callback: string, options: Salesforce.SDK.Hybrid.SmartSync.Models.SyncOptions): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        syncUp(target: Salesforce.SDK.Hybrid.SmartSync.Models.SyncUpTarget, options: Salesforce.SDK.Hybrid.SmartSync.Models.SyncOptions, soupName: string, callback: string): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        toString(): string;
    }

}
declare module Salesforce.SDK.Hybrid.SmartSync.Models {

    export interface IMruSyncDownTargetClass {
        asJson(): string;
        continueFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager): string;
        startFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, maxTimeStamp: number): string;
    }

    export interface ISoqlSyncDownTargetClass {
        asJson(): string;
        continueFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager): string;
        startFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, maxTimeStamp: number): string;
    }

    export interface ISoslSyncDownTargetClass {
        asJson(): string;
        continueFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager): string;
        startFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, maxTimeStamp: number): string;
    }

    export interface ISyncOptionsClass {
    }

    export interface ISyncStateClass {
        asJson(): string;
        save(store: Salesforce.SDK.Hybrid.SmartStore.SmartStore): void;
        mergeMode: Salesforce.SDK.Hybrid.SmartSync.Models.MergeModeOptions;
    }

    export interface ISyncUpTargetClass {
        createOnServerAsync(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, objectType: string, fields: Windows.Foundation.Collections.IMap<string,any>): Windows.Foundation.IAsyncOperation<string>;
        deleteOnServer(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, objectType: string, objectId: string): Windows.Foundation.IAsyncOperation<boolean>;
        fetchLastModifiedDate(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, objectType: string, objectId: string): Windows.Foundation.IAsyncOperation<string>;
        getIdsOfRecordsToSyncUp(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, soupName: string): any;
        updateOnServer(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, objectType: string, objectId: string, fields: Windows.Foundation.Collections.IMap<string,any>): Windows.Foundation.IAsyncOperation<boolean>;
    }

    enum MergeModeOptions {
        none,
        overwrite,
        leaveIfChanged
    }

    export class MruSyncDownTarget implements Salesforce.SDK.Hybrid.SmartSync.Models.IMruSyncDownTargetClass {
        constructor(target: string);
        static targetForMruSyncDown(objectType: string, fieldList: Windows.Foundation.Collections.IVector<string>): string;
        asJson(): string;
        continueFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager): string;
        startFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, maxTimeStamp: number): string;
        toString(): string;
    }

    export class SoqlSyncDownTarget implements Salesforce.SDK.Hybrid.SmartSync.Models.ISoqlSyncDownTargetClass {
        constructor(query: string);
        static fromJson(target: string): string;
        asJson(): string;
        continueFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager): string;
        startFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, maxTimeStamp: number): string;
        toString(): string;
    }

    export class SoslSyncDownTarget implements Salesforce.SDK.Hybrid.SmartSync.Models.ISoslSyncDownTargetClass {
        constructor(query: string);
        asJson(): string;
        continueFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager): string;
        startFetch(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, maxTimeStamp: number): string;
        toString(): string;
    }

    export class SyncOptions implements Salesforce.SDK.Hybrid.SmartSync.Models.ISyncOptionsClass {
        constructor();
        static fromJson(options: string): Salesforce.SDK.Hybrid.SmartSync.Models.SyncOptions;
        static optionsForSyncDown(mergeMode: Salesforce.SDK.Hybrid.SmartSync.Models.MergeModeOptions): Salesforce.SDK.Hybrid.SmartSync.Models.SyncOptions;
        static optionsForSyncUp(fieldList: Windows.Foundation.Collections.IVector<string>, mergeMode: Salesforce.SDK.Hybrid.SmartSync.Models.MergeModeOptions): Salesforce.SDK.Hybrid.SmartSync.Models.SyncOptions;
        toString(): string;
    }

    export class SyncState implements Salesforce.SDK.Hybrid.SmartSync.Models.ISyncStateClass {
        constructor();
        static byId(store: Salesforce.SDK.Hybrid.SmartStore.SmartStore, id: number): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        static createSyncDown(store: Salesforce.SDK.Hybrid.SmartStore.SmartStore, target: string, soupName: string, options: Salesforce.SDK.Hybrid.SmartSync.Models.SyncOptions): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        static createSyncUp(store: Salesforce.SDK.Hybrid.SmartStore.SmartStore, target: Salesforce.SDK.Hybrid.SmartSync.Models.SyncUpTarget, options: Salesforce.SDK.Hybrid.SmartSync.Models.SyncOptions, soupName: string): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        static fromJson(sync: string): Salesforce.SDK.Hybrid.SmartSync.Models.SyncState;
        static setupSyncsSoupIfNeeded(store: Salesforce.SDK.Hybrid.SmartStore.SmartStore): void;
        asJson(): string;
        save(store: Salesforce.SDK.Hybrid.SmartStore.SmartStore): void;
        toString(): string;
        mergeMode: Salesforce.SDK.Hybrid.SmartSync.Models.MergeModeOptions;
    }

    enum SyncStatusTypes {
        new,
        running,
        done,
        failed
    }

    enum SyncTypes {
        syncDown,
        syncUp
    }

    export class SyncUpTarget implements Salesforce.SDK.Hybrid.SmartSync.Models.ISyncUpTargetClass {
        constructor(target: string);
        static fromJSON(target: string): Salesforce.SDK.Hybrid.SmartSync.Models.SyncUpTarget;
        createOnServerAsync(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, objectType: string, fields: Windows.Foundation.Collections.IMap<string,any>): Windows.Foundation.IAsyncOperation<string>;
        deleteOnServer(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, objectType: string, objectId: string): Windows.Foundation.IAsyncOperation<boolean>;
        fetchLastModifiedDate(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, objectType: string, objectId: string): Windows.Foundation.IAsyncOperation<string>;
        getIdsOfRecordsToSyncUp(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, soupName: string): any;
        updateOnServer(syncManager: Salesforce.SDK.Hybrid.SmartSync.SyncManager, objectType: string, objectId: string, fields: Windows.Foundation.Collections.IMap<string,any>): Windows.Foundation.IAsyncOperation<boolean>;
        toString(): string;
    }

}

