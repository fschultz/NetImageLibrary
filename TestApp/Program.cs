using System;
using System.Drawing;
using System.Drawing.Imaging;
using Kaliko.ImageLibrary;
using Kaliko.ImageLibrary.Filters;
using System.IO;
//using Kaliko.ImageLibrary.Transformation;

namespace TestApp {
    using System.Threading;

    class Program {
        private static string _failed = string.Empty;

        static void Main() {
            var dateTime = DateTime.Now;

/*            var image = new KalikoImage(@"testimage.jpg");
            image.SetFont("84_rock.ttf", 120, FontStyle.Regular);
            image.Color = Color.FromArgb(64, Color.White);
            image.WriteText("Hello World!", 0, 0);
            image.SaveJpg("Output.jpg", 90);*/


            //var image = new KalikoImage(@"Premiere-Pro-chroma-key-original.jpg");
            //image.ApplyFilter(new ChromaKeyFilter(Color.FromArgb(120,209,129), 35f, 0.1f, 0.2f));
            //image.SavePng("Output-ChromaKeyed.png", 90);


            var image = new KalikoImage(@"Premiere-Pro-chroma-key-original.jpg");
            //image.ApplyFilter(new GaussianBlurFilter(10));
            //image.SavePng("Output-gaussian-10.png", 90);
            //image = new KalikoImage(@"Premiere-Pro-chroma-key-original.jpg");
            //image.ApplyFilter(new GaussianBlurFilter(20));
            //image.SavePng("Output-gaussian-20.png", 90);
            //image = new KalikoImage(@"Premiere-Pro-chroma-key-original.jpg");
            //image.ApplyFilter(new GaussianBlurFilter(30));
            //image.SavePng("Output-gaussian-30.png", 90);
            //image = new KalikoImage(@"Premiere-Pro-chroma-key-original.jpg");
            //image.ApplyFilter(new GaussianBlurFilter(40));
            //image.SavePng("Output-gaussian-40.png", 90);

            image = new KalikoImage(@"Premiere-Pro-chroma-key-original.jpg");
            image.ApplyFilter(new UnsharpMaskFilter(2.2f, 0.4f, 0));
            image.SavePng("Output-gaussian-unsharp.png");


            image = new KalikoImage(@"Premiere-Pro-chroma-key-original.jpg");
            image.ApplyFilter(new GaussianBlurFilter(2.2f));
            image.SavePng("Output-gaussian.png");

            //image.ApplyFilter(new BlueFilter());
            //image.ApplyFilter(new NewGaussian(30));
            //image.ApplyFilter(new GaussianBlurFilter(30));
            //image.SaveJpg("Output.jpg", 90);


            Console.WriteLine("Time taken " + (DateTime.Now-dateTime).TotalMilliseconds +" ms");
            Thread.Sleep(2000);
/*
    // Open image from file system
    var image = new KalikoImage(@"C:\MyImages\Image.png");
            
    // Open image from a stream
    MemoryStream memoryStream = ...
    var image = new KalikoImage(memoryStream);
            
    // Creating a new empty image
    var image = new KalikoImage(640, 480, Color.White);

    // Save image to file system in jpg format with quality setting 90
    image.SaveJpg(@"C:\MyImages\Output.jpg", 90);
            
    // Save image to stream in jpg format with quality setting 90
    MemoryStream memoryStream = new MemoryStream();
    image.SaveJpg(memoryStream, 90);

    // Send the image in jpg format as a HttpResponse
    image.StreamJpg(80, "MyImage.jpg");

    // Save image to file system in the selected format
    image.SaveImage(@"C:\MyImages\Output.tif", ImageFormat.Tiff);
*/
/*            image = new KalikoImage(512, 512);

    // Load the font and set both font size as well as type
    image.SetFont("84_rock.ttf", 80, FontStyle.Regular);
    
    // Set the color that will be used, a semi-transparent white in this case        
    image.Color = Color.FromArgb(60, Color.White);

    // Write the text
    image.WriteText("My string", 0, 0);
            

    // Place the source image on top, left of our image
    var sourceImage = new KalikoImage(@"C:\Img\Stamp.png");
    image.BlitImage(sourceImage, 0, 0);

    // Repeat the above, but in a single call
    image.BlitImage(@"C:\Img\Stamp.png", 0, 0);


    // Create a new image and fill the source image all over
    var image = new KalikoImage(640, 480);
    var patternImage = new KalikoImage(@"C:\Img\Checkered.png");
    image.BlitFill(patternImage);

    // Repeat the above, but in a just one additional call
    var image = new KalikoImage(640, 480);
    image.BlitFill(@"C:\Img\Checkered.png");
            
            image.SaveJpg("test.jpg", 99);


            KalikoImage sharpimg = image.GetThumbnailImage(100, 100, ThumbnailMethod.Crop);
            sharpimg.ApplyFilter(new UnsharpMaskFilter(1.2, 0.3));
            sharpimg.SaveJpg("thumbnail-unsharpened.jpg", 90);

            sharpimg.ApplyFilter(new DesaturationFilter());
            sharpimg.SaveJpg("thumbnail-gray.jpg", 90);

            sharpimg.ApplyFilter(new ContrastFilter(30));
            sharpimg.SaveJpg("thumbnail-contrast.jpg", 90);
            //PixelFormatTest();
            */
            /*

            var fileStream = new FileStream("testimage.jpg", FileMode.Open, FileAccess.Read);
            byte[] imgBytes = new byte[fileStream.Length];
            fileStream.Read(imgBytes, 0, (int)fileStream.Length);
            fileStream.Close();

            KalikoImage img = new KalikoImage(new MemoryStream(imgBytes));

            img.BlitFill(@"alpha.png");

            MemoryStream ms = new MemoryStream();
            img.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] array = ms.ToArray();

            var stream = new FileStream("testXX.jpg", FileMode.Create);
            stream.Write(array, 0, array.Length);
            stream.Close();

            */

            //KalikoImage dest = new KalikoImage(640, 480);
            /*
            Matrix matRot = new Matrix();
            Matrix matTrans1 = new Matrix();
            Matrix matTrans0 = new Matrix();
            Matrix matScale = new Matrix();
            matTrans0.Translation(-(float)(image.Width) / 2.0f, -(float)(image.Height) / 2.0f);
            matRot.Rotation(0f);
            matScale.Scaling(0.5f, 0.5f);
            matTrans1.Translation((float)(dest.Width) / 2.0f, (float)(dest.Height) / 2.0f);


            Transforms.ImageTransform(dest, image, matTrans1 * ( matScale * (matRot * matTrans0)), Transforms.NearestNeigbour);

            dest.SaveJpg("s:\\aaa.jpg", 99);
            */

/*            image.SetFont("84_rock.ttf", 40, FontStyle.Regular);
            image.Color = Color.White;
            image.WriteText("A TEST OF STATIC", 200, 100);
            image.WriteText("A TEST OF ROTATION", 200, 100, 45);
            image.Color = Color.OrangeRed;
            image.WriteText("A TEST OF ROTATION", 200, 100, 15);
            image.WriteText("A TEST OF ROTATION", 200, 100, 80);
            */
            //image.SaveJpg("test.jpg", 100);

            //image.GetThumbnailImage(430, 430, ThumbnailMethod.Fit).SaveJpg("thumbnail-fitx.jpg", 90);

            //image.GetThumbnailImage(100, 140, ThumbnailMethod.Crop).SaveJpg("thumbnail-crop.jpg", 90);

            //image.GetThumbnailImage(100, 140, ThumbnailMethod.Pad).SaveJpg("thumbnail-pad.jpg", 90);

            /*
            KalikoImage sharpimg = image.GetThumbnailImage(100, 100, ThumbnailMethod.Crop);
            sharpimg.ApplyFilter(new UnsharpMaskFilter(1.2, 0.3));
            sharpimg.SaveJpg("thumbnail-unsharpened.jpg", 90);

            sharpimg.ApplyFilter(new DesaturationFilter());
            sharpimg.SaveJpg("thumbnail-gray.jpg", 90);

            sharpimg.ApplyFilter(new ContrastFilter(30));
            sharpimg.SaveJpg("thumbnail-contrast.jpg", 90);
            */



        }

        private static void PixelFormatTest() {
            TestFormat(PixelFormat.Format16bppGrayScale);
            TestFormat(PixelFormat.Format16bppRgb555);
            TestFormat(PixelFormat.Format16bppArgb1555);
            TestFormat(PixelFormat.Format16bppRgb565);
            TestFormat(PixelFormat.Format1bppIndexed);
            TestFormat(PixelFormat.Format24bppRgb);
            TestFormat(PixelFormat.Format32bppArgb);
            TestFormat(PixelFormat.Format32bppPArgb);
            TestFormat(PixelFormat.Format32bppRgb);
            TestFormat(PixelFormat.Format48bppRgb);
            TestFormat(PixelFormat.Format4bppIndexed);
            TestFormat(PixelFormat.Format64bppArgb);
            TestFormat(PixelFormat.Format64bppPArgb);
            TestFormat(PixelFormat.Format8bppIndexed);

            string a = _failed;

            /*
             * OutOfMemoryException:Format16bppGrayScale
             * OutOfMemoryException:Format16bppArgb1555
             * Exception: Format1bppIndexed
             * Exception: Format4bppIndexed
             * Exception: Format8bppIndexed
             */
        }

        private static void TestFormat(PixelFormat pixelFormat) {
            Bitmap test = new Bitmap(200, 200, pixelFormat);
            try {
                Graphics gr = Graphics.FromImage(test);
            }
            catch (OutOfMemoryException e) {
                _failed += "OutOfMemoryException:" + pixelFormat + "\r\n";
            }
            catch (Exception e) {
                _failed += "Exception: " + pixelFormat + "\r\n";
            }

        }
    }
}
