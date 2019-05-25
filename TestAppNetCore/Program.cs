namespace TestAppNetCore {
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using Kaliko.ImageLibrary;
    using Kaliko.ImageLibrary.FastFilters;
    using Kaliko.ImageLibrary.Scaling;

    class Program {
        private static string AppPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string ImagePath = Path.GetFullPath(AppPath + "\\..\\..\\..\\vincent-guth-183404-unsplash.jpg"); // Sample photo by Vincent Guth @vingtcent
        private static string OutputPath = Path.GetFullPath(AppPath + "\\..\\..\\..\\output/");

        static void Main(string[] args) {
            Console.WriteLine("Demo application for .NET Image Library [.NET Core]");

            // Scaling
            CropScale();
            FitScale();
            FocalPointScale();
            PadScale();

            // Filters
            Brightness();
            ChromaKey();
            Contrast();
            Desaturate();
            GaussianBlur();
            Invert();
            UnsharpMask();

            // Functions
            DrawText();
            GradientFill();
            Watermark();
        }

        #region Scaling

        private static void CropScale() {
            using (var image = new KalikoImage(ImagePath)) {
                image
                    .Scale(new CropScaling(500, 500))
                    .SaveJpg($"{OutputPath}cropscale.jpg", 80);
            }
        }

        private static void FitScale() {
            using (var image = new KalikoImage(ImagePath)) {
                image
                    .Scale(new FitScaling(500, 500))
                    .SaveJpg($"{OutputPath}fitscale.jpg", 80);
            }
        }

        private static void PadScale() {
            using (var image = new KalikoImage(ImagePath)) {
                image
                    .Scale(new PadScaling(500, 500, Color.Crimson))
                    .SaveJpg($"{OutputPath}padscale.jpg", 80);
            }
        }

        private static void FocalPointScale() {
            using (var image = new KalikoImage(ImagePath)) {
                image
                    .Scale(new FocalPointScaling(500, 500, 1, 1))
                    .SaveJpg($"{OutputPath}focalpointscale.jpg", 80);
            }
        }

        #endregion Scaling

        #region Filters

        // Note: Unless your application is running in medium trust environment, always use the FastFilters-version of the filter

        private static void Brightness() {
            using (var image = new KalikoImage(ImagePath)) {
                image.ApplyFilter(new FastBrightnessFilter(50));
                image.SaveJpg($"{OutputPath}brightness.jpg", 80);
            }
        }

        private static void Contrast() {
            using (var image = new KalikoImage(ImagePath)) {
                image.ApplyFilter(new FastContrastFilter(50));
                image.SaveJpg($"{OutputPath}contrast.jpg", 80);
            }
        }

        private static void ChromaKey() {
            using (var image = new KalikoImage(Path.GetFullPath(AppPath + "\\..\\..\\..\\jcvd-green-screen.jpg"))) {
                image.ApplyFilter(new FastChromaKeyFilter(Color.FromArgb(13, 161, 37), 40, 0.5f, 0.75f));
                image.SavePng($"{OutputPath}chromakey.png");

                using (var background = new KalikoImage(ImagePath)) {
                    var scaledBackground = background.Scale(new FocalPointScaling(640, 320, 0, 1));
                    scaledBackground.BlitImage(image);
                    scaledBackground.SaveJpg($"{OutputPath}chromakey.jpg", 80);
                }
            }
        }

        private static void Desaturate() {
            using (var image = new KalikoImage(ImagePath)) {
                image.ApplyFilter(new FastDesaturationFilter());
                image.SaveJpg($"{OutputPath}desaturate.jpg", 80);
            }
        }

        private static void GaussianBlur() {
            using (var image = new KalikoImage(ImagePath)) {
                image.ApplyFilter(new FastGaussianBlurFilter(1.1f));
                image.SaveJpg($"{OutputPath}gaussianblur.jpg", 80);
            }
        }

        private static void Invert() {
            using (var image = new KalikoImage(ImagePath)) {
                image.ApplyFilter(new FastInvertFilter());
                image.SaveJpg($"{OutputPath}invert.jpg", 80);
            }
        }

        private static void UnsharpMask() {
            using (var image = new KalikoImage(ImagePath)) {
                image.ApplyFilter(new FastUnsharpMaskFilter(1.1f, 0.44f, 0));
                image.SaveJpg($"{OutputPath}unsharpmask.jpg", 80);
            }
        }

        #endregion Filters

        #region Functions

        private static void DrawText() {
            using (var image = new KalikoImage(ImagePath)) {
                var text = new TextField("Lorem ipsum") {
                    Alignment = StringAlignment.Center,
                    VerticalAlignment = StringAlignment.Center,
                    Outline = 5,
                    OutlineColor = Color.Red,
                    Font = new Font("Arial", 60),
                    Rotation = 30f,
                    TextColor = Color.DarkOrange,
                    TextShadow = new TextShadow(Color.FromArgb(128, 0, 0, 0), 4, 4)
                };
                image.DrawText(text);
                image.SaveJpg($"{OutputPath}text.jpg", 80);
            }
        }

        private static void GradientFill()
        {
            using (var image = new KalikoImage(ImagePath))
            {
                image.GradientFill(Color.FromArgb(128, 255, 200, 90), Color.FromArgb(128, 255, 64, 0));
                image.SaveJpg($"{OutputPath}gradientfill.jpg", 80);
            }
        }
        
        private static void Watermark()
        {
            using (var image = new KalikoImage(ImagePath))
            {
                image.BlitFill(Path.GetFullPath(AppPath + "\\..\\..\\..\\watermark.png"));
                image.SaveJpg($"{OutputPath}watermark.jpg", 80);
            }
        }

        #endregion Functions
    }
}

