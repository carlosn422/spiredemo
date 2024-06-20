using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Pdf;

namespace SpireSearchPDFCoordinates.Helpers
{
    public class RegionToImage
    {
        public MemoryStream FetchImageFromRectangle(PdfDocument doc , Annotation_Spire annot)
        {
                String fileName = String.Format("{0}ToPNG-img-{0}.png", annot.Page);

                var coordinates = new[]
                {
                    new { Page = annot.Page, X = annot.X, Y = annot.Y, Width = annot.Width, Height = annot.Height },
                };

                MemoryStream memStream = new MemoryStream();

                using (Image image = doc.SaveAsImage(annot.Page, 300, 300))
                {

                    image.Save(fileName, ImageFormat.Png);

                    Bitmap bitmap = new Bitmap(image.Width / 2, 1000);

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        Rectangle rect = new Rectangle((int)97.4211, (int)305.86996, (int)coordinates[0].Width,
                            (int)coordinates[0].Height);

                        graphics.DrawImage(image, rect, rect, GraphicsUnit.Point);
                        bitmap.Save(@"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\" + String.Format("CropedPNG-img-{0}.png", 0), ImageFormat.Png);
                        bitmap.Save(memStream, ImageFormat.Png);

                        memStream.Position = 0;

                        var filePath = @"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\finalImage.png";
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            memStream.CopyTo(fileStream);
                        }

                        return memStream;

                    }

                }
                
        }
    }
}
