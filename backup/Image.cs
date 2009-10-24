using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Web;
using System.Net;
using System.IO;

namespace Kaliko
{
    public enum Direction
    {
        top ,
        bottom,
        left,
        right
    }

    public class Image
    {
        private System.Drawing.Image _image;
        private System.Drawing.Graphics _g;
        private System.Drawing.Font _font;
        private System.Drawing.Color _color;


        public Image()
        {
        }


        public System.Drawing.Image getImage()
        {
            return _image;
        }

        public Size getSize()
        {
            return _image.Size;
        }

        public void SetImage(System.Drawing.Image img)
        {
            _image = img;
        }

        public void Crop(int x, int y, int width, int height)
        {
            if (x + width > _image.Width)
                width = _image.Width - x;
            if (y + height > _image.Height)
                height = _image.Height - y;

            Rectangle cropArea = new Rectangle(x, y, width, height);
            Bitmap bmpImage = new Bitmap(_image);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            _image = (System.Drawing.Image)(bmpCrop);
            bmpImage.Dispose();
        }

        /// <summary>
        /// public void Padd(int width, int height)
        /// Padd the picture if it's smaller then width, height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Padd(int width, int height, bool scaleifbigger)
        {
            //The picture is allready that size or bigger
            if (_image.Width >= width && _image.Height >= height)
            {
                if (scaleifbigger)
                    Scale(width, height);
                return;
            }
            
            Bitmap b = new Bitmap(width, height);

            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            
            
            SolidBrush brush = new SolidBrush(_color);
            g.FillRectangle(brush, 0, 0, width, height);

            int x = (int)(width - _image.Width) / 2;
            int y = (int)(height - _image.Height) / 2;

            g.DrawImage(_image, x, y);
            g.Dispose();

            _image = (System.Drawing.Image)b;
        }

       


        /// <summary>
        /// public void Padd(int width)
        /// Padds the picture if it's smaller then width. It keeps the proportions.
        /// </summary>
        /// <param name="width"></param>
        public void PaddWidth(int width, bool scaleifbigger)
        {
            //The picture is allready that size or bigger
            if (_image.Width >= width && !scaleifbigger)
                return;
            float fH = ((float)width *(float)_image.Height) / (float)_image.Width;
            int height = (int)fH;
            Padd(width, height, scaleifbigger);
        }
        

        public void Padd(int maxsize, bool scaleifbigger)
        {
            //The picture is allready that size or bigger
            if ((_image.Width >= maxsize || _image.Height >= maxsize ) && !scaleifbigger)
                return;
            int height;
            int width;
            if (_image.Width > _image.Height)
            {
                
                float fH = ((float)maxsize * (float)_image.Height) /(float)_image.Width ;
                height = (int)fH;
                width = maxsize;
            }
            else
            {
                float fW = ((float)maxsize * (float)_image.Width / (float)_image.Height);
                width = (int)fW;
                height = maxsize;
            }
            Padd(width, height, scaleifbigger);
        }


        public bool ThumbnailCallback()
        {
            return true;
        }


        public Image getThumb(int width, int height)
        {
            Kaliko.Image img = new Image();
            img._image = _image.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            return img;
        }

        public Image getThumb(int maxsize)
        {
            /*
            int height;
            int width;

            if (_image.Width > _image.Height)
            {
                float fH = ((float)maxsize *(float)_image.Height) / (float)_image.Width ;
                height = (int)fH;
                width = maxsize;
            }
            else
            {
                float fW = ((float)maxsize * (float)_image.Width / (float)_image.Height);
                width = (int)fW;
                height = maxsize;
            }
            */
            System.Drawing.Image tmpImage = _image;
            Color tmpCol = _color;

            SetColor(Color.White);
            Padd(maxsize, true);
            Padd(100,100, true);
            Kaliko.Image img = new Image();
            img._image = _image;
            _image = tmpImage;
            _color = tmpCol;

            return img;

            /*
            Kaliko.Image img = new Image();
            img._image = _image.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            return img;*/
        }

        public void ShrinkWidth(int width)
        {
            //The picture is allready that size or bigger
            if (_image.Width > width)
            {
                float fH = ((float)width * (float)_image.Height) / (float)_image.Width;
                int height = (int)fH;
                Scale(width, height);
            }
        }

        public void ShrinkHeight(int height)
        {
            //The picture is allready that size or bigger
            if (_image.Height > height)
            {
                float fW = ((float)height * (float)_image.Width) / (float)_image.Height;
                int width = (int)fW;
                Scale(width, height);
            }
        }

        public void Shrink(int width, int height)
        {

            if (_image.Height > height && _image.Width > width)
            {
                float nPercentW = ((float)width / (float)_image.Width);
                float nPercentH = ((float)height / (float)_image.Height);
                float nPercent;
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                }
                else
                {
                    nPercent = nPercentW;
                }

                width = (int)(_image.Width * nPercent);
                height = (int)(_image.Height * nPercent);
                Scale(width, height);
            }
            else if (_image.Height > height)
            {
                float fW = ((float)height * (float)_image.Width) / (float)_image.Height;
                width = (int)fW;
                Scale(width, height);
            }
            else if (_image.Width > width)
            {
                float fH = ((float)width * (float)_image.Height) / (float)_image.Width;
                height = (int)fH;
                Scale(width, height);
            }
            else
                return;
        }

        public void Scale(int width, int height)
        {
            int sourceWidth = _image.Width;
            int sourceHeight = _image.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)width / (float)sourceWidth);
            nPercentH = ((float)height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int) (sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            if (destWidth < 1)
                destWidth = 1;
            if (destHeight < 1)
                destHeight = 1;

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(_image, 0, 0, destWidth, destHeight);
            g.Dispose();

            _image = (System.Drawing.Image)b;
        }


        public void SetColor(System.Drawing.Color color)
        {
            _color = color;
        }

        public void MeldImage(string fileName, Direction dir)
        {
            
            Bitmap newimg = new Bitmap(fileName);
            Bitmap b;
            if(dir == Direction.left || dir == Direction.right)
                b = new Bitmap(newimg.Size.Width+_image.Size.Width, newimg.Size.Height>_image.Size.Height?newimg.Size.Height:_image.Size.Height);
            else
                b = new Bitmap(newimg.Size.Width > _image.Size.Width ? newimg.Size.Width : _image.Size.Width, newimg.Size.Height + _image.Size.Height);
            //draw the original image
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);

            if (dir == Direction.right)
            {
                g.DrawImage(_image, 0, 0);
                g.DrawImage((System.Drawing.Image)newimg, _image.Size.Width, 0);
            }
            else if (dir == Direction.left)
            {
                g.DrawImage(_image, newimg.Size.Width, 0);
                g.DrawImage((System.Drawing.Image)newimg, 0, 0);
            }
            else if (dir == Direction.top)
            {
                g.DrawImage(_image, 0, newimg.Size.Height);
                g.DrawImage((System.Drawing.Image)newimg, 0, 0);
            }
            else //bottom
            {
                g.DrawImage(_image, 0, 0);
                g.DrawImage((System.Drawing.Image)newimg, 0, _image.Size.Height);
            }
            
            g.Dispose();

            _image = (System.Drawing.Image)b;
        }


        public void BlitImage(string fileName)
        {
            BlitImage(fileName, 0, 0);
        }

        public void BlitImage(string fileName, int x, int y)
        {
            System.Drawing.Image mark = System.Drawing.Image.FromFile(fileName);
            Graphics g = Graphics.FromImage(_image);
            g.DrawImage(mark, x, y);
            g.Dispose();
            mark.Dispose();
        }

        public void SetFont(string fileName, float size, FontStyle fontStyle)
        {
            System.Drawing.Text.PrivateFontCollection pf = new System.Drawing.Text.PrivateFontCollection();
            pf.AddFontFile(fileName);
            _font = new Font(pf.Families[0], size, fontStyle);
        }

        public void WriteText(string txt, int x, int y)
        {
            _g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit; //.AntiAlias;
            _g.DrawString(txt, _font, new System.Drawing.SolidBrush(_color), new Point(x, y));
        }

        public void WriteText(string txt,int x,int y,TextRenderingHint antialias) {
            _g.TextRenderingHint = antialias;
            _g.DrawString(txt,_font,new System.Drawing.SolidBrush(_color),new Point(x,y));
        }

        public void WriteCenteredText(string txt, int x, int y)
        {
            _g.TextRenderingHint = TextRenderingHint.AntiAlias;
            SizeF size = _g.MeasureString(txt, _font);
            _g.DrawString(txt, _font, new System.Drawing.SolidBrush(_color), new Point(x - (int)(size.Width / 2), y));
        }


        public void LoadImage(string fileName)
        {
            _image = System.Drawing.Image.FromFile(fileName);


            if (!IsIndexed())
                _g = Graphics.FromImage(_image);
            else
            {
                Bitmap img = new Bitmap(new Bitmap(_image));
                _g = Graphics.FromImage((System.Drawing.Image)img);
                //Test Addes since writing text to imgae do not work for indexed images.. maybe we need to  create the _g after we maid the new image
                _image = (System.Drawing.Image)img;               
            }
        }

        public void LoadImageFromURL(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;

            Stream source = request.GetResponse().GetResponseStream();
            MemoryStream ms = new MemoryStream();

            byte[] data = new byte[256];
            int c = source.Read(data, 0, data.Length);

            while (c > 0)
            {
                ms.Write(data, 0, c);
                c = source.Read(data, 0, data.Length);
            }

            source.Close();
            ms.Position = 0;
            _image = System.Drawing.Image.FromStream(ms);






            /*

            const int BYTESTOREAD = 10000;
            WebRequest myRequest = WebRequest.Create(URL);
            WebResponse myResponse = myRequest.GetResponse();
            Stream ReceiveStream = myResponse.GetResponseStream();
            BinaryReader br = new BinaryReader(ReceiveStream);
            MemoryStream memstream = new MemoryStream();
            byte[] bytebuffer = new byte[BYTESTOREAD];
            int BytesRead = br.Read(bytebuffer, 0, BYTESTOREAD);
            while (BytesRead > 0)
            {
                memstream.Write(bytebuffer, 0, BytesRead);
                BytesRead = br.Read(bytebuffer, 0, BYTESTOREAD);
            }
            _image = System.Drawing.Image.FromStream(memstream);
             */
        } 
      

        public bool IsIndexed()
        {
            if (
                _image.PixelFormat == PixelFormat.Undefined ||
                _image.PixelFormat == PixelFormat.DontCare ||
                _image.PixelFormat == PixelFormat.Format1bppIndexed ||
                _image.PixelFormat == PixelFormat.Format4bppIndexed ||
                _image.PixelFormat == PixelFormat.Format8bppIndexed ||
                _image.PixelFormat == PixelFormat.Format16bppGrayScale ||
                _image.PixelFormat == PixelFormat.Format16bppArgb1555)
                    return true;

                return false;
        }
     


        public void CreateImageFromText(string txt, int paddingy, int paddingx, System.Drawing.Color bgcolor)
        {
            SizeF sz = GetTextSize(txt);
            int _width = ((int)Math.Ceiling(sz.Width)) + (paddingx << 1);
            int _height = ((int)Math.Ceiling(sz.Height)) + (paddingy << 1);

            System.Drawing.Bitmap bitmap = new Bitmap(_width, _height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            _image = (System.Drawing.Image)bitmap;
            _g = Graphics.FromImage(_image);

            _g.FillRectangle(new SolidBrush(bgcolor), 0, 0, _width, _height);
            WriteText(txt, paddingx, paddingy);
            Trim();
        }

        public void Trim()
        {
            bool foundit = false;
            int top;
            int bottom = _image.Height - 1;
            int left = 0;
            int right = _image.Width - 1;

            Bitmap bmp = new Bitmap(_image);
            Color clr = bmp.GetPixel(0, 0);
            for (top = 0; top < _image.Height; top++)
            {
                for (int x = 0; x < _image.Width; x++)
                {
                    if (bmp.GetPixel(x, top) != clr)
                    {
                        foundit = true;
                        break;
                    }
                }
                if (foundit) break;
            }

            foundit = false;
            for (bottom = _image.Height - 1; bottom > 0; bottom--)
            {
                for (int x = 0; x < _image.Width; x++)
                {
                    if (bmp.GetPixel(x, bottom) != clr)
                    {
                        foundit = true;
                        break;
                    }
                }
                if (foundit) break;
            }

            foundit = false;
            for (left = 0; left < _image.Width; left++)
            {
                for (int y = 0; y < _image.Height; y++)
                {
                    if (bmp.GetPixel(left, y) != clr)
                    {
                        foundit = true;
                        break;
                    }
                }
                if (foundit) break;
            }

            foundit = false;
            for (right = _image.Width - 1; right > 0; right--)
            {
                for (int y = 0; y < _image.Height; y++)
                {
                    if (bmp.GetPixel(right, y) != clr)
                    {
                        foundit = true;
                        break;
                    }
                }
                if (foundit) break;
            }

            if (top > 0) top--;
            if (bottom < _image.Height) bottom++;
            if (left > 0) left--;
            if (right < _image.Width) right++;

            if ((bottom < top) || (left > right)) return;



            Rectangle cropArea = new Rectangle(left, top, right - left, bottom - top);
            Bitmap bmpImage = new Bitmap(_image);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            _image = (System.Drawing.Image)(bmpCrop);
            bmpImage.Dispose();

            
            /*

            _g.Dispose();
            _image.Dispose();
            _image = (System.Drawing.Image)(new Bitmap(right - left, bottom - top, System.Drawing.Imaging.PixelFormat.Format24bppRgb));
            _g = Graphics.FromImage(_image);
            _g.DrawImageUnscaled(bmp, -left, -top, right - left, bottom - top);

            bmp.Dispose();*/
        }

        public void CreateImage(int width, int height)
        {
            System.Drawing.Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            _image = (System.Drawing.Image)bitmap;
            _g = Graphics.FromImage(_image);
        }


        public SizeF GetTextSize(string txt)
        {
            Bitmap bmp;
            bool newgfx = false;

            if (_g == null)
            {
                bmp = new Bitmap(8, 8);
                _g = Graphics.FromImage(bmp);
                newgfx = true;
            }
            _g.TextRenderingHint = TextRenderingHint.AntiAlias;
            SizeF sz = _g.MeasureString(txt, _font);
            if (newgfx)
                _g.Dispose();

            return (sz);

        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public void Clear(System.Drawing.Color color)
        {
            _g.Clear(color);
        }

        public void StreamJPG(long quality, string filename)
        {
            System.Web.HttpResponse stream = System.Web.HttpContext.Current.Response;
            stream.Clear();
            stream.ClearContent();
            stream.ClearHeaders();
            stream.ContentType = "Image/jpeg";
            stream.AddHeader("Content-Disposition", "inline;filename=" + filename);
            SaveJPG(stream.OutputStream, quality);
        }

        public void StreamGIF(long quality, string filename)
        {
            System.Web.HttpResponse stream = System.Web.HttpContext.Current.Response;
            stream.Clear();
            stream.ClearContent();
            stream.ClearHeaders();
            stream.ContentType = "image/gif";
            stream.AddHeader("Content-Disposition", "inline;filename=" + filename);
            SaveGif(stream.OutputStream, quality);
        }

        public void SaveJPG(System.IO.Stream stream, long quality)
        {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/jpeg");
            _image.Save(stream, ic, encparam);
        }

        public void SaveJPG(string filename, long quality)
        {
            EncoderParameters encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo ic = GetEncoderInfo("image/jpeg");
            _image.Save(filename, ic, encparam);
        }


        public void SaveGif(System.IO.Stream stream, long quality)
        {
            Kaliko.ImageTools.OctreeQuantizer quantizer = new Kaliko.ImageTools.OctreeQuantizer(255, 8);
            Bitmap quantized = quantizer.Quantize(_image);
            quantized.Save(stream, ImageFormat.Gif);
        }

        public void SaveGif(string fileName, long quality)
        {
            Kaliko.ImageTools.OctreeQuantizer quantizer = new Kaliko.ImageTools.OctreeQuantizer(255, 8);
            Bitmap quantized = quantizer.Quantize(_image);
            quantized.Save(fileName, ImageFormat.Gif);
        }

      

        public void Destroy()
        {
            if (_g != null)
                _g.Dispose();
            if (_image != null)
                _image.Dispose();
        }

        public void GradientFill()
        {
            Brush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, _image.Height), Color.FromArgb(100, 100, 100), Color.FromArgb(180, 180, 180));
            _g.FillRectangle(brush, 0, 0, _image.Width, _image.Height);
        }


    }
}

