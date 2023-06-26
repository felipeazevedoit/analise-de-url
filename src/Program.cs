using ReadURLsRequestResponse;
using System;
using System.Threading;

class Program
{


    static void Main()
    {
        string directory = GetDirectory();
        string fileName = GetFileName();

        while (!ValidateFile(directory, fileName))
        {
            Console.WriteLine("O arquivo fornecido não é válido. Por favor, forneça o diretório e o nome do arquivo novamente.");
            directory = GetDirectory();
            fileName = GetFileName();
        }

        Console.WriteLine("Diretório do arquivo: " + directory);
        Console.WriteLine("Nome do arquivo: " + fileName);

        var patch = Path.Combine(directory, fileName);
        var FilePatch = new FileData(patch);
        var urls = FilePatch.ReadURLsFromFile();

        if (urls.Count() != 0)
        {
            var readers = new URLReader(urls).Read();

            Console.WriteLine("Total de URL's Encontradaas no arquivo:" + urls.Count());

            if (readers.Count != 0)
            {

                Console.WriteLine("Gerando Relatórios");
                FileData.GenerateTablePDF(URLData.FilterDuplicateURLs(readers), directory);

              
                Console.WriteLine("\nMétodo concluído.");

              

                PrintService.PrintUrls(readers);
                PrintService.PrintSumForStatusCode(readers);
                PrintService.PrintUrlsErrors(readers);
            }
            Console.WriteLine("Pressione qualquer tecla para encerrar...");
            Console.ReadKey();
        }
    }

    private static string GetDirectory()
    {
        Console.Write("Digite o diretório do arquivo com as URLs: ");
        string directory = Console.ReadLine().Trim();

        return directory;
    }

    private static string GetFileName()
    {
        Console.Write("Digite o nome do arquivo: ");
        string fileName = Console.ReadLine().Trim();

        return fileName;
    }
    private static bool ValidateFile(string directory, string fileName)
    {
        string filePath = Path.Combine(directory, fileName);

        if (!File.Exists(filePath))
        {
            return false;
        }

        if (Path.GetExtension(fileName) != ".txt")
        {
            return false;
        }

        string fileContent = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            return false;
        }

        string[] urls = fileContent.Split(',');

        if (urls.Length == 0)
        {
            return false;
        }

        return true;
    }

  

    public static void UpdateLoading(int progress)
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write($"Progresso: {progress}%");
    }
}