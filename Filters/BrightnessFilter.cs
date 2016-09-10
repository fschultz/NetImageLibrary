#region License and copyright notice
/*
 * Kaliko Image Library
 * 
 * Copyright (c) 2014 Fredrik Schultz
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

namespace Kaliko.ImageLibrary.Filters {
    using System;

    /// <summary>Simple filter for adjusting brightness in images.</summary>
    public class BrightnessFilter : IFilter {
        private readonly double _brightness;

        /// <param name="changeInBrightness">The amount of change to be applied to the brightness. Entered as either a positive - to make it brighter - or negative - to make it darker - value (zero
        /// means no change).</param>
        public BrightnessFilter(int changeInBrightness) {
            _brightness = 1 + ((double)changeInBrightness / 100);
        }

        /// <summary>Execute the filter.</summary>
        public virtual void Run(KalikoImage image) {
            ChangeBrightness(image);
        }

        protected byte[] BuildLookupTable() {
            var lookupTable = new byte[256];

            for(var i = 0;i < 256;i++) {
                var val = (int)Math.Round(i * _brightness);
                if(val < 0) {
                    val = 0;
                }
                else if(val > 255) {
                    val = 255;
                }
                lookupTable[i] = (byte)val;
            }

            return lookupTable;
        }

        private void ChangeBrightness(KalikoImage image) {
            var lookupTable = BuildLookupTable();

            var byteArray = image.ByteArray;

            for(int i = 0, l = byteArray.Length;i < l;i += 4) {
                byteArray[i] = lookupTable[byteArray[i]];          // b
                byteArray[i + 1] = lookupTable[byteArray[i + 1]];  // g
                byteArray[i + 2] = lookupTable[byteArray[i + 2]];  // r
            }

            image.ByteArray = byteArray;
        }
    }
}
