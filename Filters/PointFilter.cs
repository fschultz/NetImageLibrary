namespace Kaliko.ImageLibrary.Filters {
    public abstract class PointFilter {

        protected bool CanFilterIndexColorModel = false;

        public void ApplyFilter(KalikoImage image) {
            var pixels = image.IntArray;

            var width = image.Width;
            var height = image.Height;

            for (var y = 0; y < height; y++) {
                // We try to avoid calling getRGB on images as it causes them to become unmanaged, causing horrible performance problems.
                //if (type == BufferedImage.TYPE_INT_ARGB) {
                //    srcRaster.getDataElements(0, y, width, 1, inPixels);
                //    for (int x = 0; x < width; x++)
                //        inPixels[x] = filterRGB(x, y, inPixels[x]);
                //    dstRaster.setDataElements(0, y, width, 1, inPixels);
                //}
                //else {
                //    src.getRGB(0, y, width, 1, inPixels, 0, width);
                    for (var x = 0; x < width; x++) {
                        pixels[x] = (int)FilterRgb(x, y, (uint)pixels[x]);
                    }
                //    dst.setRGB(0, y, width, 1, inPixels, 0, width);
                //}
            }

            image.IntArray = pixels;
        }

        public void SetDimensions(int width, int height) {
        }

        public abstract uint FilterRgb(int x, int y, uint rgb);
    }
}