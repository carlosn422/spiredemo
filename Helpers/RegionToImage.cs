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
        public Bitmap FetchImageFromRectangle(PdfDocument doc , Annotation_Spire annot)
        {
                


                if (annot.Page == 1)
                {
                    var status = "";
                }

                using (Image image = doc.SaveAsImage(annot.Page, 300, 300))
                {
                    
                    String fileName = String.Format(@"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\Pages\{0}ToPNG-img-{0}.png", annot.Page);
                    image.Save(fileName, ImageFormat.Png);

                    Bitmap bitmap = new Bitmap(image.Width / 2, 1000);
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        Rectangle rect = new Rectangle(
                            (int)annot.X, 
                            (int)annot.Y, 
                            (int)annot.Width,
                            (int)annot.Height);

                        graphics.DrawImage(image, rect, rect, GraphicsUnit.Point);
                        //bitmap.Save(@"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\" + String.Format("101CropedPNG-img-{0}.png", 0), ImageFormat.Png);
                        //bitmap.Save(memStream, ImageFormat.Png);

                        //var filePath = @"C:\Users\carlo\source\repos\SpireSearchPDFCoordinates\Documents\DocumentImages\finalImage.png";
                        //using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        //{
                        //    memStream.CopyTo(fileStream);
                        //}

                        return bitmap;

                    }

                }
                
        }
    }
}
