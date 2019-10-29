namespace Netsoft.Core.Model
{
    using System.Collections.Generic;
    using System.Net;

    public class ApiResponse<TBody>
    {
        public HttpStatusCode StatusCode { get; set; }

        public IEnumerable<ApiResponseHeader> Headers { get; set; }

        public TBody Body { get; set; }
    }
}
