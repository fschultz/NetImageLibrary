/*
 * Kaliko Image Library
 * 
 * Copyright (c) 2013 Fredrik Schultz and Contributors
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

namespace Kaliko.ImageLibrary {
    public enum Direction {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum ThumbnailMethod {
        Fit,
        Pad,
        Crop
    }

    public class KalikoImage : IDisposable {
        #region Private variables

        private Image _image;
        private Graphics _g;
        private Font _font;
        private Color _color;
        private Color _backgroundColor;
        private TextRenderingHint _textrenderinghint = TextRenderingHint.ClearTypeGridFit;

        #endregion


        #region Constructors and destructors

        /// <summary>
        /// Create a KalikoImage from a System.Drawing.Image.
        /// </summary>
        /// <param name="image"></param>
        public KalikoImage(Image image) {
            _image = image;
            _g = Graphics.FromImage(_image);
        }

        /// <summary>
        /// Create a KalikoImage with a defined width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public KalikoImage(int width, int height) {
            CreateImage(width, height);
        }

        /// <summary>
        /// Create a KalikoImage with a defined width, height and backgroundcolor.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bgcolor"></param>
        public KalikoImage(int width, int height, Color bgcolor) {
            _backgroundColor = bgcolor;
            CreateImage(width, height);
            Clear(bgcolor);
        }

        /// <summary>
        /// Create a KalikoImage by loading an image from either local disk or web.
        /// </summary>
        /// <example>
        /// Open a local image:
        ///     KalikoImage image = new KalikoImage("c:\\images\\test.jpg");
        /// 
        /// Load an image from the web:
        ///     KalikoImage image = new KalikoImage("http://yourdomain.com/test.jpg");
        /// </example>
        /// <param name="filepath">Local filepath or internet URL</param>
        public KalikoImage(string filepath) {
            string prefix = filepath.Length > 8 ? filepath.Substring(0, 8).ToLower() : "";

            if(prefix.StartsWith("http://") || prefix.StartsWith("https://")) {
                // Load from URL
                LoadImageFromUrl(filepath);
            }
            else {
                // Load from local disk
                LoadImage(filepath);
            }
        }

        /// <summary>
        /// Create a KalikoImage from a stream.
        /// </summary>
        /// <param name="stream"></param>
        public KalikoImage(Stream stream) {
            LoadImage(stream);
        }

        public void Destroy() {
            if(_g != null)
                _g.Dispose();
            if(_image != null)
                _image.Dispose();
        }


        #endregion


        #region Public properties

        public Image Image {
            get {
                return _image;
            }
            set {
                _image = value;
            }
        }


        public int Width {
            get {
                return _image.Width;
            }
        }

        public int Height {
            get {
                return _image.Height;
            }
        }


        public Color BackgroundColor {
            get {
                return _backgroundColor;
            }
            set {
                _backgroundColor = value;
            }
        }


        public Size Size {
            get {
                return _image.Size;
            }
        }


        public Color Color {
            get {
                return _color;
            }
            set {
                _color = value;
            }
        }


        public TextRenderingHint TextRenderingHint {
            get {
                return _textrenderinghint;
            }
            set {
                _textrenderinghint = value;
            }
        }

        public bool IsPortrait {
            get { return Width < Height; }
        }

        public bool IsLandscape {
            get { return Width > Height; }
        }

        public bool IsSquare {
            get { return Width == Height; }
        }


        #endregion


        #region Common image functions

        /// <summary>
        /// Create an exact copy of this Kaliko.ImageLibrary.KalikoImage
        /// </summary>
        /// <returns></returns>
        public KalikoImage Clone() {
            // Create new image from the old one
            KalikoImage newImage = new KalikoImage(_image);
            
            // Set all private variables to same as the current instance
            newImage._color = _color;
            newImage._font = _font;
            newImage._backgroundColor = _backgroundColor;
            newImage._textrenderinghint = _textrenderinghint;

            return newImage;
        }

        /// <summary>
        /// Instanciate an empty image of the requested resolution.
        /// </summary>
        /// <param name="width">Width of the new image</param>
        /// <param name="height">Height of the new image</param>
        private void CreateImage(int width, int height) {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            _image = bitmap;
            _g = Graphics.FromImage(_image);
        }

        #endregion


        #region Functions for text

        /// <summary>
        /// Load a font for further use.
        /// </summary>
        /// <param name="fileName">Path to font file</param>
        /// <param name="size">Font size</param>
        /// <param name="fontStyle">Font style</param>
        /// <example>
        /// KalikoImage image = new KalikoImage(200, 200);
        /// image.SetFont("c:\\fontpath\\arial.ttf", 12f, FontStyle.Regular);
        /// image.WriteText("Hello world!", 0, 10);
        /// </example>
        public void SetFont(string fileName, float size, FontStyle fontStyle) {
            PrivateFontCollection pf = new PrivateFontCollection();
            pf.AddFontFile(fileName);
            _font = new Font(pf.Families[0], size, fontStyle);
        }


        public void WriteText(string txt, int x, int y) {
            _g.TextRenderingHint = _textrenderinghint;
            _g.DrawString(txt, _font, new SolidBrush(_color), new Point(x, y));
        }


        public void WriteText(string txt, int x, int y, float angle) {
            _g.TextRenderingHint = _textrenderinghint;
            _g.TranslateTransform(x, y);
            _g.RotateTransform(angle);
            _g.DrawString(txt, _font, new SolidBrush(_color), new Point(0, 0));
            _g.ResetTransform();
        }

        #endregion


        #region Functions for loading images (from file, stream or web)

        /// <summary>
        /// Load an image from local disk
        /// </summary>
        /// <param name="fileName">File path</param>
        public void LoadImage(string fileName) {
            _image = Image.FromFile(fileName);

            if (DoesImageNeedToBeConverted) {
                ConvertImageToTrueColor();
            }

            _g = Graphics.FromImage(_image);
        }

        /// <summary>
        /// Load an image from a stream object (MemoryStream, Stream etc)
        /// </summary>
        /// <param name="stream">Pointer to stream</param>
        public void LoadImage(Stream stream) {
            _image = Image.FromStream(stream);

            if (DoesImageNeedToBeConverted) {
                ConvertImageToTrueColor();
            }

            _g = Graphics.FromImage(_image);
        }

        /// <summary>
        /// Load an image from an URL
        /// </summary>
        /// <param name="url"></param>
        public void LoadImageFromUrl(string url) {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;

            Stream source = request.GetResponse().GetResponseStream();
            MemoryStream ms = new MemoryStream();

            byte[] data = new byte[256];
            int c = source.Read(data, 0, data.Length);

            while(c > 0) {
                ms.Write(data, 0, c);
                c = source.Read(data, 0, data.Length);
            }

            source.Close();
            ms.Position = 0;
            
            LoadImage(ms);

            ms.Close();
        }

        private void ConvertImageToTrueColor() {
            using (_image) {
                using (Bitmap bitmap = new Bitmap(_image)) {
                    _image = new Bitmap(bitmap);
                }
            }
        }

        protected bool DoesImageNeedToBeConverted {
            get {
                switch (_image.PixelFormat) {
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppPArgb:
                        return false;
                    default:
                        return true;
                }
            }
        }

        #endregion


        #region Primitive drawing functions like clear, fill etc

        public void Clear(Color color) {
            _g.Clear(color);
        }


        public void GradientFill(Color colorFrom, Color colorTo) {
            GradientFill(new Point(0, 0), new Point(0, _image.Height), colorFrom, colorTo);
        }


        public void GradientFill(Point pointFrom, Point pointTo, Color colorFrom, Color colorTo) {
            Brush brush = new LinearGradientBrush(pointFrom, pointTo, colorFrom, colorTo);
            _g.FillRectangle(brush, 0, 0, _image.Width, _image.Height);
        }

        #endregion


        #region Functions for thumbnail creation

        public KalikoImage GetThumbnailImage(int width, int height) {
            return GetThumbnailImage(width, height, ThumbnailMethod.Crop);
        }


        public KalikoImage GetThumbnailImage(int width, int height, ThumbnailMethod method) {
            KalikoImage image;
            double imageRatio = (double)_image.Width / _image.Height;
            double thumbRatio = (double)width / height;

            if(method == ThumbnailMethod.Crop ) {
                int imgWidth = width;
                int imgHeight = height;

                if(imageRatio < thumbRatio) {
                    imgHeight = (_image.Height * width) / _image.Width;
                }
                else {
                    imgWidth = (_image.Width * height) / _image.Height;
                }

                image = new KalikoImage(width, height);
                DrawScaledImage(image._image, _image, (width - imgWidth) / 2, (height - imgHeight) / 2, imgWidth, imgHeight);
            }
            else if(method == ThumbnailMethod.Pad) {
                // Rewritten to fix issue #1. Thanks to Cosmin!
                float hRatio = _image.Height / (float)height;
                float wRatio = _image.Width / (float)width;
                float newRatio = hRatio > wRatio ? hRatio : wRatio;
                int imgHeight = (int)(_image.Height / newRatio);
                int imgWidth = (int)(_image.Width / newRatio);

                image = new KalikoImage(width, height, _backgroundColor);
                DrawScaledImage(image._image, _image, (width - imgWidth) / 2, (height - imgHeight) / 2, imgWidth, imgHeight);
            }
            else { // ThumbnailMethod.Fit
                if(imageRatio < thumbRatio) {
                    width = (_image.Width * height) / _image.Height;
                }
                else {
                    height = (_image.Height * width) / _image.Width;
                }

                image = new KalikoImage(width, height);
                DrawScaledImage(image._image, _image, 0, 0, width, height);
            }

            return image;
        }

        private static void DrawScaledImage(Image destImage, Image sourceImage, int x, int y, int width, int height) {
            using(Graphics g = Graphics.FromImage(destImage)) {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using (ImageAttributes wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(sourceImage, new Rectangle(x, y, width, height), 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
        }

        #endregion


        #region Functions for blitting

        public void BlitImage(string fileName) {
            BlitImage(fileName, 0, 0);
        }


        public void BlitImage(string fileName, int x, int y) {
            Image mark = Image.FromFile(fileName);
            BlitImage(mark, x, y);
            mark.Dispose();
        }


        public void BlitImage(KalikoImage image) {
            BlitImage(image._image, 0, 0);
        }


        public void BlitImage(KalikoImage image, int x, int y) {
            BlitImage(image._image, x, y);
        }


        public void BlitImage(Image image) {
            BlitImage(image, 0, 0);
        }


        public void BlitImage(Image image, int x, int y) {
            _g.DrawImageUnscaled(image, x, y);
        }



        public void BlitFill(string fileName) {
            Image mark = Image.FromFile(fileName);
            BlitFill(mark);
            mark.Dispose();
        }


        public void BlitFill(KalikoImage image) {
            BlitFill(image._image);
        }


        public void BlitFill(Image image) {
            int width = image.Width;
            int height = image.Height;
            int columns = (int)Math.Ceiling((float)_image.Width / width);
            int rows = (int)Math.Ceiling((float)_image.Width / width);

            for(int y = 0;y < rows;y++) {
                for(int x = 0;x < columns;x++) {
                    _g.DrawImageUnscaled(image, x * width, y * height);
                }
            }
        }

        #endregion


        #region Functions for image saving and streaming

        private static ImageCodecInfo GetEncoderInfo(String mimeType) {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            for(int j = 0, l = encoders.Length;j < l;++j) {
                if(encoders[j].MimeType == mimeType) {
                    return encoders[j];
                }
            }
            return null;
        }


        public void StreamJpg(long quality, string fileName) {
            SaveJpg(PrepareImageStream(fileName, "image/jpeg"), quality);
        }


        public void StreamGif(string fileName) {
            SaveGif(PrepareImageStream(fileName, "image/gif"));
        }


        private static Stream PrepareImageStream(string fileName, string mime) {
            System.Web.HttpResponse stream = System.Web.HttpContext.Current.Response;
            stream.Clear();
            stream.ClearContent();
            stream.ClearHeaders();
            stream.ContentType = mime;
            stream.AddHeader("Content-Disposition", "inline;filename=" + fileName);
            return stream.OutputStream;
        }

        public void SaveJpg(Stream stream, long quality) {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/jpeg");
            _image.Save(stream, ic, encparam);
        }


        public void SaveJpg(string fileName, long quality) {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/jpeg");
            _image.Save(fileName, ic, encparam);
        }


        public void SavePng(Stream stream, long quality) {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/png");
            _image.Save(stream, ic, encparam);
        }

        
        public void SavePng(string fileName, long quality) {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/png");
            _image.Save(fileName, ic, encparam);
        }


        public void SaveGif(Stream stream) {
            _image.Save(stream, ImageFormat.Gif);
        }


        public void SaveGif(string fileName) {
            _image.Save(fileName, ImageFormat.Gif);
        }


        public void SaveBmp(Stream stream) {
            _image.Save(stream, ImageFormat.Bmp);
        }


        public void SaveBmp(string fileName) {
            _image.Save(fileName, ImageFormat.Bmp);
        }


        public void SaveBmp(Stream stream, ImageFormat format) {
            _image.Save(stream, format);
        }


        public void SaveImage(string fileName, ImageFormat format) {
            _image.Save(fileName, format);
        }


        #endregion


        #region Functions for filters and bitmap manipulation

        private byte[] _byteArray;
        private bool _disposed;

        /// <summary>
        /// ByteArray matching PixelFormat.Format32bppArgb (bgrA in real life)
        /// </summary>
        public byte[] ByteArray {
            get {
                if(_byteArray == null) {
                    BitmapData data = ((Bitmap)_image).LockBits(new Rectangle(0, 0, _image.Width, _image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    int length = _image.Width * _image.Height * 4;
                    _byteArray = new byte[length];

                    if(data.Stride == _image.Width * 4) {
                        Marshal.Copy(data.Scan0, _byteArray, 0, length);
                    }
                    else {
                        for(int i = 0, l = _image.Height;i < l;i++) {
                            IntPtr p = new IntPtr(data.Scan0.ToInt32() + data.Stride * i);
                            Marshal.Copy(p, _byteArray, i * _image.Width * 4, _image.Width * 4);
                        }
                    }

                    ((Bitmap)_image).UnlockBits(data);
                }
                return _byteArray;
            }
            set {
                BitmapData data = ((Bitmap)_image).LockBits(new Rectangle(0, 0, _image.Width, _image.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                if(data.Stride == _image.Width * 4) {
                    Marshal.Copy(value, 0, data.Scan0, value.Length);
                }
                else {
                    for(int i = 0, l = _image.Height;i < l;i++) {
                        IntPtr p = new IntPtr(data.Scan0.ToInt32() + data.Stride * i);
                        Marshal.Copy(value, i * _image.Width * 4, p, _image.Width * 4);
                    }
                }

                ((Bitmap)_image).UnlockBits(data);
            }
        }



        public void ApplyFilter(Filters.IFilter filter) {
            filter.Run(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    if(_font != null) {
                        _font.Dispose();
                    }
                    if(_g != null) {
                        _g.Dispose();
                    }
                    if(_image != null) {
                        _image.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        ~KalikoImage() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}

