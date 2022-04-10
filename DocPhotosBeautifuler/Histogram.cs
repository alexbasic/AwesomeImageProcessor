using System;
using System.Drawing;

namespace DocPhotosBeautifuler
{
    public class Histogram
    {
        private int[] _statBufferRed = new int[256];
        private int[] _statBufferGreen = new int[256];
        private int[] _statBufferBlue = new int[256];
        private int[] _commonStatBuffer = new int[256];
        private int _maxValue;

        public int[] StatBufferRed { get => _statBufferRed; }
        public int[] StatBufferGreen { get => _statBufferGreen; }
        public int[] StatBufferBlue { get => _statBufferBlue; }

        public int[] CommonStatBuffer { get => _commonStatBuffer; }
        
        public int MaxValue { get => _maxValue; }

        public void Analyze(Bitmap bitmap)
        {
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    ++_statBufferRed[color.R];
                    ++_statBufferGreen[color.G];
                    ++_statBufferBlue[color.B];
                }
            }

            for (var i = 0; i < CommonStatBuffer.Length; i++)
            {
                CommonStatBuffer[i] = Math.Max(Math.Max(_statBufferRed[i], _statBufferGreen[i]), _statBufferBlue[i]);
                _maxValue = (_maxValue < CommonStatBuffer[i]) ? CommonStatBuffer[i] : _maxValue;      
            }
        }

        public void DrawTo(Bitmap bitmap)
        {
            var tmp = new Bitmap(256, 256);
            using (var graphics = Graphics.FromImage(tmp))
            {
                graphics.Clear(Color.White);

                for (var i = 0; i < CommonStatBuffer.Length; i++)
                {
                    var value = (int)(((float)CommonStatBuffer[i] / (float)_maxValue) * 255f);
                    graphics.DrawLine(Pens.Black, i, 255, i, 255 - value);
                }
            }
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(tmp, 0, 0, bitmap.Width, bitmap.Height);
            }
            tmp.Dispose();
        }
    }
}
