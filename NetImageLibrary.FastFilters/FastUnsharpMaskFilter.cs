#region License and copyright notice
/*
 * Ported to .NET for use in Kaliko.ImageLibrary by Fredrik Schultz 2016
 *
 * Original License:
 * Copyright 2006 Jerry Huxtable
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
#endregion

namespace Kaliko.ImageLibrary.FastFilters {
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading.Tasks;
    using Kaliko.ImageLibrary;
    using Kaliko.ImageLibrary.Filters;

    public class FastUnsharpMaskFilter : IFilter {
        readonly float _radius;
        readonly float _amount;
        private readonly int _threshold;

        public FastUnsharpMaskFilter(float radius, float amount, int threshold) {
            _radius = radius*3.14f;
            _amount = amount;
            _threshold = threshold;
        }

        public bool UseAlpha { get; set; }

        public bool PremultiplyAlpha { get; set; }

        public void Run(KalikoImage image) {
            Sharpen(image, _amount, _radius, _threshold);
        }

        public static byte ClampByte(double value) {
            if (value < 0) {
                return 0;
            }
            if (value > 255) {
                return 255;
            }
            return (byte)value;
        }

        private void Sharpen(KalikoImage image, float amount, float radius, int threshold) {
            var bitmapData = image.LockBits();

            var tempBitmap = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.PixelFormat);
            var tempBitmapData = tempBitmap.LockBits(new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), ImageLockMode.ReadWrite, tempBitmap.PixelFormat);

            var outBitmap = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.PixelFormat);
            var outBitmapData = outBitmap.LockBits(new Rectangle(0, 0, outBitmap.Width, outBitmap.Height), ImageLockMode.ReadWrite, outBitmap.PixelFormat);

            ApplyGaussianBlur(radius, bitmapData, tempBitmapData, outBitmapData);

            ApplyMask(amount, threshold, bitmapData, outBitmapData);

            outBitmap.UnlockBits(outBitmapData);
            tempBitmap.UnlockBits(tempBitmapData);
            image.UnlockBits(bitmapData);

            outBitmap.Dispose();
            tempBitmap.Dispose();
        }

        private static unsafe void ApplyMask(float amount, int threshold, BitmapData bitmapData, BitmapData outBitmapData) {
            var bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat)/8;
            var height = bitmapData.Height;
            var widthInBytes = bitmapData.Width*bytesPerPixel;
            var byteStart = (byte*)bitmapData.Scan0;
            var outStart = (byte*)outBitmapData.Scan0;

            Parallel.For(0, height, y => {
                var currentOriginalLine = byteStart + (y*bitmapData.Stride);
                var currentBlurredLine = outStart + (y*bitmapData.Stride);

                for (var x = 0; x < widthInBytes; x = x + bytesPerPixel) {
                    var r1 = currentOriginalLine[x];
                    var g1 = currentOriginalLine[x + 1];
                    var b1 = currentOriginalLine[x + 2];

                    var r2 = currentBlurredLine[x];
                    var g2 = currentBlurredLine[x + 1];
                    var b2 = currentBlurredLine[x + 2];

                    if (Abs(r1, r2) >= threshold) {
                        r1 = ClampByte(((amount + 1)*(r1 - r2) + r2));
                    }
                    if (Abs(g1, g2) >= threshold) {
                        g1 = ClampByte(((amount + 1)*(g1 - g2) + g2));
                    }
                    if (Abs(b1, b2) >= threshold) {
                        b1 = ClampByte(((amount + 1)*(b1 - b2) + b2));
                    }
                    currentOriginalLine[x] = r1;
                    currentOriginalLine[x + 1] = g1;
                    currentOriginalLine[x + 2] = b1;
                }
            });
        }

        private void ApplyGaussianBlur(float radius, BitmapData bitmapData, BitmapData tempBitmapData, BitmapData outBitmapData) {
            if (!(radius > 0)) {
                return;
            }

            var kernel = GaussianBlurFilter.CreateKernel(radius);
            FastGaussianBlurFilter.ConvolveAndTranspose(kernel, bitmapData, tempBitmapData, bitmapData.Width, bitmapData.Height, UseAlpha, UseAlpha && PremultiplyAlpha, false, ConvolveFilter.EdgeMode.Clamp);
            FastGaussianBlurFilter.ConvolveAndTranspose(kernel, tempBitmapData, outBitmapData, bitmapData.Height, bitmapData.Width, UseAlpha, false, UseAlpha && PremultiplyAlpha, ConvolveFilter.EdgeMode.Clamp);
        }

        private static int Abs(byte b1, byte b2) {
            return b1 > b2 ? b1 - b2 : b2 - b1;
        }
    }
}