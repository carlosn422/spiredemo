using SearchPDFCoordinates;

namespace SpireSearchPDFCoordinates
{
    using System;
    using System.IO;

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
        static string getTextFromDocumentPath = Path.Combine(rootDirectory, "Documents/Craigs_Initial_Sample", "Aransas.pdf");

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

            //Applying the license causes issues reading the files.
            //Spire.Pdf.License.LicenseProvider.SetLicenseKey("License/license.elic.xml");


            /*
             * Get the coordinates from the file and write them to the coordinates json file.
             * This is later used to fetch the content from the file
             */

            GetCoordinatesFromDocument.UsingSpireExtractAnnotationsCoordinates(getCoordinatesFromDocumentPath);

            /*
             * Fetch the text using the coordinates.json file
             */

            TextExtractor extractor = new TextExtractor();
            await extractor.ReadDocValuesUsingSpire(getTextFromDocumentPath);



            #region test code

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
            //            Rectangle rect = new Rectangle((int)97.4211, (int)305.86996, (int)coordinates[0].Width, (int)coordinates[0].Height);
            //            graphics.DrawImage(image, rect, rect, GraphicsUnit.Point);

            //            bitmap.Save(@"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\" + String.Format("CropedPNG-img-{0}.png", i), ImageFormat.Png);

            //            // Get response from azure AI
            //            AzureVisionAI azureVisionAI = new AzureVisionAI();
            //            await azureVisionAI.RunProcessUsingFilePath(@"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\CropedPNG-img-" + i + ".png");

            //        }



            //    }

                #endregion

            }

        }
}