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
    using System.Threading.Tasks;
    using Filters;

    public class FastContrastFilter : ContrastFilter {

        public FastContrastFilter(int changeInContrast) : base(changeInContrast) {
        }

        public override void Run(KalikoImage image) {
            ChangeContrast(image);
        }

        private void ChangeContrast(KalikoImage image) {
            var lookupTable = BuildLookupTable();

            unsafe {
                var bitmapData = image.LockBits();

                var bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat)/8;
                var height = bitmapData.Height;
                var widthInBytes = bitmapData.Width*bytesPerPixel;
                var startOffset = (byte*)bitmapData.Scan0;

                Parallel.For(0, height, y => {
                    var currentLine = startOffset + (y*bitmapData.Stride);
                    for (var x = 0; x < widthInBytes; x = x + bytesPerPixel) {
                        var red = currentLine[x];
                        var green = currentLine[x + 1];
                        var blue = currentLine[x + 2];

                        currentLine[x] = lookupTable[red];
                        currentLine[x + 1] = lookupTable[green];
                        currentLine[x + 2] = lookupTable[blue];
                    }
                });

                image.UnlockBits(bitmapData);
            }
        }
    }
}