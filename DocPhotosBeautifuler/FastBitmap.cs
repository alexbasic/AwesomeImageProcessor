using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DocPhotosBeautifuler
{
    public class FastBitmap
    {
        private Bitmap _bitmap;
        private byte[] _rgbValues;

        private PixelFormat _pixelFormat;
        private BitmapData _bmpData;
        private IntPtr _ptr;
        private int _bytes;
        private int _bitmapWidth;

        public FastBitmap(Bitmap bitmap)
        {
            _bitmap = bitmap;

            _pixelFormat = bitmap.PixelFormat;

            _bitmapWidth = bitmap.Width;

            if (_pixelFormat != PixelFormat.Format24bppRgb &&
                _pixelFormat != PixelFormat.Format32bppArgb /*||
                _pixelFormat != PixelFormat.Format32bppRgb ||
                _pixelFormat != PixelFormat.Format16bppGrayScale*/) throw new ArgumentException("FastBitmap don't support this pixel format");
        }

        public void Lock()
        {
            Rectangle rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            _bmpData =
                _bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                _bitmap.PixelFormat);

            _ptr = _bmpData.Scan0;

            _bytes = Math.Abs(_bmpData.Stride) * _bitmap.Height;
            _rgbValues = new byte[_bytes];

            System.Runtime.InteropServices.Marshal.Copy(_ptr, _rgbValues, 0, _bytes);
        }

        public void Unlock()
        {
            System.Runtime.InteropServices.Marshal.Copy(_rgbValues, 0, _ptr, _bytes);
            _bitmap.UnlockBits(_bmpData);
        }

        public (byte r, byte g, byte b) GetPixel(int x, int y)
        {
            if (_pixelFormat == PixelFormat.Format24bppRgb) 
            {
                var bytesPerPixel = 3;
                var pixelPoint = (x + (y * _bitmapWidth)) * bytesPerPixel;
                return (_rgbValues[pixelPoint + 2],
                    _rgbValues[pixelPoint + 1],
                    _rgbValues[pixelPoint]);
            }
            if (_pixelFormat == PixelFormat.Format32bppArgb)
            {
                var bytesPerPixel = 4;
                var pixelPoint = (x + (y * _bitmapWidth)) * bytesPerPixel;
                return (_rgbValues[pixelPoint + 2],
                    _rgbValues[pixelPoint + 1],
                    _rgbValues[pixelPoint]);
            }
            throw new NotImplementedException();
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b)
        {
            if (_pixelFormat == PixelFormat.Format24bppRgb)
            {
                var bytesPerPixel = 3;

                var pixelPoint = (x + (y * _bitmapWidth)) * bytesPerPixel;

                _rgbValues[pixelPoint + 2] = r;
                _rgbValues[pixelPoint + 1] = g;
                _rgbValues[pixelPoint] = b;
            }

            if (_pixelFormat == PixelFormat.Format32bppArgb)
            {
                var bytesPerPixel = 4;

                var pixelPoint = (x + (y * _bitmapWidth)) * bytesPerPixel;

                _rgbValues[pixelPoint + 2] = r;
                _rgbValues[pixelPoint + 1] = g;
                _rgbValues[pixelPoint] = b;
            }
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
