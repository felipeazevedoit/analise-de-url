using System.Net;

namespace ReadURLsRequestResponse
{
    public class URLReader
    {
        public List<URLData> Urls { get; set; }
        public List<string> UrlString { get; set; }
        public URLReader(List<string> urlString)
        {
            UrlString = urlString;
            Urls = new List<URLData>();
        }

        public URLReader()
        {
                Urls = new List<URLData>();
        }

        public static string? GetValidUrl(string url)
        {
            if (url == null) return null;

            url = url.Trim();
            url = url.Replace("\r\n", string.Empty);

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "http://" + url;

            return "http://" + url;
        }

        public List<URLData> Read()
        {
            foreach (string url in UrlString)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var urlData = new URLData();

                    try
                    {
                        var httpUrl = GetValidUrl(url);
                        if (httpUrl is null)
                        {
                            urlData.StatusCode = HttpStatusCode.BadRequest;
                            urlData.Active = false;
                            urlData.Url = url;
                            urlData.StatusDescription = "URL Inválida";
                        }
                        else
                        {
                            using var client = new HttpClient();
                            var response = client.GetAsync(httpUrl).Result;

                            urlData.Url = url;
                            urlData.StatusCode = response.StatusCode;
                            urlData.Active = true;
                            urlData.StatusDescription = response.ReasonPhrase;
                        }

                    }
                    catch (Exception ex)
                    {
                        urlData.StatusCode = HttpStatusCode.BadRequest;
                        urlData.Active = false;
                        urlData.Url = url;
                        urlData.StatusDescription = ex.Message;
                    }
                    Urls.Add(urlData);
                }
            }
            return OrderStatusCode(Urls);
        }

        public async Task<List<URLData>> ReadAsync()
        {
            var tasks = new List<Task<URLData>>();

            foreach (string url in UrlString)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    tasks.Add(ProcessUrlAsync(url));
                }
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            var orderedResults = OrderStatusCode(results.ToList());

            return orderedResults;
        }

        public static async Task<URLData> ReadAsync(string url)
        {
            var urlData = new URLData();

            try
            {
                var httpUrl = GetValidUrl(url);
                if (httpUrl is null)
                {
                    urlData.StatusCode = HttpStatusCode.BadRequest;
                    urlData.Active = false;
                    urlData.Url = url;
                    urlData.StatusDescription = "URL Inválida";
                }
                else
                {
                    using var client = new HttpClient();
                    var response = await client.GetAsync(httpUrl);

                    urlData.Url = url;
                    urlData.StatusCode = response.StatusCode;
                    urlData.Active = true;
                    urlData.StatusDescription = response.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                urlData.StatusCode = HttpStatusCode.BadRequest;
                urlData.Active = false;
                urlData.Url = url;
                urlData.StatusDescription = ex.Message;
            }

            return urlData;
        }

        private static async Task<URLData> ProcessUrlAsync(string url)
        {
            var urlData = new URLData();

            try
            {
                var httpUrl = GetValidUrl(url);
                if (httpUrl is null)
                {
                    urlData.StatusCode = HttpStatusCode.BadRequest;
                    urlData.Active = false;
                    urlData.Url = url;
                    urlData.StatusDescription = "URL Inválida";
                }
                else
                {
                    using var client = new HttpClient();
                    var response = await client.GetAsync(httpUrl).ConfigureAwait(false);

                    urlData.Url = url;
                    urlData.StatusCode = response.StatusCode;
                    urlData.Active = true;
                    urlData.StatusDescription = response.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {
                urlData.StatusCode = HttpStatusCode.BadRequest;
                urlData.Active = false;
                urlData.Url = url;
                urlData.StatusDescription = ex.Message;
            }

            return urlData;
        }

      
        public static List<URLData> OrderStatusCode(List<URLData> urls)
        {
            urls.Sort((x, y) => x.StatusCode.CompareTo(y.StatusCode));
            return urls;
        }

    }
}
