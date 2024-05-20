using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Tesseract;
using University.ConsoleApp.Helper;
using University.ConsoleApp.Models;

namespace University.ConsoleApp.Services;

public class ContentExtractor
{

    public string ReadContent(CustomFileModel file) =>

         Path.GetExtension(file.Path) switch
         {
             FileExtensions.Word => ReadXmlFileContent(file),
             FileExtensions.Pdf => ReadPdfFileContent(file),
             FileExtensions.Text => ReadTextFileContent(file),
             FileExtensions.Png => ReadImageFileContent(file),
             FileExtensions.Jpeg => ReadImageFileContent(file),
             FileExtensions.Jpg => ReadImageFileContent(file),
         };


    public string ReadTextFileContent(CustomFileModel file)
    {
        return string.Join(" ", File.ReadAllText(file.Path).Split().Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()));
    }

    public string ReadPdfFileContent(CustomFileModel file)
    {
        string content = string.Empty;

        using (var pdfDocument = new PdfDocument(new PdfReader(file.Path)))
        {
            int totalPages = pdfDocument.GetNumberOfPages();

            for (int i = 1; i <= totalPages; i++) content += PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i));

        }
        var temp = string.Join(" ", content.Split().Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()).ToArray());

        return string.Join(" ", content.Split().Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()).ToArray());
    }

    public string ReadXmlFileContent(CustomFileModel file)
    {
        var content = "";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(file.Path, false))
        {
            var body = doc.MainDocumentPart?.Document.Body;
            foreach (var node in body) if (!string.IsNullOrEmpty(node.InnerText)) content += node.InnerText.ToLower() + " ";
        }
        return content;
    }

    public string ReadImageFileContent(CustomFileModel file)
    {
        try
        {
            using (var engine = new TesseractEngine(AppContext.BaseDirectory, "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(file.Path))
                {
                    using (var page = engine.Process(img)) return string.Join(" ", page.GetText().Split().Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()));
                }
            }
        }
        catch
        {
            return string.Empty;
        }
    }
}
