using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace SpireSearchPDFCoordinates.Helpers
{
    public class AzureVisionAI
    {
        private string _subscriptionKey = "0952e6d6586c4ab39be179020d11564e";
        private string _endpoint = "https://voce-data-computer-vision.cognitiveservices.azure.com/";
        private string uriBase = string.Empty;
        private static ComputerVisionClient _client;

        public AzureVisionAI()
        {
            uriBase = _endpoint + "vision/v3.1/ocr";
            _client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_subscriptionKey))
            {
                Endpoint = _endpoint
            };
        }
        public async Task<string> RunProcess(MemoryStream imageStream)
        {
            try
            {
                // Read the image file into a byte array
                //byte[] imageByteArray = File.ReadAllBytes(imageFilePath);

                //await AnalyzeImage(client, imageFilePath);

                string imageFilePath =
                    @"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\CropedPNG-img-0.png";
                await ExtractTextFromImage(imageFilePath);

                try
                {
                    var responseString = await ExtractTextFromImageStream(imageStream);
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

        private async Task<string> ExtractTextFromImageStream(MemoryStream imageStream)
        {
            string completeString = string.Empty;

            try
            {
                var canReadImage = imageStream.CanRead;
               
                // Recognize text from the image
                OcrResult ocrResult = await _client.RecognizePrintedTextInStreamAsync(true, imageStream);

                // Process OCR result
                if (ocrResult != null)
                {
                    string jsonResult = JsonConvert.SerializeObject(ocrResult, Formatting.Indented);

                   
                    Console.WriteLine("Extracted Text:");
                    foreach (OcrRegion region in ocrResult.Regions)
                    {
                        foreach (OcrLine line in region.Lines)
                        {
                            foreach (OcrWord word in line.Words)
                            {
                                completeString += word.Text + " ";
                                //Console.Write(word.Text + " ");
                            }
                            //Console.WriteLine();
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

            return completeString;
        }
    }
}
