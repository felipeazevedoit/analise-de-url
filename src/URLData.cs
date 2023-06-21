using System.Net;

namespace ReadURLsRequestResponse
{
    public class URLData
    {
        public string? Url { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool Ativa { get; set; }
    }
}
