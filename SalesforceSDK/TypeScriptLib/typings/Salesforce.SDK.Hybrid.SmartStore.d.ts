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

declare module Salesforce.SDK.Hybrid.SmartStore {

    export class DBOpenHelper implements Salesforce.SDK.Hybrid.SmartStore.IDBOpenHelperClass {
        constructor();
        static getOpenHelper(account: Salesforce.SDK.Hybrid.Auth.Account): Salesforce.SDK.Hybrid.SmartStore.DBOpenHelper;
        static getOpenHelper(account: Salesforce.SDK.Hybrid.Auth.Account, communityId: string): Salesforce.SDK.Hybrid.SmartStore.DBOpenHelper;
        static getOpenHelper(dbNamePrefix: string, account: Salesforce.SDK.Hybrid.Auth.Account, communityId: string): Salesforce.SDK.Hybrid.SmartStore.DBOpenHelper;
        toString(): string;
        databaseFile: string;
    }

    export interface IDBOpenHelperClass {
        databaseFile: string;
    }

    export interface IIndexSpecClass {
    }

    export class IndexSpec implements Salesforce.SDK.Hybrid.SmartStore.IIndexSpecClass {
        constructor();
        constructor(path: string, type: Salesforce.SDK.Hybrid.SmartStore.SmartStoreType);
        constructor(path: string, type: Salesforce.SDK.Hybrid.SmartStore.SmartStoreType, columnName: string);
        static mapForIndexSpecs(indexSpecs: Salesforce.SDK.Hybrid.SmartStore.IndexSpec[]): Windows.Foundation.Collections.IMap<string,Salesforce.SDK.Hybrid.SmartStore.IndexSpec>;
        toString(): string;
    }

    export interface IQuerySpecClass {
    }

    export interface ISmartStoreClass {
        clearSoup(soupName: string): void;
        convertSmartSql(smartSql: string): string;
        countQuery(querySpec: Salesforce.SDK.Hybrid.SmartStore.QuerySpec): number;
        createMetaTables(): void;
        create(soupName: string, soupElt: string): any;
        create(soupName: string, soupElt: string, handleTx: boolean): any;
        delete(soupName: string, soupEntryIds: number[], handleTx: boolean): boolean;
        dropAllSoups(): void;
        dropAllSoups(databasePath: string): void;
        dropSoup(soupName: string): void;
        getAllSoupNames(): Windows.Foundation.Collections.IVector<string>;
        getSoupIndexSpecs(soupName: string): Salesforce.SDK.Hybrid.SmartStore.IndexSpec[];
        hasSoup(soupName: string): boolean;
        lookupSoupEntryId(soupName: string, fieldPath: string, fieldValue: string): number;
        query(querySpec: Salesforce.SDK.Hybrid.SmartStore.QuerySpec, pageIndex: number): string;
        registerSoup(soupName: string, indexSpecs: Salesforce.SDK.Hybrid.SmartStore.IndexSpec[]): void;
        reIndexSoup(soupName: string, indexPaths: string[], handleTx: boolean): void;
        resetDatabase(): void;
        retrieve(soupName: string, soupEntryIds: number[]): string;
        update(soupName: string, soupElt: string, soupEntryId: number, handleTx: boolean): any;
        upsert(soupName: string, soupElt: string): any;
        upsert(soupName: string, soupElt: string, externalIdPath: string): any;
        upsert(soupName: string, soupElt: string, externalIdPath: string, handleTx: boolean): any;
    }

    export interface ISmartStoreTypeClass {
        columnType: string;
    }

    export class QuerySpec implements Salesforce.SDK.Hybrid.SmartStore.IQuerySpecClass{
        constructor();
        static buildAllQuerySpec(soupName: string, path: string, order: Salesforce.SDK.Hybrid.SmartStore.SqlOrder, pageSize: number): Salesforce.SDK.Hybrid.SmartStore.QuerySpec;
        static buildExactQuerySpec(soupName: string, path: string, exactMatchKey: string, pageSize: number): Salesforce.SDK.Hybrid.SmartStore.QuerySpec;
        static buildLikeQuerySpec(soupName: string, path: string, likeKey: string, order: Salesforce.SDK.Hybrid.SmartStore.SqlOrder, pageSize: number): Salesforce.SDK.Hybrid.SmartStore.QuerySpec;
        static buildRangeQuerySpec(soupName: string, path: string, beginKey: string, endKey: string, order: Salesforce.SDK.Hybrid.SmartStore.SqlOrder, pageSize: number): Salesforce.SDK.Hybrid.SmartStore.QuerySpec;
        static buildSmartQuerySpec(smartSql: string, pageSize: number): Salesforce.SDK.Hybrid.SmartStore.QuerySpec;
        toString(): string;
    }

    enum SmartQueryType {
        smart,
        exact,
        range,
        like
    }

    export class SmartStore implements Salesforce.SDK.Hybrid.SmartStore.ISmartStoreClass{
        constructor();
        static deleteAllDatabases(includeGlobal: boolean): void;
        static generateDatabasePath(account: Salesforce.SDK.Hybrid.Auth.Account): string;
        static getGlobalSmartStore(): Salesforce.SDK.Hybrid.SmartStore.SmartStore;
        static getSmartStore(): Salesforce.SDK.Hybrid.SmartStore.SmartStore;
        static getSmartStore(account: Salesforce.SDK.Hybrid.Auth.Account): Salesforce.SDK.Hybrid.SmartStore.SmartStore;
        static getSoupTableName(soupId: number): string;
        static hasGlobalSmartStore(): Windows.Foundation.IAsyncOperation<boolean>;
        static hasSmartStore(account: Salesforce.SDK.Hybrid.Auth.Account): Windows.Foundation.IAsyncOperation<boolean>;
        static project(soup: any, path: string): any;
        clearSoup(soupName: string): void;
        convertSmartSql(smartSql: string): string;
        countQuery(querySpec: Salesforce.SDK.Hybrid.SmartStore.QuerySpec): number;
        createMetaTables(): void;
        create(soupName: string, soupElt: string): any;
        create(soupName: string, soupElt: string, handleTx: boolean): any;
        delete(soupName: string, soupEntryIds: number[], handleTx: boolean): boolean;
        dropAllSoups(): void;
        dropAllSoups(databasePath: string): void;
        dropSoup(soupName: string): void;
        getAllSoupNames(): Windows.Foundation.Collections.IVector<string>;
        getSoupIndexSpecs(soupName: string): Salesforce.SDK.Hybrid.SmartStore.IndexSpec[];
        hasSoup(soupName: string): boolean;
        lookupSoupEntryId(soupName: string, fieldPath: string, fieldValue: string): number;
        query(querySpec: Salesforce.SDK.Hybrid.SmartStore.QuerySpec, pageIndex: number): string;
        registerSoup(soupName: string, indexSpecs: Salesforce.SDK.Hybrid.SmartStore.IndexSpec[]): void;
        reIndexSoup(soupName: string, indexPaths: string[], handleTx: boolean): void;
        resetDatabase(): void;
        retrieve(soupName: string, soupEntryIds: number[]): string;
        update(soupName: string, soupElt: string, soupEntryId: number, handleTx: boolean): any;
        upsert(soupName: string, soupElt: string): any;
        upsert(soupName: string, soupElt: string, externalIdPath: string): any;
        upsert(soupName: string, soupElt: string, externalIdPath: string, handleTx: boolean): any;
        toString(): string;
    }

    export class SmartStoreType implements Salesforce.SDK.Hybrid.SmartStore.ISmartStoreTypeClass{
        constructor();
        constructor(columnType: string);
        toString(): string;
        static smartInteger: Salesforce.SDK.Hybrid.SmartStore.SmartStoreType;
        static smartString: Salesforce.SDK.Hybrid.SmartStore.SmartStoreType;
        static smartFloating: Salesforce.SDK.Hybrid.SmartStore.SmartStoreType;
        columnType: string;
    }

    enum SqlOrder {
        asc,
        desc
    }

}

