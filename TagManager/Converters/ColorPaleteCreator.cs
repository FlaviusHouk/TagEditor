using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TagManager.Helpers;

namespace TagManager.Converters
{
    class ColorCluster : ICluster<Color>
    {
        public Color Main { get; private set; }
        public int Count { get; set; } = 0;

        public ColorCluster(Color main)
        {
            Main = main;
        }
        
        public double CheckDistance(Color toCheck)
        {
            double redDiff = Math.Pow(Main.R - toCheck.R, 2);
            double greenDiff = Math.Pow(Main.G - toCheck.G, 2);
            double blueDiff = Math.Pow(Main.B - toCheck.B, 2);

            return Math.Sqrt(redDiff + greenDiff + blueDiff);
        }

        public override int GetHashCode()
        {
            return Main.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ColorCluster that = obj as ColorCluster;

            if (that == null)
                return false;

            return that.Main.Equals(Main) && Count == that.Count;
        }
    }

    class ColorPaleteCreator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime start = DateTime.Now;
            byte[] data = value as byte[];

            if (data == null || data.Length == 0)
                return null;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(data))
            {
                try
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.DecodePixelHeight = 128;
                    image.DecodePixelWidth = 128;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                catch (Exception e)
                {
                    return Brushes.White;
                }
            }
            image.Freeze();
            Debug.WriteLine(DateTime.Now - start);
            start = DateTime.Now;

            int minStride = checked(((image.PixelWidth * image.Format.BitsPerPixel) + 7) / 8);

            int minRequiredDestSize = checked((minStride * (image.PixelHeight - 1)) + minStride);
            data = new byte[minRequiredDestSize];

            image.CopyPixels(data, minStride, 0);

            Debug.WriteLine(DateTime.Now - start);
            start = DateTime.Now;

            List<Color> pixels = new List<Color>(data.Length / 3);

            if (image.Format == PixelFormats.Bgr24)
            {
                for (int i = 0; i < data.Length; i += 3)
                {
                    pixels.Add(Color.FromRgb(data[i + 2], data[i + 1], data[i]));
                }
            }
            else if (image.Format == PixelFormats.Bgr32)
            {
                for (int i = 0; i < data.Length; i += 4)
                {
                    pixels.Add(Color.FromRgb(data[i + 2], data[i + 1], data[i]));
                }
            }
            else if (image.Format == PixelFormats.Rgb24)
            {
                for (int i = 0; i < data.Length; i += 3)
                {
                    pixels.Add(Color.FromRgb(data[i], data[i + 1], data[i + 2]));
                }
            }
            

            if (pixels.Count != 0)
            {
                Random rand = new Random((int)DateTime.Now.Ticks);
                _clusterSet = new List<ColorCluster>(10);
                for (int i = 0; i < 10; i++)
                {
                    _clusterSet.Add(new ColorCluster(pixels.ElementAt(rand.Next(pixels.Count()))));
                }
                ClusterAnalisis<Color, ColorCluster> helper = new ClusterAnalisis<Color, ColorCluster>(pixels, _clusterSet);
                helper.RunAnalysisAsync(4);
                IEnumerable<ColorCluster> main = _clusterSet.OrderBy(c => c.Count).Skip(8).ToArray();
                helper = new ClusterAnalisis<Color, ColorCluster>(pixels, main);
                helper.RunAnalysisAsync(4);
                _clusterSet = main.ToList();

                Debug.WriteLine(DateTime.Now - start);
                start = DateTime.Now;

                return new SolidColorBrush(GetMainColor());
            }
            else
            {
                return Brushes.White;
            }
        }

        private List<ColorCluster> _clusterSet;

        private Color ClusterAnalysis(IEnumerable<Color> pixels)
        {
            Random rand = new Random((int)DateTime.Now.Ticks);

            List<ColorCluster> clusters = new List<ColorCluster>(10);
            for (int i = 0; i < 10; i++)
            {
                clusters.Add(new ColorCluster(pixels.ElementAt(rand.Next(pixels.Count()))));
            }

            foreach (Color pixel in pixels)
            {
                var distances = clusters.Select(cluster => cluster.CheckDistance(pixel)).ToList();
                clusters.ElementAt(distances.IndexOf(distances.Min())).Count++;
            }

            var counts = clusters.Select(cluster => cluster.Count).ToList();

            return clusters.ElementAt(counts.IndexOf(counts.Max())).Main;
        }

        private Color GetMainColor()
        {
            List<Color> rezults = new List<Color>();

            var counts = _clusterSet.Select(cluster => cluster.Count).ToList();

            rezults.Add(_clusterSet.ElementAt(counts.IndexOf(counts.Max())).Main);

            byte r = (byte)rezults.Select(c => (double)c.R).Average();
            byte g = (byte)rezults.Select(c => (double)c.G).Average();
            byte b = (byte)rezults.Select(c => (double)c.B).Average();

            _clusterSet.Clear();

            return Color.FromRgb(r, g, b);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
