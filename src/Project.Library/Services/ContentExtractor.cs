using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using University.Project.Library.Helper;
using University.Project.Library.Models;

namespace University.Project.Library.Services;

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
        return string.Join(" ", File.ReadAllText(file.Path).Replace("\n", string.Empty).Split());
    }

    public string ReadPdfFileContent(CustomFileModel file)
    {
        var content = "";

        using (var pdfDocument = new PdfDocument(new PdfReader(file.Path)))
        {
            int totalPages = pdfDocument.GetNumberOfPages();

            for (int i = 1; i <= totalPages; i++) content += PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i));

        }

        return string.Join(" ", content.Split().Where(x => !string.IsNullOrEmpty(x)).ToArray());
    }

    public string ReadXmlFileContent(CustomFileModel file)
    {
        var content = "";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(file.Path, false))
        {
            var body = doc.MainDocumentPart?.Document.Body;
            foreach (var node in body) if (!string.IsNullOrEmpty(node.InnerText)) content += node.InnerText + "\n";
        }
        return content;
    }

    public string ReadImageFileContent(CustomFileModel file)
    {
        var content = PythonInterPreter.ReadImageContent(file.Path);
        return string.Join(" ", content.Where(x => !string.IsNullOrEmpty(x)).ToArray());
    }
}
