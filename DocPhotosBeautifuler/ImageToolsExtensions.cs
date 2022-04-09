using System.Drawing;

namespace DocPhotosBeautifuler
{
    public static class ImageToolsExtensions
    {
        const float OneInchInMm = 25.4f;

        /// <summary>
        /// Подогнать размер к меньшему с сохранением пропорций и ориентированием посередине
        /// </summary>
        public static Rectangle AdjustTheSize(this Size srcSize, Size dstSize)
        {
            var dstSizeX = (double)dstSize.Width;
            var dstSizeY = (double)dstSize.Height;
            var srcSizeX = (double)srcSize.Width;
            var srcSizeY = (double)srcSize.Height;

            var destAspectRatio = dstSizeX / dstSizeY;
            var srcAspectRatio = srcSizeX / srcSizeY;

            var w = 0;
            var h = 0;
            var x = 0;
            var y = 0;

            if (destAspectRatio > srcAspectRatio)
            {
                w = (int)((dstSizeY / srcSizeY) * srcSizeX);
                h = dstSize.Height;
                x = (int)((dstSizeX / 2d) - (w / 2d));
                y = 0;
            }
            else
            {
                w = dstSize.Width;
                h = (int)((dstSizeX / srcSizeX) * srcSizeY);
                x = 0;
                y = (int)((dstSizeY / 2d) - (h / 2d));
            }

            return new Rectangle(x, y, w, h);
        }

        public static SizeF AdjustTheSize(this SizeF srcSize, SizeF dstSize)
        {
            var destAspectRatio = dstSize.Width / dstSize.Height;
            var srcAspectRatio = srcSize.Width / srcSize.Height;

            var w = 0f;
            var h = 0f;

            if (destAspectRatio > srcAspectRatio)
            {
                w = (dstSize.Height / srcSize.Height) * srcSize.Width;
                h = dstSize.Height;
            }
            else
            {
                w = dstSize.Width;
                h = (dstSize.Width / srcSize.Width) * srcSize.Height;
            }

            return new SizeF(w, h);
        }

        public static int MillimetersToPoints(float value, float dpi) => (int)((value * dpi) / OneInchInMm);

        public static float PointsToMillimeters(float value, float dpi) => (value * OneInchInMm) / dpi;

        public static float AspectRatio(this Size size) => size.Width / size.Height;

        public static float AspectRatio(float width, float height) => width / height;
    }
}
