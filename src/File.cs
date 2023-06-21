namespace ReadURLsRequestResponse
{
    public class FileData
    {
        public string FilePath { get; set; }
        public List<string> URLs { get; set; }

        public FileData(string filePath)
        {
            FilePath = filePath;
            URLs = new List<string>();
        }

        public List<string> ReadURLsFromFile()
        {
            try
            {
                string fileContent = System.IO.File.ReadAllText(FilePath);
                string[] urlArray = fileContent.Split(',');

                URLs.AddRange(urlArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao ler o arquivo: " + ex.Message);
            }
            return URLs;
        }
    }
}
