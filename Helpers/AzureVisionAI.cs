using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Azure;
using Azure.AI.Vision.ImageAnalysis;

namespace SpireSearchPDFCoordinates.Helpers
{
    public class AzureVisionAI
    {
        private string _subscriptionKey = "0952e6d6586c4ab39be179020d11564e";
        private string _endpoint = "https://voce-data-computer-vision.cognitiveservices.azure.com/";
        private string uriBase = string.Empty;
        private static ComputerVisionClient _client;
        private static ImageAnalysisClient _analysysClient;

        public AzureVisionAI()
        {
            uriBase = _endpoint + "vision/v3.1/ocr";
            _client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_subscriptionKey))
            {
                Endpoint = _endpoint
            };

            _analysysClient = new ImageAnalysisClient(
                new Uri(_endpoint),
                new AzureKeyCredential(_subscriptionKey));
        }
        public async Task<string> RunProcess(Bitmap bitmap)
        {
            try
            {
                try
                {
                    var responseString = await ExtractTextFromImageStream(bitmap);
                    return responseString;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
               
                //Console.Write(responseString);

                #region using httpclient
                //using (HttpClient client = new HttpClient())
                //{
                //    // Request headers
                //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                //    client.DefaultRequestHeaders.Add("Content-Type", "application/octet-stream");

                //    // Make the request
                //    HttpResponseMessage response;

                //    using (ByteArrayContent content = new ByteArrayContent(imageByteArray))
                //    {
                //        response = await client.PostAsync(uriBase, content);
                //    }

                //    // Get the JSON response
                //    string jsonResponse = await response.Content.ReadAsStringAsync();

                //    // Display the JSON response
                //    Console.WriteLine(jsonResponse);
                //}
                #endregion

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            return string.Empty;
        }

        public async Task<string> RunProcessUsingFilePath(string imageFilePath)
        {
            try
            {
                await ExtractTextFromImage(imageFilePath);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            return string.Empty;
        }

        private static async Task AnalyzeImage(string imageUrl)
        {
            IList<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags
            };

            try
            {
                // Analyze the image
                ImageAnalysis results = await _client.AnalyzeImageAsync(imageUrl, features);

                // Display results
                Console.WriteLine("Categories:");
                foreach (var category in results.Categories)
                {
                    Console.WriteLine($"  {category.Name} (Score: {category.Score})");
                }

                Console.WriteLine("\nDescription:");
                Console.WriteLine($"  {results.Description.Captions[0].Text} (Confidence: {results.Description.Captions[0].Confidence})");

                Console.WriteLine("\nTags:");
                foreach (var tag in results.Tags)
                {
                    Console.WriteLine($"  {tag.Name} (Confidence: {tag.Confidence})");
                }

                Console.WriteLine("\nFaces:");
                foreach (var face in results.Faces)
                {
                    Console.WriteLine($"  {face.Gender} ({face.Age} years old)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing image: {ex.Message}");
            }
        }

        private static async Task ExtractTextFromImage(string imageUrl)
        {
            try
            {
                // Read the image data from the URL
                Stream imageStream = new MemoryStream(new System.Net.WebClient().DownloadData(imageUrl));

                // Recognize text from the image
                OcrResult ocrResult = await _client.RecognizePrintedTextInStreamAsync(true, imageStream);

                // Process OCR result
                if (ocrResult != null)
                {

                    //JSON
                    string jsonResult = JsonConvert.SerializeObject(ocrResult, Formatting.Indented);

                    Console.WriteLine("Extracted Text:");
                    foreach (OcrRegion region in ocrResult.Regions)
                    {
                        foreach (OcrLine line in region.Lines)
                        {
                            foreach (OcrWord word in line.Words)
                            {
                                Console.Write(word.Text + " ");
                            }
                            Console.WriteLine();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No text recognized from the image.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text: {ex.Message}");
            }
        }

        private async Task<string> ExtractTextFromImageStream(Bitmap bitmap)
        {
            string completeString = string.Empty;

            try
            {

                MemoryStream memStream = new MemoryStream();
                bitmap.Save(memStream, ImageFormat.Png);
                memStream.Position = 0;
                byte[] imageData = memStream.ToArray();
                BinaryData binaryData = BinaryData.FromBytes(imageData);


                //var finalImagePath =
                //    @"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\" +
                //    String.Format("{0}.png", Guid.NewGuid());
                //MemoryStream memone = new MemoryStream();
                //bitmap.Save(finalImagePath, ImageFormat.Png);
                //bitmap.Save(memone, ImageFormat.Png);

                //using (MemoryStream memStream = new MemoryStream())
                //{;

                //MemoryStream memStream = new MemoryStream(new System.Net.WebClient().DownloadData(finalImagePath));

                ////Read the image from a path
                //var imageBytes = File.ReadAllBytes(finalImagePath);
                //MemoryStream imageStream = new MemoryStream(imageBytes);


                #region Using Analyze Image

                //using (FileStream fs = File.OpenRead(imageData))
                //{
                    //byte[] buffer = new byte[fs.Length];
                    //int bytesRead = fs.Read(buffer, 0, buffer.Length);
                    //BinaryData binaryData = BinaryData.FromBytes(buffer);

                ImageAnalysisResult result = _analysysClient.Analyze(
                    binaryData, VisualFeatures.Read);

                foreach (var detectedTextBlock in result.Read.Blocks)
                {
                    foreach (var lines in detectedTextBlock.Lines)
                    {
                        completeString += lines.Text + " ";
                    }
                }

                //}

                memStream.Dispose();

                return completeString;
                #endregion

                #region old OCR
                //OcrResult ocrResult = await _client.RecognizePrintedTextInStreamAsync(false, imageStream);

                //    //MemoryStream memStream = new MemoryStream();
                //    //bitmap.Save(memStream, ImageFormat.Png);
                //    //memStream.Position = 0;



                //    // Recognize text from the image
                //    //OcrResult ocrResult = await _client.RecognizePrintedTextInStreamAsync(false, memStream);


                //    //// Write the file
                //    //var filePath = @"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\finalImage101.png";
                //    //using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                //    //{
                //    //    memStream.CopyTo(fileStream);
                //    //}

                //    // Process OCR result
                //    if (ocrResult != null)
                //    {
                //        string jsonResult = JsonConvert.SerializeObject(ocrResult, Formatting.Indented);


                //        Console.WriteLine("Extracted Text:");
                //        foreach (OcrRegion region in ocrResult.Regions)
                //        {
                //            foreach (OcrLine line in region.Lines)
                //            {
                //                foreach (OcrWord word in line.Words)
                //                {
                //                    completeString += word.Text + " ";
                //                    Console.Write(word.Text + " ");
                //                }
                //                //Console.WriteLine();
                //            }
                //        }
                //    }
                //    else
                //    {
                //        Console.WriteLine("No text recognized from the image.");
                //    }
                ////}
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text: {ex.Message}");
            }

            return completeString;
        }


    }
}
