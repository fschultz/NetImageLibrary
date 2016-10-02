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