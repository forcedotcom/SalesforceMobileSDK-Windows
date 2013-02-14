/*
 * Copyright (c) 2013, salesforce.com, inc.
 * All rights reserved.
 * Redistribution and use of this software in source and binary forms, with or
 * without modification, are permitted provided that the following conditions
 * are met:
 * - Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * - Neither the name of salesforce.com, inc. nor the names of its contributors
 * may be used to endorse or promote products derived from this software without
 * specific prior written permission of salesforce.com, inc.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */
using Newtonsoft.Json.Linq;
using Salesforce.Sample.RestExplorer.Shared;
using Salesforce.SDK.Net;
using Salesforce.SDK.Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace Salesforce.Sample.RestExplorer.ViewModels
{
    public class RestActionViewModel : INotifyPropertyChanged
    {
        // Bound properties
        public const String RETURNED_REST_RESPONSE = "ReturnedRestResponse";
        // Bound indexed properties
        public const String SELECTED_REST_ACTION = "SelectedRestAction";
        public const String API_VERSION = "ApiVersion";
        public const String OBJECT_TYPE = "ObjectType";
        public const String OBJECT_ID = "ObjectId";
        public const String EXTERNAL_ID_FIELD = "ExternalIdField";
        public const String EXTERNAL_ID = "ExternalId";
        public const String FIELD_LIST = "FieldList";
        public const String FIELDS = "Fields";
        public const String SOQL = "Soql";
        public const String SOSL = "Sosl";
        public const String REQUEST_PATH = "RequestPath";
        public const String REQUEST_BODY = "RequestBody";
        public const String REQUEST_METHOD = "RequestMethod";

        public SynchronizationContext SyncContext;

        private SendRequestCommand _sendRequestCommand;
        public SendRequestCommand SendRequest
        {
            get
            {
                if (_sendRequestCommand == null)
                {
                    _sendRequestCommand = new SendRequestCommand(this);
                }
                return _sendRequestCommand;
            }
        }

        private Dictionary<String, String> _properties;
        public String this[String name]
        {
            get
            {
                return _properties[name];
            }

            set
            {
                _properties[name] = value;
                RaisePropertyChanged(name);
            }
        }

        private RestResponse _returnedRestResponse;
        public RestResponse ReturnedRestResponse
        {
            get
            {
                return _returnedRestResponse;
            }

            set
            {
                _returnedRestResponse = value;
                RaisePropertyChanged(RETURNED_REST_RESPONSE);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string p)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                SyncContext.Post((state) => { handler(this, state as PropertyChangedEventArgs); }, new PropertyChangedEventArgs(p)); 
            }
        }

        public RestActionViewModel()
        {
            _properties = new Dictionary<string, string>()
            {
                {API_VERSION, "v26.0"},
                {OBJECT_TYPE, "Account"},
                {OBJECT_ID, "Id"},
                {EXTERNAL_ID_FIELD, "ExternalIdField"},
                {EXTERNAL_ID, "ExternalId"},
                {FIELD_LIST, "Id,Name"},
                {FIELDS, "{\"Name\":\"acme\"}"},
                {SOQL, "SELECT Id,Name FROM Account"},
                {SOSL, "FIND {acme*}"},
                {REQUEST_PATH, "/services/data/v26.0/chatter/feeds/news/me"},
                {REQUEST_BODY, "Body"},
                {REQUEST_METHOD, "GET"},
            };
        }
    }

    public class SendRequestCommand : ICommand
    {
        private RestActionViewModel _vm;
        public SendRequestCommand(RestActionViewModel vm)
        {
            _vm = vm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(Object parameter)
        {
            return true; // server-side validation only
        }

        public void Execute(Object parameter)
        {
            ClientManager cm = new ClientManager(Config.LoginOptions);
            RestClient rc = cm.GetRestClient();
            if (rc != null)
            {
                RestRequest request = BuildRestRequest();
                rc.SendAsync(request, (response) => { _vm.ReturnedRestResponse = response; });
            }
        }

        private RestRequest BuildRestRequest()
        {
            RestAction restAction = (RestAction)Enum.Parse(typeof(RestAction), _vm[RestActionViewModel.SELECTED_REST_ACTION]);
            switch (restAction)
            {
                case RestAction.VERSIONS:
                    return RestRequest.GetRequestForVersions();
                case RestAction.RESOURCES:
                    return RestRequest.GetRequestForResources(_vm[RestActionViewModel.API_VERSION]);
                case RestAction.DESCRIBE_GLOBAL:
                    return RestRequest.GetRequestForDescribeGlobal(_vm[RestActionViewModel.API_VERSION]);
                case RestAction.METADATA:
                    return RestRequest.GetRequestForMetadata(_vm[RestActionViewModel.API_VERSION], _vm[RestActionViewModel.OBJECT_TYPE]);
                case RestAction.DESCRIBE:
                    return RestRequest.GetRequestForDescribe(_vm[RestActionViewModel.API_VERSION], _vm[RestActionViewModel.OBJECT_TYPE]);
                case RestAction.CREATE:
                    return RestRequest.GetRequestForCreate(_vm[RestActionViewModel.API_VERSION], _vm[RestActionViewModel.OBJECT_TYPE], ParseFieldsValue());
                case RestAction.RETRIEVE:
                    return RestRequest.GetRequestForRetrieve(_vm[RestActionViewModel.API_VERSION], _vm[RestActionViewModel.OBJECT_TYPE], _vm[RestActionViewModel.OBJECT_ID], ParseFieldListValue());
                case RestAction.UPSERT:
                    return RestRequest.GetRequestForUpsert(_vm[RestActionViewModel.API_VERSION], _vm[RestActionViewModel.OBJECT_TYPE], _vm[RestActionViewModel.EXTERNAL_ID_FIELD], _vm[RestActionViewModel.EXTERNAL_ID], ParseFieldsValue());
                case RestAction.UPDATE:
                    return RestRequest.GetRequestForUpdate(_vm[RestActionViewModel.API_VERSION], _vm[RestActionViewModel.OBJECT_TYPE], _vm[RestActionViewModel.OBJECT_ID], ParseFieldsValue());
                case RestAction.DELETE:
                    return RestRequest.GetRequestForDelete(_vm[RestActionViewModel.API_VERSION], _vm[RestActionViewModel.OBJECT_TYPE], _vm[RestActionViewModel.OBJECT_ID]);
                case RestAction.QUERY:
                    return RestRequest.GetRequestForQuery(_vm[RestActionViewModel.API_VERSION], _vm[RestActionViewModel.SOQL]);
                case RestAction.SEARCH:
                    return RestRequest.GetRequestForSearch(_vm[RestActionViewModel.API_VERSION], _vm[RestActionViewModel.SOSL]);
                case RestAction.MANUAL:
                    return BuildManualRestReuqest();
                default:
                    throw new InvalidOperationException();
            }
        }

        private RestRequest BuildManualRestReuqest()
        {
            RestMethod restMethod = (RestMethod)Enum.Parse(typeof(RestMethod), _vm[RestActionViewModel.REQUEST_METHOD], true);
            return new RestRequest(restMethod, _vm[RestActionViewModel.REQUEST_PATH], _vm[RestActionViewModel.REQUEST_BODY], ContentType.JSON);
        }

        private string[] ParseFieldListValue()
        {
            return _vm[RestActionViewModel.FIELD_LIST].Split(',');
        }

        private Dictionary<String, Object> ParseFieldsValue()
        {
            Dictionary<String, Object> result = new Dictionary<String, Object>();
            JObject fieldMap = JObject.Parse(_vm[RestActionViewModel.FIELDS]);
            foreach (var item in fieldMap)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }

    }
}
