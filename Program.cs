using SearchPDFCoordinates;
using SpireSearchPDFCoordinates.Helpers;

namespace SpireSearchPDFCoordinates
{
    using System;
    using System.Drawing.Imaging;
    using System.Drawing;
    using System.IO;
    using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

    /// <summary>
    /// The purpose of the app, is to read the getCoordinatesFromDocumentPath, create the annotations.json file
    /// then, read the annotations.json file to fetch the content from getTextFromDocumentPath pdf document.
    /// If the document is scanned, then we add the textfields in order to attempt to read the document again
    /// and fetch the values.
    ///
    ///
    /// Problem: The returned text is empty and if we add textfields, they are in the wrong location. 
    /// </summary>

    class Program
    {
        static string rootDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

        //Not Working Documents - These documents are working properly
        static string getCoordinatesFromDocumentPath = Path.Combine(rootDirectory, "Documents", "Aransas_template.pdf");
        static string getTextFromDocumentPath = Path.Combine(rootDirectory, "Documents", "Aransas - Copy.pdf");

        //Working Documents - These documents are working properly
        //static string getCoordinatesFromDocumentPath = Path.Combine(rootDirectory, "Documents/Working", "R163775_WD_Template.pdf");
        //static string getTextFromDocumentPath = Path.Combine(rootDirectory, "Documents/Working", "R163775 WD.pdf");

        ////NOT Working Documents - These documents are working properly
        //static string getCoordinatesFromDocumentPath = Path.Combine(rootDirectory, "Documents/NewFilesToTest", "Dallas_Template.pdf");
        //static string getTextFromDocumentPath = Path.Combine(rootDirectory, "Documents/NewFilesToTest", "Dallas.pdf");

        ////Working Documents - These documents are working properly
        //static string getCoordinatesFromDocumentPath = Path.Combine(rootDirectory, "Documents/NewFilesToTest", "El Paso_Template.pdf");
        //static string getTextFromDocumentPath = Path.Combine(rootDirectory, "Documents/NewFilesToTest", "El Paso.pdf");

        //Working Documents - These documents are working properly
        //static string getCoordinatesFromDocumentPath = Path.Combine(rootDirectory, "Documents/NewFilesToTest", "Kaufman - 1_Template.pdf");
        //static string getTextFromDocumentPath = Path.Combine(rootDirectory, "Documents/NewFilesToTest", "Kaufman - 1.pdf");

        ////Noav Files
        //static string getCoordinatesFromDocumentPath = Path.Combine(rootDirectory, "Documents", "00000259-E291-455F-A3F8-D2F542BA74FE_NOAV.pdf");
        //static string getTextFromDocumentPath = Path.Combine(rootDirectory, "Documents/", "El Paso.pdf");


        static async Task Main(string[] args)
        {

           
            /*
             * Get the coordinates from the file and write them to the coordinates json file.
             * This is later used to fetch the content from the file
             */
            GetCoordinatesFromDocument.UsingSpireExtractAnnotationsCoordinates(getCoordinatesFromDocumentPath);

            /*
             * Fetch the text using the coordinates.json file
             */
            TextExtractor.ReadDocValuesUsingSpire(getTextFromDocumentPath);

            #region read image
            //Spire.Pdf.PdfDocument pdf = new Spire.Pdf.PdfDocument();

            //pdf.LoadFromFile(getCoordinatesFromDocumentPath);

            //for (int i = 0; i < pdf.Pages.Count; i++)
            //{

            //    String fileName = String.Format("{0}ToPNG-img-{0}.png", i);


            //    var coordinates = new[]
            //    {
            //        new { Page = 0, X = 412.799f, Y = 320.29f, Width = 150.0f, Height = 22.0f },
            //    };

            //    using (Image image = pdf.SaveAsImage(i, 300, 300))

            //    {

            //        image.Save(fileName, ImageFormat.Png);

            //        Bitmap bitmap = new Bitmap(image.Width / 2, 1000);

            //        using (Graphics graphics = Graphics.FromImage(bitmap))
            //        {
            //            //RectangleF rect = new RectangleF(coordinates[0].X, coordinates[0].Y, coordinates[0].Width, coordinates[0].Height);
            //            //using (Pen pen = new Pen(Color.Red, 2))
            //            //{
            //            //    graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            //            //}

            //            //graphics.DrawImage(image, new Rectangle((int)coordinates[0].X, (int)coordinates[0].Y, (int)coordinates[0].Width / 2, (int)coordinates[0].Height), 
            //            //                          new Rectangle((int)(coordinates[0].X * 2), (int)coordinates[0].Y, (int)coordinates[0].Width / 2, (int)coordinates[0].Height), GraphicsUnit.Pixel);

            //            //Rectangle destRect = new Rectangle((int)412.799, (int)320.29, (int)coordinates[0].Width, (int)coordinates[0].Height);
            //            //Rectangle sourceRect = new Rectangle((int)412.799, (int)320.29, (int)coordinates[0].Width, (int)coordinates[0].Height);

            //            Rectangle rect = new Rectangle((int)97.4211, (int)305.86996, (int)coordinates[0].Width, (int)coordinates[0].Height);


            //            //Rectangle destRect = new Rectangle(0, 50, image.Width / 2, 500); // Adjust width as needed
            //            //Rectangle sourceRect = new Rectangle(100, 50, image.Width / 2, 500);

            //            graphics.DrawImage(image, rect, rect, GraphicsUnit.Point);



            //            //graphics.DrawImage(image, rect, rect, GraphicsUnit.Pixel);

            //            bitmap.Save(@"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\" + String.Format("CropedPNG-img-{0}.png", i), ImageFormat.Png);

            //        }

                    

            //    }

            //}


            //// Get response from azure AI
            //AzureVisionAI azureVisionAI = new AzureVisionAI();
            //await azureVisionAI.RunProcess();
            #endregion

            // Using GemBox
            //ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            // Read content
            //using (var document = PdfDocument.Load(getCoordinatesFromDocumentPath))
            //{
            
            #region read text
            //var page = document.Pages[0];
            //var text = page.Content.GetText(new PdfTextOptions
            //{
            //    Bounds = new PdfQuad(140, 640, 250, 660),
            //    Order = PdfTextOrder.Reading
            //}).ToString();

            //Console.WriteLine($"Result: {text}");
            #endregion

            //foreach (var page in document.Pages)
            //{
            //    var contentEnumerator = page.Content.Elements.All(page.Transform).GetEnumerator();
            //    while (contentEnumerator.MoveNext())
            //    {
            //        if (contentEnumerator.Current.ElementType == PdfContentElementType.Text)
            //        {
            //            var textElement = (PdfTextContent)contentEnumerator.Current;

            //            var text = textElement.ToString();
            //            var font = textElement.Format.Text.Font;
            //            var color = textElement.Format.Fill.Color;
            //            var bounds = textElement.Bounds;

            //            contentEnumerator.Transform.Transform(ref bounds);

            //            // Read the text content element's additional information.
            //            Console.WriteLine($"Unicode text: {text}");
            //            Console.WriteLine($"Font name: {font.Face.Family.Name}");
            //            Console.WriteLine($"Font size: {font.Size}");
            //            Console.WriteLine($"Font style: {font.Face.Style}");
            //            Console.WriteLine($"Font weight: {font.Face.Weight}");

            //            if (color.TryGetRgb(out double red, out double green, out double blue))
            //                Console.WriteLine($"Color: Red={red}, Green={green}, Blue={blue}");

            //            Console.WriteLine($"Bounds: Left={bounds.Left:0.00}, Bottom={bounds.Bottom:0.00}, Right={bounds.Right:0.00}, Top={bounds.Top:0.00}");
            //            Console.WriteLine();
            //        }

            //        if (contentEnumerator.Current.ElementType == PdfContentElementType.Form)
            //        {
            //            var textElement = (PdfFormContent)contentEnumerator.Current;

            //            var text = textElement.ToString();
            //            var bounds = textElement.Bounds;

            //            contentEnumerator.Transform.Transform(ref bounds);

            //            // Read the text content element's additional information.
            //            Console.WriteLine($"Unicode text: {text}");

            //            Console.WriteLine($"Bounds: Left={bounds.Left:0.00}, Bottom={bounds.Bottom:0.00}, Right={bounds.Right:0.00}, Top={bounds.Top:0.00}");
            //            Console.WriteLine();
            //        } 

            //        if (contentEnumerator.Current.ElementType == PdfContentElementType.Path)
            //        {
            //            var textElement = (PdfPathContent)contentEnumerator.Current;

            //            var text = textElement.ToString();
            //            var bounds = textElement.Bounds;

            //            contentEnumerator.Transform.Transform(ref bounds);

            //            // Read the text content element's additional information.
            //            Console.WriteLine($"Unicode text: {text}");

            //            Console.WriteLine($"Bounds: Left={bounds.Left:0.00}, Bottom={bounds.Bottom:0.00}, Right={bounds.Right:0.00}, Top={bounds.Top:0.00}");
            //            Console.WriteLine();
            //        }
            //    }
            //}


            //}


        }


    }
}