using iTextSharp.text.pdf;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using iTextSharp.text;
using DocumentFormat.OpenXml.Bibliography;

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
                List<string> urls = new List<string>();
                foreach (var item in urlArray)
                {
                    urls.Add(item.Replace(@"\r\", "").Replace(" ","").Replace(@"\","").Replace('"', '\'').Replace(",",""));
                }

                URLs.AddRange(urlArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao ler o arquivo: " + ex.Message);
            }
            return URLs;
        }

        public static void GenerateTablePDF(List<URLData> dataList, string outputPath)
        {
            var document = new Document();
            document.SetPageSize(PageSize.A4);
            document.SetPageSize(PageSize.A4.Rotate());

            string fullName = Path.Combine(outputPath, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pdf");
            var writer = PdfWriter.GetInstance(document, new FileStream(fullName, FileMode.Create));

            document.Open();

            // Criação da tabela com 4 colunas
            var table = new PdfPTable(4);

            // Adiciona o cabeçalho da tabela
            AddCell(table, "URL", true);
            AddCell(table, "Status Code", true);
            AddCell(table, "Status Description", true);
            AddCell(table, "Active", true);

            // Preenche a tabela com os dados da lista de objetos usando processamento paralelo
            Parallel.ForEach(dataList, data =>
            {

                Console.WriteLine(data.Url + "    " + data.StatusCode.ToString());
                
                AddCell(table, data.Url ?? "", false);
                AddCell(table, data.StatusCode.ToString(), false);
                AddCell(table, data.StatusDescription ?? "", false);
                AddCell(table, data.Active.ToString(), false);
            });

            // Adiciona a tabela ao documento
            document.Add(table);

            // Fechamento do documento
            document.Close();
        }

     

        public static void AddCell(PdfPTable table, string content, bool isHeader)
        {
            // Cria a célula com estilo diferente para o cabeçalho
            PdfPCell cell = new PdfPCell(new Phrase(content));
            cell.Padding = 5;
            cell.HorizontalAlignment = isHeader ? Element.ALIGN_CENTER : Element.ALIGN_LEFT;
            cell.BackgroundColor = isHeader ? BaseColor.LIGHT_GRAY : BaseColor.WHITE;

            // Adiciona a célula à tabela
            table.AddCell(cell);
        }

        public void WriteExcelFiles(List<URLData> dataList, string outputDirectory)
        {
            // Agrupa os objetos por StatusCode
            Dictionary<HttpStatusCode, List<URLData>> dataByStatusCode = new Dictionary<HttpStatusCode, List<URLData>>();

            foreach (URLData data in dataList)
            {
                if (!dataByStatusCode.ContainsKey(data.StatusCode))
                {
                    dataByStatusCode[data.StatusCode] = new List<URLData>();
                }

                dataByStatusCode[data.StatusCode].Add(data);
            }

            Directory.CreateDirectory(outputDirectory);

            // Escreve um arquivo do Excel para cada StatusCode
            foreach (KeyValuePair<HttpStatusCode, List<URLData>> kvp in dataByStatusCode)
            {
                HttpStatusCode statusCode = kvp.Key;
                List<URLData> statusDataList = kvp.Value;

                // Criação do nome do arquivo com o StatusCode e a data/hora atual
                string fileName = $"{statusCode}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                string filePath = Path.Combine(outputDirectory, fileName);

                // Criação do arquivo do Excel
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Dados" };
                    sheets.Append(sheet);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Escreve os cabeçalhos das colunas
                    Row headerRow = new Row();
                    headerRow.Append(CreateCell("URL"));
                    headerRow.Append(CreateCell("Status Code"));
                    headerRow.Append(CreateCell("Status Description"));
                    headerRow.Append(CreateCell("Active"));
                    sheetData.Append(headerRow);

                    // Escreve os dados na planilha
                    foreach (URLData data in statusDataList)
                    {
                        Row dataRow = new Row();
                        dataRow.Append(CreateCell(data.Url ?? ""));
                        dataRow.Append(CreateCell(((int)data.StatusCode).ToString()));
                        dataRow.Append(CreateCell(data.StatusDescription ?? ""));
                        dataRow.Append(CreateCell(data.Active.ToString()));
                        sheetData.Append(dataRow);
                    }

                    worksheetPart.Worksheet.Save();
                }
            }
        }

        private Cell CreateCell(string cellValue)
        {
            return new Cell(new InlineString(cellValue));
        }

    }
}
