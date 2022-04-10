using System.Drawing;

namespace DocPhotosBeautifuler
{
    public class ChangeContrast
    {
        public int MinValue { get; }
        public int MaxValue { get; }

        public void Change(Bitmap bitmap, Histogram histogram)
        {
            //var (min, max) = GetMinMax(histogram);

            var (min, max) = GetMinMax2(histogram, bitmap.Width * bitmap.Height, 0.05f);

            var h = bitmap.Height;
            var w = bitmap.Width;
            for(var y = 0; y < h; y++) 
            {
                for(var x = 0; x < w; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    var newColor = Color.FromArgb(Stretch(color.R, min, max), Stretch(color.G, min, max), Stretch(color.B, min, max));
                    bitmap.SetPixel(x, y, newColor);
                }
            }
        }

        private int Stretch(int value, int min, int max)
        {
            var mi = (float)min / 255f;
            var ma = (float)max / 255f;
            var b = 1f / (-mi+ma);
            var a = -b * mi;
            
            var y = (int)((a + b * ((float)value/255f))*255f);

            if (y < 0) return 0;
            if (y > 255) return 255;

            return y;
        }

        /// <summary>
        /// Отсечь с конца и с начала по превышению порога
        /// </summary>
        private (int min, int max) GetMinMax(Histogram histogram, float threshold = 0.005f)
        {
            var l = histogram.CommonStatBuffer.Length;

            var max = l - 1;
            for (var i = l - 1; i >= 10; i--)
            {
                var t = histogram.CommonStatBuffer[i];
                if (((float)t / (float)histogram.MaxValue) > threshold)
                {
                    max = i;
                    break;
                }
            }
            var min = 0;
            for (var i = 0; i < l - 10; i++)
            {
                var t = histogram.CommonStatBuffer[i];
                if (((float)t / (float)histogram.MaxValue) > threshold)
                {
                    min = i;
                    break;
                }
            }

            return (min, max);
        }

        /// <summary>
        /// Отсечь процент пикселей с начала и с конца
        /// </summary>
        private (int min, int max) GetMinMax2(Histogram histogram, float bitmapSquare, float cutoff = 0.05f)
        {
            var countForCut = bitmapSquare * cutoff;

            var l = histogram.CommonStatBuffer.Length;

            var max = l - 1;
            int counter = 0;
            for (var i = l - 1; i >= 10; i--)
            {
                counter += histogram.CommonStatBuffer[i];
                if (counter >= countForCut)
                {
                    max = i;
                    break;
                }
            }
            var min = 0;
            counter = 0;
            for (var i = 0; i < max-1; i++)
            {
                counter += histogram.CommonStatBuffer[i];
                if (counter >= countForCut)
                {
                    min = i;
                    break;
                }
            }

            return (min, max);
        }
    }
}
