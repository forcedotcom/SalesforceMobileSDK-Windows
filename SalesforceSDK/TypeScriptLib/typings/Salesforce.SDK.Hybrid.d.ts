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

/// <reference path="../typings/winrt.d.ts"/>

declare module Salesforce.SDK.Hybrid.Auth {

    export class Account implements Salesforce.SDK.Hybrid.Auth.IAccountClass {
        constructor(loginUrl: string, clientId: string, callbackUrl: string, scopes: string[], instanceUrl: string, identityUrl: string, accessToken: string, refreshToken: string);
        static fromJson(accountJson: string): Salesforce.SDK.Hybrid.Auth.Account;
        static toJson(account: Salesforce.SDK.Hybrid.Auth.Account): string;
        getLoginOptions(): Salesforce.SDK.Hybrid.Auth.LoginOptions;
        toString(): string;
        ToString(): string;
        loginUrl: string;
        userId: string;
        userName: string;
        clientId: string;
        callbackUrl: string;
        scopes: string[];
        instanceUrl: string;
        identityUrl: string;
        accessToken: string;
        refreshToken: string;
        communityId: string;
        communityUrl: string;
        policy: Salesforce.SDK.Hybrid.Auth.MobilePolicy;
    }

    export class AuthResponse implements Salesforce.SDK.Hybrid.Auth.IAuthResponseClass {
        constructor();
        ToString(): string;
        scopes: string[];
        identityUrl: string;
        instanceUrl: string;
        issuedAt: string;
        accessToken: string;
        refreshToken: string;
        scopesStr: string;
        communityId: string;
        communityUrl: string;
    }

    export class HybridAccountManager implements Salesforce.SDK.Hybrid.Auth.IHybridAccountManagerClass {
        constructor();
        static createNewAccount(loginOptions: Salesforce.SDK.Hybrid.Auth.LoginOptions, response: string): Windows.Foundation.IAsyncOperation<Salesforce.SDK.Hybrid.Auth.Account>;
        static switchToAccount(account: Salesforce.SDK.Hybrid.Auth.Account): Windows.Foundation.IAsyncOperation<boolean>;
        static deleteAccount(): void;
        static getAccount(): Salesforce.SDK.Hybrid.Auth.Account;
        static getAccounts(): Windows.Foundation.Collections.IMap<string,Salesforce.SDK.Hybrid.Auth.Account>;
        static getInstance(): Salesforce.SDK.Hybrid.Auth.HybridAccountManager;
        static initEncryption(): void;
        static wipeAccounts(): void;
        ToString(): string;
    }

    export interface IAccountClass {
        getLoginOptions(): Salesforce.SDK.Hybrid.Auth.LoginOptions;
        toString(): string;
        loginUrl: string;
        userId: string;
        userName: string;
        clientId: string;
        callbackUrl: string;
        scopes: string[];
        instanceUrl: string;
        identityUrl: string;
        accessToken: string;
        refreshToken: string;
        communityId: string;
        communityUrl: string;
        policy: Salesforce.SDK.Hybrid.Auth.MobilePolicy;
    }

    export interface IAuthResponseClass {
        scopes: string[];
        identityUrl: string;
        instanceUrl: string;
        issuedAt: string;
        accessToken: string;
        refreshToken: string;
        scopesStr: string;
        communityId: string;
        communityUrl: string;
    }

    export interface IHybridAccountManagerClass {
    }

    export interface ILoginOptionsClass {
        loginUrl: string;
        clientId: string;
        callbackUrl: string;
        displayType: string;
        scopes: string[];
    }

    export interface IMobilePolicyClass {
        pinLength: number;
        screenLockTimeout: number;
        pincodeHash: string;
    }

    export interface IOAuth2Class {
    }

    export class LoginOptions implements Salesforce.SDK.Hybrid.Auth.ILoginOptionsClass {
        constructor();
        constructor(loginUrl: string, clientId: string, callbackUrl: string, scopes: string[]);
        constructor(loginUrl: string, clientId: string, callbackUrl: string, displayType: string, scopes: string[]);
        ToString(): string;
        loginUrl: string;
        clientId: string;
        callbackUrl: string;
        displayType: string;
        scopes: string[];
    }

    export class MobilePolicy implements Salesforce.SDK.Hybrid.Auth.IMobilePolicyClass {
        constructor();
        ToString(): string;
        pinLength: number;
        screenLockTimeout: number;
        pincodeHash: string;
    }

    export class OAuth2 implements Salesforce.SDK.Hybrid.Auth.IOAuth2Class {
        constructor();
        static computeAuthorizationUrl(options: Salesforce.SDK.Hybrid.Auth.LoginOptions): string;
        static computeFrontDoorUrl(instanceUrl: string, accessToken: string, url: string): string;
        static computeFrontDoorUrl(instanceUrl: string, displayType: string, accessToken: string, url: string): string;
        static parseFragment(fragmentstring: string): Salesforce.SDK.Hybrid.Auth.AuthResponse;
        static refreshAuthTokenRequest(loginOptions: Salesforce.SDK.Hybrid.Auth.LoginOptions, refreshToken: string): Windows.Foundation.IAsyncOperation<Salesforce.SDK.Hybrid.Auth.AuthResponse>;
        static refreshAuthToken(account: Salesforce.SDK.Hybrid.Auth.Account): Windows.Foundation.IAsyncOperation<Salesforce.SDK.Hybrid.Auth.Account>;
        static revokeAuthToken(loginOptions: Salesforce.SDK.Hybrid.Auth.LoginOptions, refreshToken: string): Windows.Foundation.IAsyncOperation<boolean>;
        ToString(): string;
    }

}
declare module Salesforce.SDK.Hybrid.Rest {

    export interface AccessTokenProvider {
        target: any;
        detail: any[];
        type: string;
    }

    export class ClientManager implements Salesforce.SDK.Hybrid.Rest.IClientManagerClass {
        constructor();
        logout(): Windows.Foundation.IAsyncOperation<boolean>;
        peekRestClient(): Salesforce.SDK.Hybrid.Rest.RestClient;
        ToString(): string;
    }

    enum ContentTypeValues {
        formUrlEncoded,
        json,
        xml,
        none
    }

    export interface IClientManagerClass {
        logout(): Windows.Foundation.IAsyncOperation<boolean>;
        peekRestClient(): Salesforce.SDK.Hybrid.Rest.RestClient;
    }

    export interface IRestClientClass {
        sendAsync(request: Salesforce.SDK.Hybrid.Rest.RestRequest): Windows.Foundation.IAsyncOperation<Salesforce.SDK.Hybrid.Rest.RestResponse>;
        sendAsync(method: any, url: string): Windows.Foundation.IAsyncOperation<Salesforce.SDK.Hybrid.Rest.RestResponse>;
        instanceUrl: string;
        accessToken: string;
    }

    export interface IRestRequestClass {
    }

    export interface IRestResponseClass {
        success: boolean;
        error: Windows.Foundation.HResult;
        asString: string;
        statusCode: any;
    }

    export class RestClient implements Salesforce.SDK.Hybrid.Rest.IRestClientClass {
        sendAsync(request: Salesforce.SDK.Hybrid.Rest.RestRequest): Windows.Foundation.IAsyncOperation<Salesforce.SDK.Hybrid.Rest.RestResponse>;
        sendAsync(method: any, url: string): Windows.Foundation.IAsyncOperation<Salesforce.SDK.Hybrid.Rest.RestResponse>;
        ToString(): string;
        instanceUrl: string;
        accessToken: string;
    }

    export class RestRequest implements Salesforce.SDK.Hybrid.Rest.IRestRequestClass {
        constructor();
        constructor(method: any, path: string);
        constructor(method: any, path: string, requestBody: string);
        constructor(method: any, path: string, requestBody: string, contentType: Salesforce.SDK.Hybrid.Rest.ContentTypeValues);
        constructor(method: any, path: string, requestBody: string, contentType: Salesforce.SDK.Hybrid.Rest.ContentTypeValues, additionalHeaders: Windows.Foundation.Collections.IMap<string,string>);
        static getRequestForCreate(apiVersion: string, objectType: string, fields: Windows.Foundation.Collections.IMap<string,any>): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForDelete(apiVersion: string, objectType: string, objectId: string): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForDescribeGlobal(apiVersion: string): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForDescribe(apiVersion: string, objectType: string): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForMetadata(apiVersion: string, objectType: string): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForQuery(apiVersion: string, q: string): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForResources(apiVersion: string): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForRetrieve(apiVersion: string, objectType: string, objectId: string, fieldsList: string[]): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForSearch(apiVersion: string, q: string): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForUpdate(apiVersion: string, objectType: string, objectId: string, fields: Windows.Foundation.Collections.IMap<string,any>): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForUpsert(apiVersion: string, objectType: string, externalIdField: string, externalId: string, fields: Windows.Foundation.Collections.IMap<string,any>): Salesforce.SDK.Hybrid.Rest.RestRequest;
        static getRequestForVersions(): Salesforce.SDK.Hybrid.Rest.RestRequest;
        ToString(): string;
    }

    export class RestResponse implements Salesforce.SDK.Hybrid.Rest.IRestResponseClass {
        constructor();
        ToString(): string;
        success: boolean;
        error: Windows.Foundation.HResult;
        asString: string;
        statusCode: any;
    }

}

