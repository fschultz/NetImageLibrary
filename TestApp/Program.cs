using System;
using System.Drawing;
using Kaliko.ImageLibrary;
using Kaliko.ImageLibrary.Filters;

namespace TestApp {
    class Program {
        static void Main(string[] args) {
            KalikoImage image = new KalikoImage("testimage.jpg");
            image.BackgroundColor = Color.Aquamarine;

            image.GetThumbnailImage(100, 100, ThumbnailMethod.Fit).SaveJPG("thumbnail-fit.jpg", 90);

            image.GetThumbnailImage(100, 100, ThumbnailMethod.Crop).SaveJPG("thumbnail-crop.jpg", 90);

            image.GetThumbnailImage(100, 100, ThumbnailMethod.Pad).SaveJPG("thumbnail-pad.jpg", 90);

            KalikoImage sharpimg = image.GetThumbnailImage(100, 100, ThumbnailMethod.Crop);
            sharpimg.ApplyFilter(new UnsharpMaskFilter(1.2, 0.3));
            sharpimg.SaveJPG("thumbnail-unsharpened.jpg", 90);

            sharpimg.ApplyFilter(new DesaturationFilter());
            sharpimg.SaveJPG("thumbnail-gray.jpg", 90);

            sharpimg.ApplyFilter(new ContrastFilter(30));
            sharpimg.SaveJPG("thumbnail-contrast.jpg", 90);

        }
    }
}
