using ReadURLsRequestResponse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        string filePath = GetFilePath();

        while (!ValidateFile(filePath))
        {
            Console.WriteLine("O arquivo fornecido não é válido. Por favor, forneça o caminho do arquivo novamente.");
            filePath = GetFilePath();
        }

        Console.WriteLine("Caminho do arquivo: " + filePath);

        List<URLData> urlDataList = await ProcessURLsFromFileAsync(filePath);

        if (urlDataList.Count != 0)
        {
            Console.WriteLine("Total de URLs encontradas no arquivo: " + urlDataList.Count);
            Console.WriteLine("Gerando relatórios...");

            FileData.GenerateTablePDF(URLData.FilterDuplicateURLs(urlDataList), Path.GetDirectoryName(filePath));

            Console.WriteLine("\nMétodo concluído.");

            PrintService.PrintUrls(urlDataList);
            PrintService.PrintSumForStatusCode(urlDataList);
            PrintService.PrintUrlsErrors(urlDataList);
        }

        Console.WriteLine("Pressione qualquer tecla para encerrar...");
        Console.ReadKey();
    }

    private static string GetFilePath()
    {
        Console.Write("Digite o caminho do arquivo com as URLs: ");
        string filePath = Console.ReadLine().Trim();

        return filePath;
    }

    private static bool ValidateFile(string filePath)
    {
        if (!File.Exists(filePath) || Path.GetExtension(filePath) != ".txt")
            return false;

        string fileContent = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(fileContent))
            return false;

        string[] urls = fileContent.Split(',');

        if (urls.Length == 0)
            return false;

        return true;
    }

    private static async Task<List<URLData>> ProcessURLsFromFileAsync(string filePath)
    {
        List<URLData> urlDataList = new();
        string[] urls = File.ReadAllText(filePath).Split(',');
        for (int i = 0; i < urls.Length; i++)
            urls[i] = urls[i].Replace("\r", string.Empty);

        await Task.WhenAll(urls.Where(url => !string.IsNullOrWhiteSpace(url)).Select(async url =>
        {
            URLData urlData = await URLReader.ReadAsync(url);
            lock (urlDataList)
            {
                urlDataList.Add(urlData);
            }
        }));

        return urlDataList;
    }
}