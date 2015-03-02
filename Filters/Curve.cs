#region License and copyright notice
/*
 * Ported to .NET for use in Kaliko.ImageLibrary by Fredrik Schultz 2015
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

namespace Kaliko.ImageLibrary.Filters {
    using System;

    public class Curve {
        public float[] X;
        public float[] Y;

        public Curve() {
            X = new float[] { 0, 1 };
            Y = new float[] { 0, 1 };
        }

        public Curve(Curve curve) {
            X = (float[])curve.X.Clone();
            Y = (float[])curve.Y.Clone();
        }

        public int AddKnot(float kx, float ky) {
            var pos = -1;
            var numKnots = X.Length;
            var nx = new float[numKnots + 1];
            var ny = new float[numKnots + 1];
            var j = 0;
            for (var i = 0; i < numKnots; i++) {
                if (pos == -1 && X[i] > kx) {
                    pos = j;
                    nx[j] = kx;
                    ny[j] = ky;
                    j++;
                }
                nx[j] = X[i];
                ny[j] = Y[i];
                j++;
            }
            if (pos == -1) {
                pos = j;
                nx[j] = kx;
                ny[j] = ky;
            }
            X = nx;
            Y = ny;
            return pos;
        }

        public void RemoveKnot(int n) {
            var numKnots = X.Length;
            if (numKnots <= 2) {
                return;
            }
            var nx = new float[numKnots - 1];
            var ny = new float[numKnots - 1];
            var j = 0;
            for (var i = 0; i < numKnots - 1; i++) {
                if (i == n) {
                    j++;
                }
                nx[i] = X[j];
                ny[i] = Y[j];
                j++;
            }
            X = nx;
            Y = ny;
        }

        private void SortKnots() {
            var numKnots = X.Length;
            for (var i = 1; i < numKnots - 1; i++) {
                for (var j = 1; j < i; j++) {
                    if (!(X[i] < X[j])) {
                        continue;
                    }

                    var t = X[i];
                    X[i] = X[j];
                    X[j] = t;
                    t = Y[i];
                    Y[i] = Y[j];
                    Y[j] = t;
                }
            }
        }

        public int[] MakeTable() {
            var numKnots = X.Length;
            var nx = new float[numKnots + 2];
            var ny = new float[numKnots + 2];
            Array.Copy(X, 0, nx, 1, numKnots);
            Array.Copy(Y, 0, ny, 1, numKnots);
            nx[0] = nx[1];
            ny[0] = ny[1];
            nx[numKnots + 1] = nx[numKnots];
            ny[numKnots + 1] = ny[numKnots];

            var table = new int[256];
            for (var i = 0; i < 1024; i++) {
                var f = i / 1024.0f;
                var x = (int)(255 * ImageMath.spline(f, nx.Length, nx) + 0.5f);
                var y = (int)(255 * ImageMath.spline(f, nx.Length, ny) + 0.5f);
                x = ImageMath.Clamp(x, 0, 255);
                y = ImageMath.Clamp(y, 0, 255);
                table[x] = y;
            }
            return table;
        } 
    }
}