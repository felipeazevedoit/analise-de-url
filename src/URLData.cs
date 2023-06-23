using System.Net;

namespace ReadURLsRequestResponse
{
    public class URLData
    {
        public string? Url { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? StatusDescription { get; set; }
        public bool Active { get; set; }

    }
}
