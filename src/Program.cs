using ReadURLsRequestResponse;

class Program
{


    static void Main()
    {
        var patch = @"C:\\url\lista.txt";
        var urls = new FileData(patch).ReadURLsFromFile();
        if (urls.Count() != 0)
        {
            var readers = new URLReader(urls).Read();

            Console.WriteLine("Total de URL's Encontradaas no arquivo:" + urls.Count());


            if (readers.Count != 0)
            {
                PrintService.PrintUrls(readers);
                PrintService.PrintSumForStatusCode(readers);
                PrintService.PrintUrlsErrors(readers);
            }

        }
    }
}