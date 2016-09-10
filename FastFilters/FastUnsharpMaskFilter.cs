#region License and copyright notice
/*
 * Kaliko Image Library
 * 
 * Copyright (c) Fredrik Schultz and Contributors
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
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