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
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using ColorSpace;
    using Filters;

    public class FastChromaKeyFilter : IFilter {
        public float ToleranceHue { get; set; }

        public float ToleranceSaturnation { get; set; }

        public float ToleranceBrightness { get; set; }

        public Color KeyColor { get; set; }

        public FastChromaKeyFilter() {
            KeyColor = Color.FromArgb(0, 255, 0);
            ToleranceHue = 10;
            ToleranceSaturnation = 0.7f;
            ToleranceBrightness = 0.5f;
        }

        public FastChromaKeyFilter(Color keyColor) {
            KeyColor = keyColor;
            ToleranceHue = 10;
            ToleranceSaturnation = 0.7f;
            ToleranceBrightness = 0.5f;
        }

        public FastChromaKeyFilter(Color keyColor, float toleranceHue, float toleranceSaturnation, float toleranceBrightness) {
            KeyColor = keyColor;
            ToleranceHue = toleranceHue;
            ToleranceSaturnation = toleranceSaturnation;
            ToleranceBrightness = toleranceBrightness;
        }

        public void Run(KalikoImage image) {
            ValidateParameters();

            ApplyChromaKey(image);
        }

        public void ApplyChromaKey(KalikoImage image) {
            unsafe {
                var bitmapData = image.LockBits();

                var bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat)/8;
                var height = bitmapData.Height;
                var widthInBytes = bitmapData.Width*bytesPerPixel;
                var startOffset = (byte*)bitmapData.Scan0;

                var keyHsb = ColorSpaceHelper.RGBtoHSB(KeyColor);

                Parallel.For(0, height, y => {
                    var currentLine = startOffset + (y * bitmapData.Stride);
                    for (var x = 0; x < widthInBytes; x = x + bytesPerPixel) {
                        var red = currentLine[x];
                        var green = currentLine[x + 1];
                        var blue = currentLine[x + 2];
                        var hsb = ColorSpaceHelper.RGBtoHSB(red, green, blue);

                        if (Abs(hsb.Hue, keyHsb.Hue) < ToleranceHue && Abs(hsb.Saturation, keyHsb.Saturation) < ToleranceSaturnation && Abs(hsb.Brightness, keyHsb.Brightness) < ToleranceBrightness) {
                            // TODO: Add alpha check
                            currentLine[x + 3] = 0;
                        }
                    }
                });

                image.UnlockBits(bitmapData);
            }
        }

        private static double Abs(double b1, double b2) {
            return b1 > b2 ? b1 - b2 : b2 - b1;
        }

        private void ValidateParameters() {
            if (ToleranceHue < 0 || ToleranceHue > 360) {
                throw new ArgumentException("ToleranceHue out of range (0..360)");
            }
            if (ToleranceSaturnation < 0 || ToleranceSaturnation > 1) {
                throw new ArgumentException("ToleranceSaturnation out of range (0..1)");
            }
            if (ToleranceBrightness < 0 || ToleranceBrightness > 1) {
                throw new ArgumentException("ToleranceBrightness out of range (0..1)");
            }
        }
    }
}