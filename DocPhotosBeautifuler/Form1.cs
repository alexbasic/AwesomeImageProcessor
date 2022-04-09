using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocPhotosBeautifuler
{
    public partial class Form1 : Form
    {
        Bitmap _bitmap;
        Bitmap _adjustedBitmap;
        int _jpegQuality;

        public Form1()
        {
            InitializeComponent();

            _jpegQuality = trackBar1.Value;
            textBox4.Text = _jpegQuality.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) != DialogResult.OK) return;

            textBox1.Text = openFileDialog1.FileName;

            _bitmap = new Bitmap(openFileDialog1.FileNames[0]);
            textBox2.Text = GetBitmapInfo(_bitmap);
            pictureBox1.Refresh();

            var histogram = new Histogram();
            histogram.Analyze(_bitmap);
            var histogramBitmap = new Bitmap(pictureBox3.ClientSize.Width, pictureBox3.ClientSize.Height);
            histogram.DrawTo(histogramBitmap);
            pictureBox3.Image = histogramBitmap;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_bitmap == null) return;

            var rectangle = _bitmap.Size.AdjustTheSize(((PictureBox)sender).ClientSize);

            e.Graphics.DrawImage(_bitmap, rectangle);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) != DialogResult.OK) return;

            var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            var encoderKind = System.Drawing.Imaging.Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(encoderKind, 70L);

            _adjustedBitmap.Save(saveFileDialog1.FileName, jpgEncoder, encoderParameters);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var a4SizeInMm = new SizeF(210, 297);
            var upDownBorderInMm = 20f;
            var leftRightBorderInMm = 15f;
            var maxSizeInMm = new SizeF(
                a4SizeInMm.Width - (leftRightBorderInMm * 2f),
                a4SizeInMm.Height - (upDownBorderInMm * 2f));
            var destinationDpi = float.Parse(comboBox1.Text); // 75, 150, 300
            var maxSizeInPix = new Size(
                ImageToolsExtensions.MillimetersToPoints(maxSizeInMm.Width, destinationDpi),
                ImageToolsExtensions.MillimetersToPoints(maxSizeInMm.Height, destinationDpi));

            var dstSizePix =
                    _bitmap.Size.AdjustTheSize(
                        (_bitmap.Width / _bitmap.Height) < 1 ?
                        maxSizeInPix :
                        new Size(maxSizeInPix.Height, maxSizeInPix.Width));
            if (!(_bitmap.Size.Width > maxSizeInPix.Width || _bitmap.Size.Height > maxSizeInPix.Height))
            {
                MessageBox.Show($"Destination size of the pictire is more big ({dstSizePix.Width} x {dstSizePix.Height})");
                return;
            }

            _adjustedBitmap = new Bitmap(dstSizePix.Width, dstSizePix.Height, PixelFormat.Format24bppRgb);
            _adjustedBitmap.SetResolution(destinationDpi, destinationDpi);

            //var bitmapData = _adjustedBitmap.LockBits(new Rectangle(0,0, _adjustedBitmap.Width, _adjustedBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //_adjustedBitmap.UnlockBits(bitmapData);
            using (var g = Graphics.FromImage(_adjustedBitmap))
            {
                g.DrawImage(_bitmap, 0, 0, dstSizePix.Width, dstSizePix.Height);
            }

            textBox3.Text = GetBitmapInfo(_adjustedBitmap);
            pictureBox2.Refresh();

            var histogram = new Histogram();
            histogram.Analyze(_adjustedBitmap);
            var histogramBitmap = new Bitmap(pictureBox4.ClientSize.Width, pictureBox4.ClientSize.Height);
            histogram.DrawTo(histogramBitmap);
            pictureBox4.Image = histogramBitmap;
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (_adjustedBitmap == null) return;

            var rectangle = _adjustedBitmap.Size.AdjustTheSize(((PictureBox)sender).ClientSize);

            e.Graphics.DrawImage(_adjustedBitmap, rectangle);
        }

        private string GetBitmapInfo(Bitmap value) =>
            $"Resolution: {value.Width}x{value.Height}\r\nDPI: {value.HorizontalResolution} x {value.VerticalResolution}\r\nSize: {ImageToolsExtensions.PointsToMillimeters(value.PhysicalDimension.Width, value.HorizontalResolution).ToString("0.00")}mm x {ImageToolsExtensions.PointsToMillimeters(value.PhysicalDimension.Height, value.VerticalResolution).ToString("0.00")}mm";

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            _jpegQuality = trackBar1.Value;
            textBox4.Text = _jpegQuality.ToString();
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            _jpegQuality = int.Parse(textBox4.Text);
            trackBar1.Value = _jpegQuality;
        }

        private void textBox4_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            _jpegQuality = int.Parse(textBox4.Text);
            trackBar1.Value = _jpegQuality;
        }
    }
}
