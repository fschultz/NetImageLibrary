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

    /// <summary>
    /// 
    /// </summary>
    public class ContrastFilter : IFilter {
        private readonly double _contrast;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeInContrast"></param>
        public ContrastFilter(int changeInContrast) {
            _contrast = 1 + ((double)changeInContrast / 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public virtual void Run(KalikoImage image) {
            ChangeContrast(image);
        }

        private void ChangeContrast(KalikoImage image) {
            var lookupTable = BuildLookupTable();

            var byteArray = image.ByteArray;

            for(int i = 0, l = byteArray.Length;i < l;i += 4) {
                byteArray[i] = lookupTable[byteArray[i]];          // b
                byteArray[i + 1] = lookupTable[byteArray[i + 1]];  // g
                byteArray[i + 2] = lookupTable[byteArray[i + 2]];  // r
            }

            image.ByteArray = byteArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected byte[] BuildLookupTable() {
            var lookupTable = new byte[256];

            // Precalculate all changes
            for (var i = 0; i < 256; i++) {
                var value = i/255.0;
                value -= 0.5;
                value *= _contrast;
                value += 0.5;
                value = (int)Math.Round(value*255);
                if (value < 0) {
                    value = 0;
                }
                else if (value > 255) {
                    value = 255;
                }
                lookupTable[i] = (byte)value;
            }
            return lookupTable;
        }
    }
}
