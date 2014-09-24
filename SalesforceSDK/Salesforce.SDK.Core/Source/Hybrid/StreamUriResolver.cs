/*
 * Copyright (c) 2014, salesforce.com, inc.
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

using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web;

namespace Salesforce.SDK.Hybrid
{
    public sealed class StreamUriResolver : IUriToStreamResolver
    {
        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            string host = uri.Host;
            int delimiter = host.LastIndexOf('_');
            string encodedContentId = host.Substring(delimiter + 1);
            IBuffer buffer = CryptographicBuffer.DecodeFromHexString(encodedContentId);

            string contentIdentifier = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffer);
            string relativePath = uri.PathAndQuery;

            var appDataUri = new Uri("ms-appx:///" + contentIdentifier + relativePath);

            return GetFileStreamFromApplicationUriAsync(appDataUri).AsAsyncOperation();
        }

        private async Task<IInputStream> GetFileStreamFromApplicationUriAsync(Uri uri)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            if (file == null)
                return null;
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            return stream;
        }
    }
}