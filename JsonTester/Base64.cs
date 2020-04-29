using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
namespace JsonTester
{
    class MyConverter
    {
        static public void RawToBitmap(ref BitmapSource reSource,int iWidth, int iHeight, byte[] rawData)
        {

            double dpi = 96;
            byte[] pixelData = new byte[iWidth * iHeight];

            for (int y = 0; y < iHeight; ++y)
            {
                int yIndex = y * iWidth;
                for (int x = 0; x < iWidth; ++x)
                {
                    pixelData[x + yIndex] = (byte)(x + y);
                }
            }

            reSource = BitmapSource.Create(iWidth, iHeight, dpi, dpi,
                PixelFormats.Gray8, null, rawData, iWidth);
        }
      
    }
}

