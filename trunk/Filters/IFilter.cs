using System;
using System.Drawing;
using System.Text;

namespace Kaliko.ImageLibrary.Filters {
    public interface IFilter {

        void run(KalikoImage image);
    }
}
