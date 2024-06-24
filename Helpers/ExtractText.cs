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
using Spire.Pdf.Graphics;
using System.Reflection.PortableExecutable;
using Image = System.Drawing.Image;
using PdfDocument = Spire.Pdf.PdfDocument;
using SpireSearchPDFCoordinates.Helpers;

namespace SearchPDFCoordinates
{
    public class TextExtractor
    {
        static string rootDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        static string inputCoordinatesFile = System.IO.Path.Combine(rootDirectory, "Annotations", "annotations.json");
        
        public async Task ReadDocValuesUsingSpire(string documentPath)
        {
            //Spire.Pdf.License.LicenseProvider.SetLicenseKey("License/license.elic.xml");
            #region get the field values from coordinates

            // Read the JSON content from the file
            string jsonText = File.ReadAllText(inputCoordinatesFile);
            List<Annotation_Spire> deserializedAnnotations = JsonConvert.DeserializeObject<List<Annotation_Spire>>(jsonText);



            string filePath = documentPath;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fileStream.CanRead)
                {
                    await TextExtractor.ExtractTextFromRegion(deserializedAnnotations, fileStream);
                }
                else
                {
                    Console.WriteLine("Error: The file stream is not open for reading.");
                }
            }


            #endregion
        }
        private static async Task ExtractTextFromRegion(List<Annotation_Spire> deserializedAnnotations, FileStream reader)
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

                //PdfDocument modifiedDocument = new Spire.Pdf.PdfDocument();
                //if (isScanned)
                //{
                //    modifiedDocument = AddRectangleToMemoryStream(doc, deserializedAnnotations);
                //}

                List<string> output = new List<string>();
                foreach (Annotation_Spire annot in deserializedAnnotations)
                {
                    #region original implementation

                    Spire.Pdf.PdfPageBase currentPage = doc.Pages[annot.Page];
                    //currentPage.Rotation = PdfPageRotateAngle.RotateAngle0;
                    
                    //if (isScanned)
                    //{
                    //    currentPage = modifiedDocument.Pages[annot.Page];
                    //}

                    Spire.Pdf.Texts.PdfTextExtractor textExtractor = new Spire.Pdf.Texts.PdfTextExtractor(currentPage);
                    PdfTextExtractOptions extractOptions = new PdfTextExtractOptions();
                    extractOptions.ExtractArea =
                        new RectangleF(annot.X, annot.Y, annot.Width, annot.Height);
                    string text = textExtractor.ExtractText(extractOptions);
                   
                    #endregion


                    // Convert the region to an image and send to azure AI to fetch the content
                    //if (string.IsNullOrEmpty(text))
                    //{
                        RegionToImage regionToImage = new RegionToImage();
                        Bitmap bitmap = regionToImage.FetchImageFromRectangle(doc,annot);

                        // Get response from azure AI
                        AzureVisionAI azureVisionAI = new AzureVisionAI();
                        text = await azureVisionAI.RunProcess(bitmap);
                    //}

                    Console.WriteLine("Field: {0} | Value:{1}", annot.Name, text);

                    #region get an image of the region

                    //// Create a single image from a pdf document
                    //Image image = doc.SaveAsImage(0, PdfImageType.Bitmap, (int)currentPage.ActualSize.Width, (int)currentPage.ActualSize.Height);
                    //image.Save(@"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\El Paso\cropped_image.svg");


                    //// Crop the region and add to the new document
                    //PdfPageBase croppedPage = currentPage.Document.Pages.Add(currentPage.Size, new PdfMargins(0));
                    //currentPage.CreateTemplate().Draw(croppedPage, new RectangleF(annot.X, annot.Y, annot.Width, annot.Height));
                    //currentPage.Document.Save(@"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\El Paso\cropped_image.svg", FileFormat.SVG);


                    ////RectangleF rect = new RectangleF(annot.X, annot.Y, annot.Width, annot.Height);
                    ////Bitmap bitmap = new Bitmap((int)annot.Width, (int)annot.Height);

                    ////using (Graphics graphics = Graphics.FromImage(bitmap))
                    ////{
                    ////    graphics.Clear(Color.White); 
                    ////    currentPage.CreateTemplate().Draw(currentPage, rect);
                    ////    bitmap.Save(@"C:\Users\carlo\Downloads\output_image.png", System.Drawing.Imaging.ImageFormat.Png);
                    ////    bitmap.Dispose();
                    ////}

                    ////currentPage.ExtractImages();
                    ////System.Drawing.Image image = currentPage.ConvertToImage(new RectangleF(annot.X, annot.Y, annot.Width, annot.Height));
                    ////image.Save($"captured_region_page_{annot.Page}.png", System.Drawing.Imaging.ImageFormat.Png);
                    ////image.Dispose(); 

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
                   
                    float targetX = annotation.X;
                    float targetY = annotation.Y;
                    float targetWidth = annotation.Width;
                    float targetHeight = annotation.Height;


                    PdfPageBase page = doc.Pages[annotation.Page];
                    //page.Rotation = PdfPageRotateAngle.RotateAngle90;
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
