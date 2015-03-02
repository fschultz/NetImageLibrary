namespace Kaliko.ImageLibrary.Filters {
    public abstract class TransferFilter : PointFilter {

        protected int[] rTable, gTable, bTable;
        protected bool Initialized = false;

        protected TransferFilter() {
            CanFilterIndexColorModel = true;
        }

        public override uint FilterRgb(int x, int y, uint rgb) {
            uint a = rgb & 0xff000000;
            uint r = (rgb >> 16) & 0xff;
            uint g = (rgb >> 8) & 0xff;
            uint b = rgb & 0xff;
            r = (uint)rTable[r];
            g = (uint)gTable[g];
            b = (uint)bTable[b];
            return a | (r << 16) | (g << 8) | b;
        }

        //public BufferedImage filter(BufferedImage src, BufferedImage dst) {
        //    if (!initialized)
        //        initialize();
        //    return super.filter(src, dst);
        //}

        protected virtual void Initialize() {
            Initialized = true;
            rTable = gTable = bTable = MakeTable();
        }

        protected int[] MakeTable() {
            var table = new int[256];
            for (var i = 0; i < 256; i++) {
                table[i] = PixelUtils.Clamp((int)(255*TransferFunction(i/255.0f)));
            }
            return table;
        }

        protected float TransferFunction(float v) {
            return 0;
        }

        public uint[] GetLUT() {
            if (!Initialized) {
                Initialize();
            }

            var lut = new uint[256];
            for (uint i = 0; i < 256; i++) {
                lut[i] = FilterRgb(0, 0, (i << 24) | (i << 16) | (i << 8) | i);
            }
            return lut;
        }

    }
}