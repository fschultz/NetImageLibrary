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