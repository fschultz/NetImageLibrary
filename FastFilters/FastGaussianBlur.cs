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

    public class FastGaussianBlurFilter : GaussianBlurFilter {
        public FastGaussianBlurFilter() : this(2) {}

        public FastGaussianBlurFilter(float radius) {
            Radius = radius;
        }

        public override void Run(KalikoImage image) {
            if (Radius == 0) {
                return;
            }

            var bitmapData = image.LockBits();
            var tempBitmap = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.PixelFormat);

            var tempBitmapData = tempBitmap.LockBits(new Rectangle(0, 0, tempBitmap.Width, tempBitmap.Height), ImageLockMode.ReadWrite, tempBitmap.PixelFormat);

            ConvolveAndTranspose(Kernel, bitmapData, tempBitmapData, bitmapData.Width, bitmapData.Height, UseAlpha, UseAlpha && PremultiplyAlpha, false, EdgeMode.Clamp);
            ConvolveAndTranspose(Kernel, tempBitmapData, bitmapData, bitmapData.Height, bitmapData.Width, UseAlpha, false, UseAlpha && PremultiplyAlpha, EdgeMode.Clamp);

            tempBitmap.UnlockBits(tempBitmapData);
            image.UnlockBits(bitmapData);

            tempBitmap.Dispose();
        }

        public static void ConvolveAndTranspose(Kernel kernel, BitmapData inPixels, BitmapData outPixels, int width, int height, bool alpha, bool premultiply, bool unpremultiply, EdgeMode edgeAction) {
            unsafe {
                var matrix = kernel.GetKernel();
                var halfKernalWidth = kernel.Width/2;

                var bytesPerPixel = Image.GetPixelFormatSize(inPixels.PixelFormat)/8;

                var hasAlpha = PixelFormat.Alpha.HasFlag(inPixels.PixelFormat);

                var sourceStart = (byte*)inPixels.Scan0;
                var destinationStart = (byte*)outPixels.Scan0;

                Parallel.For(0, height, y => {
                    var index = y;
                    var baseOffset = y*width;

                    for (var x = 0; x < width; x++) {
                        var r = 0f;
                        var g = 0f;
                        var b = 0f;
                        var a = 0f;

                        for (var col = -halfKernalWidth; col <= halfKernalWidth; col++) {
                            var f = matrix[halfKernalWidth + col];
                            if (f == 0) {
                                continue;
                            }

                            var offsetX = x + col;
                            if (offsetX < 0) {
                                switch (edgeAction) {
                                    case EdgeMode.Clamp:
                                        offsetX = 0;
                                        break;
                                    case EdgeMode.Wrap:
                                        offsetX = (x + width)%width;
                                        break;
                                }
                            }
                            else if (offsetX >= width) {
                                switch (edgeAction) {
                                    case EdgeMode.Clamp:
                                        offsetX = width - 1;
                                        break;
                                    case EdgeMode.Wrap:
                                        offsetX = (x + width)%width;
                                        break;
                                }
                            }
                            var sourceOffset = sourceStart + ((baseOffset + offsetX)*bytesPerPixel);
                            var sourceR = sourceOffset[0];
                            var sourceG = sourceOffset[1];
                            var sourceB = sourceOffset[2];
                            byte sourceA = 0;
                            if (hasAlpha) {
                                sourceA = sourceOffset[3];
                            }

                            if (premultiply && hasAlpha) {
                                var alphaMultiply = sourceA*(1.0f/255.0f);
                                sourceR = (byte)(sourceR*alphaMultiply);
                                sourceG = (byte)(sourceG*alphaMultiply);
                                sourceB = (byte)(sourceB*alphaMultiply);
                            }

                            if (hasAlpha) {
                                a += f*sourceA;
                            }
                            r += f*sourceR;
                            g += f*sourceG;
                            b += f*sourceB;
                        }

                        if (unpremultiply && hasAlpha && a != 0 && a != 255) {
                            var f = 255.0f / a;
                            r *= f;
                            g *= f;
                            b *= f;
                        }

                        var destinationA = alpha ? ClampByte((int)(a + 0.5)) : (byte)0xff;
                        var destinationR = ClampByte(r + 0.5);
                        var destinationG = ClampByte(g + 0.5);
                        var destinationB = ClampByte(b + 0.5);

                        var destOffset = destinationStart + (index*bytesPerPixel);

                        destOffset[0] = destinationR;
                        destOffset[1] = destinationG;
                        destOffset[2] = destinationB;
                        if (hasAlpha) {
                            destOffset[3] = destinationA;
                        }
                        index += height;
                    }
                });
            }
        }



        /// <summary>
        /// Clamp a value to the range 0..255
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte ClampByte(double value) {
            if (value < 0) {
                return 0;
            }
            if (value > 255) {
                return 255;
            }
            return (byte)value;
        }
    }
}