using System.Net;

namespace ReadURLsRequestResponse
{
    public static class PrintService
    {
        public static void PrintUrls(List<URLData> urls)
        {
            foreach (var urlData in urls)
            {
                Console.WriteLine($"URL: {urlData.Url} | StatusCode: {urlData.StatusCode} | Ativa: {urlData.Ativa}");
            }
        }

        public static void PrintSumForStatusCode(List<URLData> urls)
        {
            var statusCodeCount = new Dictionary<HttpStatusCode, int>();

            Console.WriteLine("\nTotal de URLs por StatusCode:");

            foreach (URLData urlData in urls)
            {
                if (statusCodeCount.ContainsKey(urlData.StatusCode))
                    statusCodeCount[urlData.StatusCode]++;
                else
                    statusCodeCount[urlData.StatusCode] = 1;
            }

            foreach (var kvp in statusCodeCount)
            {
                Console.WriteLine($"StatusCode: {kvp.Key} | Total: {kvp.Value}");
            }

        }

        public static void PrintUrlsErrors(List<URLData> urls)
        {
            Console.WriteLine("\nURLs com erro:");

            foreach (URLData urlData in urls)
            {
                if (urlData.StatusCode != HttpStatusCode.OK)
                    Console.WriteLine($"URL: {urlData.Url} | StatusCode: {urlData.StatusCode} | Ativa: {urlData.Ativa}");
            }
        }

    }

}

