// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Utils.Errors
{
    /// <summary>
    /// Exception thrown when an HTTP request fails.
    /// </summary>
    public class HttpResponseException : Exception
    {
        public HttpResponseException(int statusCode, object? value = null) => (StatusCode, Value) = (statusCode, value);

        /// <summary>
        /// HTTP Status code associated with this <see cref="Exception"/>
        /// </summary>
        public int StatusCode   { get; }

        /// <summary>
        /// Value associated with this <see cref="Exception"/>
        /// </summary>
        public object? Value    { get; }
    }
}
