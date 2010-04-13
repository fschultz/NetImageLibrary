using System;
using System.Drawing;
using Kaliko.ImageLibrary;
using Kaliko.ImageLibrary.Filters;

namespace TestApp {
    class Program {
        static void Main() {
            KalikoImage image = new KalikoImage("testimage.jpg");
            image.BackgroundColor = Color.Aquamarine;

            image.GetThumbnailImage(100, 140, ThumbnailMethod.Fit).SaveJpg("thumbnail-fit.jpg", 90);

            image.GetThumbnailImage(100, 140, ThumbnailMethod.Crop).SaveJpg("thumbnail-crop.jpg", 90);

            image.GetThumbnailImage(100, 140, ThumbnailMethod.Pad).SaveJpg("thumbnail-pad.jpg", 90);

            KalikoImage sharpimg = image.GetThumbnailImage(100, 100, ThumbnailMethod.Crop);
            sharpimg.ApplyFilter(new UnsharpMaskFilter(1.2, 0.3));
            sharpimg.SaveJpg("thumbnail-unsharpened.jpg", 90);

            sharpimg.ApplyFilter(new DesaturationFilter());
            sharpimg.SaveJpg("thumbnail-gray.jpg", 90);

            sharpimg.ApplyFilter(new ContrastFilter(30));
            sharpimg.SaveJpg("thumbnail-contrast.jpg", 90);

        }
    }
}
