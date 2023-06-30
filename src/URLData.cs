using System.Net;
using System.Text.RegularExpressions;

namespace ReadURLsRequestResponse
{
    public class URLData
    {
        public string? Url { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? StatusDescription { get; set; }
        public bool Active { get; set; }



        public static List<URLData> FilterDuplicateURLs(List<URLData> dataList)
        {
            HashSet<string> uniqueUrls = new HashSet<string>();
            List<URLData> filteredList = new List<URLData>();

            foreach (var data in dataList)
            {
                if (!string.IsNullOrEmpty(data.Url) && !uniqueUrls.Contains(data.Url))
                {
                    data.Url = Regex.Replace(data.Url, @"\r|\n|\t", string.Empty);
                    uniqueUrls.Add(data.Url);
                    filteredList.Add(data);
                }
            }

            
            return filteredList;
        }

    }
}
