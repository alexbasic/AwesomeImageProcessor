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
            var tt = new FastBitmap(bitmap);
            tt.Lock();
            var width = bitmap.Width;
            var height = bitmap.Height;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    //var color = bitmap.GetPixel(x, y);
                    //++_statBufferRed[color.R];
                    //++_statBufferGreen[color.G];
                    //++_statBufferBlue[color.B];

                    var (r, g, b) = tt.GetPixel(x, y);
                    ++_statBufferRed[r];
                    ++_statBufferGreen[g];
                    ++_statBufferBlue[b];
                }
            }
            tt.Unlock();
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

    //public class DirectBitmap : IDisposable
    //{
    //    public Bitmap Bitmap { get; private set; }
    //    public Int32[] Bits { get; private set; }
    //    public bool Disposed { get; private set; }
    //    public int Height { get; private set; }
    //    public int Width { get; private set; }

    //    protected GCHandle BitsHandle { get; private set; }

    //    public DirectBitmap(int width, int height)
    //    {
    //        Width = width;
    //        Height = height;
    //        Bits = new Int32[width * height];
    //        BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
    //        Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
    //    }

    //    public void SetPixel(int x, int y, Color colour)
    //    {
    //        int index = x + (y * Width);
    //        int col = colour.ToArgb();

    //        Bits[index] = col;
    //    }

    //    public Color GetPixel(int x, int y)
    //    {
    //        int index = x + (y * Width);
    //        int col = Bits[index];
    //        Color result = Color.FromArgb(col);

    //        return result;
    //    }

    //    public void Dispose()
    //    {
    //        if (Disposed) return;
    //        Disposed = true;
    //        Bitmap.Dispose();
    //        BitsHandle.Free();
    //    }
    //}
}
