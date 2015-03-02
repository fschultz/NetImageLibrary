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

    public class CurvesFilter : TransferFilter, IFilter {

        private Curve[] _curves;

        public CurvesFilter() {
            _curves = new Curve[3];
            _curves[0] = new Curve();
            _curves[1] = new Curve();
            _curves[2] = new Curve();
        }

        protected override void Initialize() {
            Initialized = true;
            if (_curves.Length == 1) {
                rTable = gTable = bTable = _curves[0].MakeTable();
            }
            else {
                rTable = _curves[0].MakeTable();
                gTable = _curves[1].MakeTable();
                bTable = _curves[2].MakeTable();
            }
        }

        public void SetCurve(Curve curve) {
            _curves = new[] { curve };
            Initialized = false;
        }

        public void SetCurves(Curve[] curves) {
            if (curves == null || (curves.Length != 1 && curves.Length != 3)) {
                throw new ArgumentException("Curves must be length 1 or 3");
            }
            _curves = curves;
            Initialized = false;
        }

        public Curve[] GetCurves() {
            return _curves;
        }

        public void Run(KalikoImage image) {
            
        }
    }
}
