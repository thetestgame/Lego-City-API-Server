// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Utils.Errors
{
    using System.Net;

    public class HttpObjectNotFoundException : HttpResponseException
    {
        public HttpObjectNotFoundException(string message) : base((int) HttpStatusCode.NotFound, message) {}
    }
}
