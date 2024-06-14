using System;
using System.Drawing;
using System.IO;
using Spire.Pdf;
using Spire.Pdf.Texts;
using Spire.Pdf.Fields;
using Spire.Pdf.Widget;
using System.Text;
using System.Collections.Generic;
using Spire.Additions.Xps.Schema;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using SpireSearchPDFCoordinates;
using Newtonsoft.Json;

namespace SearchPDFCoordinates
{
    public static class TextExtractor
    {
        static string rootDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        static string inputCoordinatesFile = System.IO.Path.Combine(rootDirectory, "Annotations", "annotations.json");
        
        public static void ReadDocValuesUsingSpire(string documentPath)
        {
            #region get the field values from coordinates

            // Read the JSON content from the file
            string jsonText = File.ReadAllText(inputCoordinatesFile);
            List<Annotation_Spire> deserializedAnnotations = JsonConvert.DeserializeObject<List<Annotation_Spire>>(jsonText);



            string filePath = documentPath;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fileStream.CanRead)
                {
                    TextExtractor.ExtractTextFromRegion(deserializedAnnotations, fileStream);
                }
                else
                {
                    Console.WriteLine("Error: The file stream is not open for reading.");
                }
            }


            #endregion
        }
        private static void ExtractTextFromRegion(List<Annotation_Spire> deserializedAnnotations, FileStream reader)
        {
            using (Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument())
            {
                doc.LoadFromStream(reader);

                // Check for text layers
                int textLayerCount = 0;
                foreach (PdfPageBase pdfPage in doc.Pages)
                {
                    var extractor = new Spire.Pdf.Texts.PdfTextExtractor(pdfPage);
                    PdfTextExtractOptions extractOption = new PdfTextExtractOptions();
                    textLayerCount += extractor.ExtractText(extractOption).Length;
                    textLayerCount += 0;
                }


                /*
                 * Fewer layers means it's scanned - normally PDF Forms have many layers
                 * To skip over AddRectangleToMemoryStream set the isScanned to false;
                 */

                //var isScanned = false; // uncomment this to skip over the AddRectangleToMemoryStream
                var isScanned = textLayerCount < 10;

                PdfDocument modifiedDocument = new Spire.Pdf.PdfDocument();
                if (isScanned)
                {
                    modifiedDocument = AddRectangleToMemoryStream(doc, deserializedAnnotations);
                }

                List<string> output = new List<string>();
                foreach (Annotation_Spire annot in deserializedAnnotations)
                {
                    #region original implementation

                    Spire.Pdf.PdfPageBase currentPage = doc.Pages[annot.Page];
                    if (isScanned)
                    {
                        currentPage = modifiedDocument.Pages[annot.Page];
                    }

                    Spire.Pdf.Texts.PdfTextExtractor textExtractor = new Spire.Pdf.Texts.PdfTextExtractor(currentPage);
                    PdfTextExtractOptions extractOptions = new PdfTextExtractOptions();
                    extractOptions.ExtractArea =
                        new RectangleF(annot.X, annot.Y, annot.Width, annot.Height);
                    string text = textExtractor.ExtractText(extractOptions);
                    Console.WriteLine(text);

                    #endregion
                }
            }

        }

        private static PdfDocument AddRectangleToMemoryStream(Spire.Pdf.PdfDocument doc, List<Annotation_Spire> deserializedAnnotations)
        {
            try
            {
                //Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                doc.AllowCreateForm = (doc.Form == null) ? true : false;
               

                foreach (Annotation_Spire annotation in deserializedAnnotations)
                {
                    //PdfPageBase page = doc.Pages.Add();
                    Func<float, float, float, float> MapCoordinates = (float sourceX, float sourceY, float sourceHeight) =>
                    {
                        float targetX = sourceX; 
                        float targetY = doc.Pages[annotation.Page].Size.Height - sourceY - sourceHeight; 
                        return targetY;
                    };

                    float targetX = annotation.X;
                    float targetY = MapCoordinates(annotation.X, annotation.Y, annotation.Height);
                    float targetWidth = annotation.Width;
                    float targetHeight = annotation.Height;


                    PdfPageBase page = doc.Pages[annotation.Page];
                    PdfTextBoxField textbox = new PdfTextBoxField(page, annotation.Name);
                    textbox.Bounds = new RectangleF(targetX, targetY, targetWidth, targetHeight);
                    textbox.BorderWidth = 0.75f;
                    textbox.BorderStyle = PdfBorderStyle.Solid;
                    doc.Form.Fields.Add(textbox);
                }


;           
                #region uncomment to view the output file  - this is for testing the document output rectangle values
                string modifiedDocumentOutputPath = System.IO.Path.Combine(rootDirectory, "Documents", "Aransas_modified.pdf");
                doc.SaveToFile(modifiedDocumentOutputPath);
                #endregion

                // Create a new MemoryStream to store the modified PDF
                MemoryStream outputStream = new MemoryStream();

                // Save the modified PDF document to the MemoryStream
                doc.SaveToStream(outputStream);

                // Reset the position of the MemoryStream to the beginning
                outputStream.Position = 0;

                // Return the MemoryStream containing the modified PDF
                return doc;
                
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, e.g., log it or throw it further.
                Console.WriteLine("Error adding rectangle: " + ex.Message);
                return null;
            }
        }

       
    }
}
