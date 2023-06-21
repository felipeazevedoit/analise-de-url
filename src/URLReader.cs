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

        public List<URLData> Read()
        {
            foreach (string url in UrlString)
            {
                if (url != "")
                {
                    var urlData = new URLData();

                    try
                    {
                        var request = (HttpWebRequest)WebRequest.Create(url);
                        var response = (HttpWebResponse)request.GetResponse();

                        urlData.Url = url;
                        urlData.StatusCode = response.StatusCode;
                        urlData.Ativa = true;

                        response.Close();
                    }
                    catch (WebException ex)
                    {
                        HttpWebResponse errorResponse = (HttpWebResponse)ex.Response;

                        if (errorResponse != null)
                        {
                            urlData.StatusCode = errorResponse.StatusCode;
                        }
                        else
                        {
                            urlData.StatusCode = HttpStatusCode.BadRequest;
                        }

                        urlData.Ativa = false;
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
