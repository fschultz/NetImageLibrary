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

    public class KalikoImage {
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
        /// Private constructor
        /// </summary>
        /// <remarks>
        /// This constructor is not public to ensure that a KalikoImage 
        /// object always will have an image and a graphics object.
        /// </remarks>
        private KalikoImage() {
        }

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
                LoadImageFromURL(filepath);
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


        public System.Drawing.Color Color {
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


        public bool IndexedPalette {
            get {
                switch(_image.PixelFormat) {
                    case PixelFormat.Undefined:
                    case PixelFormat.Format1bppIndexed:
                    case PixelFormat.Format4bppIndexed:
                    case PixelFormat.Format8bppIndexed:
                    case PixelFormat.Format16bppGrayScale:
                    case PixelFormat.Format16bppArgb1555:
                        return true;
                    default:
                        return false;
                }
            }
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


        private void CreateImage(int width, int height) {
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            _image = (Image)bitmap;
            _g = Graphics.FromImage(_image);
        }

        #endregion


        #region Functions for text

        public void SetFont(string fileName, float size, FontStyle fontStyle) {
            System.Drawing.Text.PrivateFontCollection pf = new System.Drawing.Text.PrivateFontCollection();
            pf.AddFontFile(fileName);
            _font = new Font(pf.Families[0], size, fontStyle);
        }


        public void WriteText(string txt, int x, int y) {
            _g.TextRenderingHint = _textrenderinghint;
            _g.DrawString(txt, _font, new System.Drawing.SolidBrush(_color), new Point(x, y));
        }

        #endregion


        #region Functions for loading images (from file, stream or web)

        /// <summary>
        /// Load an image from local disk
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadImage(string fileName) {
            _image = Image.FromFile(fileName);

            MakeImageNonIndexed();

            _g = Graphics.FromImage(_image);
        }

        /// <summary>
        /// Load an image from a stream object (MemoryStream, Stream etc)
        /// </summary>
        /// <param name="stream"></param>
        public void LoadImage(Stream stream) {
            _image = System.Drawing.Image.FromStream(stream);

            MakeImageNonIndexed();

            _g = Graphics.FromImage(_image);
        }

        /// <summary>
        /// Load an image from an URL
        /// </summary>
        /// <param name="url"></param>
        public void LoadImageFromURL(string url) {
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

        /// <summary>
        /// Check if image has an indexed palette and if so convert to truecolor
        /// </summary>
        private void MakeImageNonIndexed() {
            if(IndexedPalette) {
                _image = (Image)new Bitmap(new Bitmap(_image));
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
            double imageRatio = _image.Width / _image.Height;
            double thumbRatio = width / height;

            if(method == ThumbnailMethod.Crop ) {
                int imgWidth = width;
                int imgHeight = height;

                if(imageRatio > thumbRatio) {
                    imgHeight = (_image.Height * width) / _image.Width;
                }
                else {
                    imgWidth = (_image.Width * height) / _image.Height;
                }

                image = new KalikoImage(width, height);
                Graphics g = Graphics.FromImage(image._image);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(_image, (width - imgWidth) / 2, (height - imgHeight) / 2, imgWidth, imgHeight);
            }
            else if(method == ThumbnailMethod.Pad) {
                // Rewritten to fix issue #1. Thanks to Cosmin!
                float hRatio = (float)_image.Height / (float)height;
                float wRatio = (float)_image.Width / (float)width;
                float newRatio = hRatio > wRatio ? hRatio : wRatio;
                int imgHeight = (int)(_image.Height / newRatio);
                int imgWidth = (int)(_image.Width / newRatio);

                image = new KalikoImage(width, height, _backgroundColor);
                Graphics g = Graphics.FromImage(image._image);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(_image, (width - imgWidth) / 2, (height - imgHeight) / 2, imgWidth, imgHeight);
            }
            else { // ThumbnailMethod.Fit
                if(imageRatio > thumbRatio) {
                    width = (_image.Width * height) / _image.Height;
                }
                else {
                    height = (_image.Height * width) / _image.Width;
                }

                image = new KalikoImage(width, height);
                Graphics g = Graphics.FromImage(image._image);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(_image, 0, 0, width, height);
            }

            return image;
        }

        #endregion


        #region Functions for blitting

        public void BlitImage(string fileName) {
            BlitImage(fileName, 0, 0);
        }


        public void BlitImage(string fileName, int x, int y) {
            System.Drawing.Image mark = System.Drawing.Image.FromFile(fileName);
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
            System.Drawing.Image mark = System.Drawing.Image.FromFile(fileName);
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
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for(int j = 0, l = encoders.Length;j < l;++j) {
                if(encoders[j].MimeType == mimeType) {
                    return encoders[j];
                }
            }
            return null;
        }


        public void StreamJPG(long quality, string filename) {
            SaveJPG(PrepareImageStream(filename, "image/jpeg"), quality);
        }


        public void StreamGIF(string filename) {
            SaveGif(PrepareImageStream(filename, "image/gif"));
        }


        private Stream PrepareImageStream(string filename, string mime) {
            System.Web.HttpResponse stream = System.Web.HttpContext.Current.Response;
            stream.Clear();
            stream.ClearContent();
            stream.ClearHeaders();
            stream.ContentType = mime;
            stream.AddHeader("Content-Disposition", "inline;filename=" + filename);
            return stream.OutputStream;
        }

        public void SaveJPG(Stream stream, long quality) {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/jpeg");
            _image.Save(stream, ic, encparam);
        }


        public void SaveJPG(string filename, long quality) {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/jpeg");
            _image.Save(filename, ic, encparam);
        }


        public void SavePNG(string filename, long quality) {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/png");
            _image.Save(filename, ic, encparam);
        }


        public void SaveGif(System.IO.Stream stream) {
            _image.Save(stream, ImageFormat.Gif);
        }


        public void SaveGif(string fileName) {
            _image.Save(fileName, ImageFormat.Gif);
        }


        #endregion


        #region Functions for filters and bitmap manipulation

        private byte[] _byteArray;

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
            filter.run(this);
        }

        #endregion


    }
}

