namespace SpireSearchPDFCoordinates
{
    using Newtonsoft.Json;
    using SearchPDFCoordinates;
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
        static string getTextFromDocumentPath = Path.Combine(rootDirectory, "Documents", "Aransas.pdf");

        //Working Documents - These documents are working properly
        //static string getCoordinatesFromDocumentPath = Path.Combine(rootDirectory, "Documents/Working", "R163775_WD_Template.pdf");
        //static string getTextFromDocumentPath = Path.Combine(rootDirectory, "Documents/Working", "R163775 WD.pdf");


        static void Main(string[] args)
        {
            /*
             * Get the coordinates from the file and write them to the coordinates json file.
             * This is later used to fetch the content from the file
             */
            //GetCoordinatesFromDocument.UsingSpireExtractAnnotationsCoordinates(getCoordinatesFromDocumentPath);


            /*
             * Fetch the text using the coordinates.json file
             */
            TextExtractor.ReadDocValuesUsingSpire(getTextFromDocumentPath);
        }

        
    }
}