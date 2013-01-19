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
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Salesforce.WinSDK.Net;
using System;
using System.Collections.Generic;
using System.Net;

namespace Salesforce.WinSDK.Rest
{
    [TestClass]
    public class RestRequestTest
    {
    	private const String TEST_API_VERSION = "v99.0";
    	private const String TEST_OBJECT_TYPE = "testObjectType";
    	private const String TEST_OBJECT_ID = "testObjectId";
    	private const String TEST_EXTERNAL_ID_FIELD = "testExternalIdField";
    	private const String TEST_EXTERNAL_ID = "testExternalId";
    	private const String TEST_QUERY = "testQuery";
    	private const String TEST_SEARCH = "testSearch";
    	private const String TEST_FIELDS_STRING = "{\"fieldX\":\"value with spaces\",\"name\":\"testAccount\"}";
    	private const String TEST_FIELDS_LIST_STRING = "name,fieldX";
        private const String FORM_URLENCODED = "application/x-www-form-urlencoded";
        private const String APPLICATION_JSON = "application/json"; 

        private String[] TEST_FIELDS_LIST = new String[]{"name", "fieldX"};    	
    	private Dictionary<String, Object> TEST_FIELDS = new Dictionary<String, Object> {{"fieldX", "value with spaces"},{"name", "testAccount"}};
    	
        [TestMethod]
        public void testGetRequestForVersions() 
        {
    		RestRequest request = RestRequest.getRequestForVersions();
    		Assert.AreEqual(Method.GET, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.NONE, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/", request.Path, "Wrong path");
    		Assert.IsNull(request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    	
        [TestMethod]
    	public void testGetRequestForResources() 
        {
    		RestRequest request = RestRequest.getRequestForResources(TEST_API_VERSION);
    		Assert.AreEqual(Method.GET, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.NONE, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/", request.Path, "Wrong path");
    		Assert.IsNull(request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    
    	
        [TestMethod]
    	public void testGetRequestForDescribeGlobal() 
        {
    		RestRequest request = RestRequest.getRequestForDescribeGlobal(TEST_API_VERSION);
    		Assert.AreEqual(Method.GET, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.NONE, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/sobjects/", request.Path, "Wrong path");
    		Assert.IsNull(request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    
    	
        [TestMethod]
    	public void testGetRequestForMetadata() 
        {
    		RestRequest request = RestRequest.getRequestForMetadata(TEST_API_VERSION, TEST_OBJECT_TYPE);
    		Assert.AreEqual(Method.GET, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.NONE, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/sobjects/" + TEST_OBJECT_TYPE + "/", request.Path, "Wrong path");
    		Assert.IsNull(request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    
        [TestMethod]
        public void testGetRequestForDescribe() 
        {
    		RestRequest request = RestRequest.getRequestForDescribe(TEST_API_VERSION, TEST_OBJECT_TYPE);
    		Assert.AreEqual(Method.GET, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.NONE, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/sobjects/" + TEST_OBJECT_TYPE + "/describe/", request.Path, "Wrong path");
    		Assert.IsNull(request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    
    	
        [TestMethod]
        public void testGetRequestForCreate() 
        {
    		RestRequest request = RestRequest.getRequestForCreate(TEST_API_VERSION, TEST_OBJECT_TYPE, TEST_FIELDS);
    		Assert.AreEqual(Method.POST, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.JSON, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/sobjects/" + TEST_OBJECT_TYPE, request.Path, "Wrong path");
    		Assert.AreEqual(TEST_FIELDS_STRING, request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    	
        [TestMethod]
    	public void testGetRequestForRetrieve() 
        {
    		RestRequest request = RestRequest.getRequestForRetrieve(TEST_API_VERSION, TEST_OBJECT_TYPE, TEST_OBJECT_ID, TEST_FIELDS_LIST);
    		Assert.AreEqual(Method.GET, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.NONE, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/sobjects/" + TEST_OBJECT_TYPE + "/" + TEST_OBJECT_ID + "?fields=" + TEST_FIELDS_LIST_STRING, request.Path, "Wrong path");
    		Assert.IsNull(request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    
        [TestMethod]
    	public void testGetRequestForUpdate() 
        {
    		RestRequest request = RestRequest.getRequestForUpdate(TEST_API_VERSION, TEST_OBJECT_TYPE, TEST_OBJECT_ID, TEST_FIELDS);
    		Assert.AreEqual(Method.PATCH, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.JSON, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/sobjects/" + TEST_OBJECT_TYPE + "/" + TEST_OBJECT_ID, request.Path, "Wrong path");
    		Assert.AreEqual(TEST_FIELDS_STRING, request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    	
        [TestMethod]
    	public void testGetRequestForUpsert()
        {
    		RestRequest request = RestRequest.getRequestForUpsert(TEST_API_VERSION, TEST_OBJECT_TYPE, TEST_EXTERNAL_ID_FIELD, TEST_EXTERNAL_ID, TEST_FIELDS);
    		Assert.AreEqual(Method.PATCH, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.JSON, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/sobjects/" + TEST_OBJECT_TYPE + "/" + TEST_EXTERNAL_ID_FIELD + "/" + TEST_EXTERNAL_ID, request.Path, "Wrong path");
    		Assert.AreEqual(TEST_FIELDS_STRING, request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    
        [TestMethod]
    	public void testGetRequestForDelete() 
        {
    		RestRequest request = RestRequest.getRequestForDelete(TEST_API_VERSION, TEST_OBJECT_TYPE, TEST_OBJECT_ID);
    		Assert.AreEqual(Method.DELETE, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.NONE, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/sobjects/" + TEST_OBJECT_TYPE + "/" + TEST_OBJECT_ID, request.Path, "Wrong path");
    		Assert.IsNull(request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    	
    	public void testGetRequestForQuery() 
        {
    		RestRequest request = RestRequest.getRequestForQuery(TEST_API_VERSION, TEST_QUERY);
    		Assert.AreEqual(Method.GET, request.Method, "Wrong method");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/query?q=" + TEST_QUERY, request.Path, "Wrong path");
    		Assert.IsNull(request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    
        [TestMethod]
    	public void testGetRequestForSeach()
        {
    		RestRequest request = RestRequest.getRequestForSearch(TEST_API_VERSION, TEST_SEARCH);
    		Assert.AreEqual(Method.GET, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.NONE, request.ContentType, "Wrong content type");
    		Assert.AreEqual("/services/data/" + TEST_API_VERSION + "/search?q=" + TEST_SEARCH, request.Path, "Wrong path");
    		Assert.IsNull(request.Body, "Wrong request body");
    		Assert.IsNull(request.AdditionalHeaders, "Wrong additional headers");
    	}
    
        [TestMethod]
    	public void testAdditionalHeaders() 
        {
    		Dictionary<String, String> headers = new Dictionary<String, String>() {{"X-Foo", "RestRequestName"}};
    		RestRequest request = new RestRequest(Method.GET, "/my/foo/", null, ContentType.NONE, headers);
    		Assert.AreEqual(Method.GET, request.Method, "Wrong method");
            Assert.AreEqual(ContentType.NONE, request.ContentType, "Wrong content type");
            Assert.AreEqual("/my/foo/", request.Path, "Wrong path");
            Assert.IsNull(request.Body, "Wrong body");
            Assert.AreEqual(headers, request.AdditionalHeaders, "Wrong headers");
    	}
    }
}