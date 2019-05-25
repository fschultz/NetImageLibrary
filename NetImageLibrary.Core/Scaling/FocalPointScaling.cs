#pragma warning disable 1591
namespace Kaliko.ImageLibrary.Scaling {
    using System;
    using System.Drawing;

    /// <summary>
    /// Focal point scaling
    /// </summary>
    public class FocalPointScaling : ScalingBase {
        private readonly double _focalPointX;
        private readonly double _focalPointY;
        private readonly Size _targetSize;

        public FocalPointScaling(int targetWidth, int targetHeight, double focalPointX, double focalPointY) : base(targetWidth, targetHeight) {
            _focalPointX = focalPointX;
            _focalPointY = focalPointY;
            _targetSize = new Size(targetWidth, targetHeight);
        }

        internal override Size CalculateNewImageSize(Size originalSize) {
            var targetRatio = GetRatio(_targetSize);
            var originalRatio = GetRatio(originalSize);

            var size = new Size(_targetSize.Width, _targetSize.Height);

            if (originalRatio < targetRatio)
            {
                size.Height = (originalSize.Height * _targetSize.Width) / originalSize.Width;
            }
            else
            {
                size.Width = (originalSize.Width * _targetSize.Height) / originalSize.Height;
            }

            return size;
        }

        internal override KalikoImage DrawResizedImage(KalikoImage sourceImage, Size calculatedSize, Size originalSize) {
            var resizedImage = new KalikoImage(_targetSize, sourceImage.BackgroundColor);

            var scaledFocalPointX = _focalPointX / ((double)(calculatedSize.Width - _targetSize.Width) / calculatedSize.Width);
            var scaledFocalPointY = _focalPointY / ((double)(calculatedSize.Height - _targetSize.Height) / calculatedSize.Height);

            var sourceX = (int)Math.Round((_targetSize.Width - calculatedSize.Width) * scaledFocalPointX) + (_targetSize.Width / 2);
            if (sourceX < _targetSize.Width - calculatedSize.Width) {
                sourceX = _targetSize.Width - calculatedSize.Width;
            }
            else if (sourceX > 0) {
                sourceX = 0;
            }

            var sourceY = (int)Math.Round((_targetSize.Height - calculatedSize.Height) * scaledFocalPointY) + (_targetSize.Height / 2);
            if (sourceY < _targetSize.Height - calculatedSize.Height) {
                sourceY = _targetSize.Height - calculatedSize.Height;
            }
            else if (sourceY > 0) {
                sourceY = 0;
            }

            KalikoImage.DrawScaledImage(resizedImage, sourceImage, sourceX, sourceY, calculatedSize.Width, calculatedSize.Height);

            return resizedImage;
        }
    }
}