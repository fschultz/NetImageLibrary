using System;
using System.Collections.Generic;

using System.Text;
using Kaliko.ImageLibrary;
using System.Drawing;

namespace TestApp {
    class Program {
        static void Main(string[] args) {
            KalikoImage image = new KalikoImage("testimage.jpg");
            image.BackgroundColor = Color.Aquamarine;

            image.GetThumbnailImage(100, 100, ThumbnailMethod.Fit).SaveJPG("thumbnail-fit.jpg", 90);

            image.GetThumbnailImage(100, 100, ThumbnailMethod.Crop).SaveJPG("thumbnail-crop.jpg", 90);

            image.GetThumbnailImage(100, 100, ThumbnailMethod.Pad).SaveJPG("thumbnail-pad.jpg", 90);
        }
    }
}
