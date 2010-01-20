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
using System.Drawing;
using System.Text;

namespace Kaliko.ImageLibrary.Filters {
    public class GaussianBlurFilter : IFilter {
        double _radius;
        double _amount;

        
        public GaussianBlurFilter(double radius, double amount) {
            _radius = radius;
            _amount = amount;
        }


        public void run(KalikoImage image) {
            byte[] b = image.ByteArray;
            byte[] dest = new byte[b.Length];

            GaussianBlur(image.Width, image.Height, _radius, _amount, ref b, ref dest);

            image.ByteArray = dest;
        }
        

        internal static void GaussianBlur(int width, int height, double radius, double amount, ref byte[] src, ref byte[] dst) {

            int shift, dest, source;
            int blurDiam = (int)Math.Pow(radius, 2);
            int gaussWidth = (blurDiam * 2) + 1;

            double[] kernel = CreateKernel(gaussWidth, blurDiam);

            // Calculate the sum of the Gaussian kernel      
            double gaussSum = 0;
            for(int n = 0;n < gaussWidth;n++) {
                gaussSum += kernel[n];
            }

            // Scale the Gaussian kernel
            for(int n = 0;n < gaussWidth;n++) {
                kernel[n] = kernel[n] / gaussSum;
            }
            //premul = kernel[k] / gaussSum;


            // Create an X & Y pass buffer  
            byte[] gaussPassX = new byte[src.Length];

            // Do Horizontal Pass  
            for(int y = 0;y < height;y++) {
                for(int x = 0;x < width;x++) {
                    dest = y * width + x;

                    // Iterate through kernel  
                    for(int k = 0;k < gaussWidth;k++) {

                        // Get pixel-shift (pixel dist between dest and source)  
                        shift = k - blurDiam;

                        // Basic edge clamp  
                        source = dest + shift;
                        if(x + shift <= 0 || x + shift >= width) { source = dest; }

                        // Combine source and destination pixels with Gaussian Weight  
                        gaussPassX[(dest << 2) + 2] = (byte)(gaussPassX[(dest << 2) + 2] + (src[(source << 2) + 2]) * kernel[k]);
                        gaussPassX[(dest << 2) + 1] = (byte)(gaussPassX[(dest << 2) + 1] + (src[(source << 2) + 1]) * kernel[k]);
                        gaussPassX[(dest << 2)] = (byte)(gaussPassX[(dest << 2)] + (src[(source << 2)]) * kernel[k]);
                    }
                }
            }

            // Do Vertical Pass  
            for(int x = 0;x < width;x++) {
                for(int y = 0;y < height;y++) {
                    dest = y * width + x;

                    // Iterate through kernel  
                    for(int k = 0;k < gaussWidth;k++) {

                        // Get pixel-shift (pixel dist between dest and source)   
                        shift = k - blurDiam;

                        // Basic edge clamp  
                        source = dest + (shift * width);
                        if(y + shift <= 0 || y + shift >= height) { source = dest; }

                        // Combine source and destination pixels with Gaussian Weight  
                        dst[(dest << 2) + 2] = (byte)(dst[(dest << 2) + 2] + (gaussPassX[(source << 2) + 2]) * kernel[k]);
                        dst[(dest << 2) + 1] = (byte)(dst[(dest << 2) + 1] + (gaussPassX[(source << 2) + 1]) * kernel[k]);
                        dst[(dest << 2)] = (byte)(dst[(dest << 2)] + (gaussPassX[(source << 2)]) * kernel[k]);
                    }
                }
            }
        }



        private static double[] CreateKernel(int gaussianWidth, int blurDiam) {
            
            double[] kernel = new double[gaussianWidth];

            // Set the maximum value of the Gaussian curve  
            double sd = 255;

            // Set the width of the Gaussian curve  
            double range = gaussianWidth;

            // Set the average value of the Gaussian curve   
            double mean = (range / sd);

            // Set first half of Gaussian curve in kernel  
            for(int pos = 0, len = blurDiam + 1;pos < len;pos++) {
                // Distribute Gaussian curve across kernel[array]   
                kernel[gaussianWidth - 1 - pos] = kernel[pos] = Math.Sqrt(Math.Sin((((pos + 1) * (Math.PI / 2)) - mean) / range)) * sd;
            }

            return kernel;
        }




        private static double[] CreateKernel2(int gaussianWidth, int blurDiam) {
            double[] kernel = new double[gaussianWidth];
            int half = gaussianWidth >> 1;

            kernel[half] = 1;

            for(int weight = 1;weight < half + 1;++weight) {
                double x = 3 * (double)weight / (double)half;
                //   Corresponding symmetric weights
                kernel[half - weight] = kernel[half + weight] = Math.Exp(-x * x / 2);
            }

            return kernel;
        }

    }
}
