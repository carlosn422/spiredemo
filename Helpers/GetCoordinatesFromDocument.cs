using Spire.Pdf.Fields;
using Spire.Pdf.Widget;
using Newtonsoft.Json;

namespace SpireSearchPDFCoordinates
{
    public static class GetCoordinatesFromDocument
    {
        static string rootDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        static string outputCoordinatesFile = Path.Combine(rootDirectory, "Annotations", "annotations.json");

        public static void UsingSpireExtractAnnotationsCoordinates(string documentPath)
        {

            try
            {
                // List to store annotation properties
                List<object> annotations = new List<object>();
                List<object> formFields = new List<object>();


                // Load the PDF document
                Spire.Pdf.PdfDocument document = new Spire.Pdf.PdfDocument();
                //document.LoadFromFile(documentPath);

                using (FileStream fileStream = new FileStream(documentPath, FileMode.Open, FileAccess.Read))
                {
                    document.LoadFromStream(fileStream);

                    // Iterate through each page of the document -- Removing the for loop since document.Pages is not working
                    for (int pageNumber = 0; pageNumber < document.Pages.Count; pageNumber++)
                    {

                        // Get the page
                        //var page = document.Pages[pageNumber];

                        // Get the widget from the page
                        //PdfFormWidget widget = page.Document.Form as PdfFormWidget;
                        PdfFormWidget widget = document.Form as PdfFormWidget;

                        // Go through the list of widgets
                        for (int i = 0; i < widget.FieldsWidget.List.Count; i++)
                        {
                            var field = widget.FieldsWidget.List[i] as PdfField;

                            if (document.Pages.IndexOf(field.Page) == pageNumber)
                            {
                                var fieldObj = new Annotation_Spire
                                {
                                    Name = ((Spire.Pdf.Widget.PdfStyledFieldWidget)field).Name,
                                    Page = pageNumber,
                                    X = ((Spire.Pdf.Widget.PdfStyledFieldWidget)field).Bounds.X,
                                    Y = ((Spire.Pdf.Widget.PdfStyledFieldWidget)field).Bounds.Y,
                                    Width = ((Spire.Pdf.Widget.PdfStyledFieldWidget)field).Bounds.Width,
                                    Height = ((Spire.Pdf.Widget.PdfStyledFieldWidget)field).Bounds.Height,

                                };

                                formFields.Add(fieldObj);
                            }

                        }
                    }
                }

                // Serialize the list of annotations to JSON
                string json = JsonConvert.SerializeObject(formFields, Formatting.Indented);

                // Write JSON to a file
                //string outputPath = Path.Combine(Path.GetDirectoryName(documentPath), "annotations.json");
                File.WriteAllText(outputCoordinatesFile, json);
                Console.WriteLine("Annotations exported to annotations.json.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error extracting annotations: " + ex.Message);
            }
        }
    }
}
