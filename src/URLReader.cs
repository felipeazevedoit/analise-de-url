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
                if (url != "")
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

                            var responseRequest = WebRequest.Create(httpUrl).GetResponse();
                            if (responseRequest is not null)
                            {
                                HttpWebResponse response = responseRequest as HttpWebResponse;
                                urlData.Url = url;
                                urlData.StatusCode = response.StatusCode;
                                urlData.Active = true;
                                urlData.StatusDescription = response.StatusDescription;
                                response.Close();
                            }
                            else
                            {
                                urlData.StatusCode = HttpStatusCode.BadRequest;

                                urlData.Active = false;
                                urlData.Url = url;
                                urlData.StatusDescription = "BadRequest";
                            }


                        }

                    }
                    catch (WebException ex)
                    {
                        HttpWebResponse errorResponse = (HttpWebResponse)ex.Response;

                        if (errorResponse != null)
                            urlData.StatusCode = errorResponse.StatusCode;
                        else
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

        public List<URLData> OrderStatusCode(List<URLData> urls)
        {
            urls.Sort((x, y) => x.StatusCode.CompareTo(y.StatusCode));
            return urls;
        }

    }
}
