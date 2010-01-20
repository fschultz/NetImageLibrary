/*
 * Kaliko Image Library
 * 
 * Copyright (c) 2009 Fredrik Schultz
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

using System;
using System.Collections.Generic;
using System.Text;


namespace Kaliko.ImageLibrary.Filters {
    public class UnsharpMaskFilter : IFilter {
        double _radius;
        double _amount;

        public UnsharpMaskFilter(double radius, double amount) {
            _radius = radius;
            _amount = amount;
        }

        public void run(KalikoImage image) {
            Sharpen(image, _amount, _radius);
        }

        private void Sharpen(KalikoImage image, double amount, double radius) {
            byte[] src = image.ByteArray;
            byte[] dest = new byte[src.Length];

            GaussianBlurFilter.GaussianBlur(image.Width, image.Height, _radius, _amount, ref src, ref dest);

            int i = 0;
            int r, g, b;

            for(int x = 0, l = image.Width;x < l;x++) {
                for(int y = 0, k = image.Height;y < k;y++) {
                    // Apply difference of gaussian blur filter
                    b = src[i]     + (int)((src[i]     - dest[i])     * amount);
                    g = src[i + 1] + (int)((src[i + 1] - dest[i + 1]) * amount);
                    r = src[i + 2] + (int)((src[i + 2] - dest[i + 2]) * amount);

                    // Keep inside range 0 to 255
                    if(r < 0)
                        r = 0;
                    else if(r > 255)
                        r = 255;
                    if(g < 0)
                        g = 0;
                    else if(g > 255)
                        g = 255;
                    if(b < 0)
                        b = 0;
                    else if(b > 255)
                        b = 255;

                    // Write back final bytes
                    dest[i] = (byte)b;
                    dest[i + 1] = (byte)g;
                    dest[i + 2] = (byte)r;

                    i += 4;
                }
            }

            image.ByteArray = dest;
        }
    }
}
