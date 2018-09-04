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

namespace TagManager.Converters
{
    class ColorCluster
    {
        public Color MainColor { get; private set; }
        public int Close { get; set; } = 0;

        public ColorCluster(Color main)
        {
            MainColor = main;
        }
        
        public double CheckDistance(Color toCheck)
        {
            double redDiff = Math.Pow(MainColor.R - toCheck.R, 2);
            double greenDiff = Math.Pow(MainColor.G - toCheck.G, 2);
            double blueDiff = Math.Pow(MainColor.B - toCheck.B, 2);

            return Math.Sqrt(redDiff + greenDiff + blueDiff);
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
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
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

            if (image.Format.BitsPerPixel == 24)
            {
                for (int i = 0; i < data.Length; i += 3)
                {
                    pixels.Add(Color.FromRgb(data[i + 2], data[i + 1], data[i]));
                }

                Debug.WriteLine(DateTime.Now - start);
                start = DateTime.Now;
            }

            if (pixels.Count != 0)
            {
                _pixelData = pixels;
                _asyncEmul = new ManualResetEvent(false);
                ClusterAnalysisAsync();
                _asyncEmul.WaitOne();

                Debug.WriteLine(DateTime.Now - start);
                start = DateTime.Now;

                return new SolidColorBrush(GetMainColor());
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

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
                clusters.ElementAt(distances.IndexOf(distances.Min())).Close++;
            }

            var counts = clusters.Select(cluster => cluster.Close).ToList();

            return clusters.ElementAt(counts.IndexOf(counts.Max())).MainColor;
        }

        ManualResetEvent _asyncEmul;

        private IEnumerable<Color> _pixelData;
        private List<IEnumerable<ColorCluster>> _clusterSets = new List<IEnumerable<ColorCluster>>();
        private int _sharedCouner = 0;
        private object _indexLocker = new object();
        private object _clusterLocker = new object();

        private void ClusterAnalysisAsync()
        {
            Random rand = new Random(/*(int)DateTime.Now.Ticks*/);

            for (int j = 0; j < 1; j++)
            {
                List<ColorCluster> clusters = new List<ColorCluster>(10);
                for (int i = 0; i < 10; i++)
                {
                    clusters.Add(new ColorCluster(_pixelData.ElementAt(rand.Next(_pixelData.Count()))));
                }
                _clusterSets.Add(clusters);
            }

            Task firstTask = Task.Run(new Action(FindColor));
            Task secondTask = Task.Run(new Action(FindColor));
            Task thirdTask = Task.Run(new Action(FindColor));
            Task fourthTask = Task.Run(new Action(FindColor));
        } 

        private void FindColor()
        {
            int index;
            lock (_indexLocker)
            {
                index = _sharedCouner;
                _sharedCouner++;
            }

            while (index < _pixelData.Count())
            {
                Color pixel = _pixelData.ElementAt(index);
                foreach (IEnumerable<ColorCluster> clusters in _clusterSets)
                {
                    var distances = clusters.Select(cluster => cluster.CheckDistance(pixel)).ToList();
                    ColorCluster c = clusters.ElementAt(distances.IndexOf(distances.Min()));

                    lock (_clusterLocker)
                    {
                        c.Close++;
                    }
                }

                lock (_indexLocker)
                {
                    index = _sharedCouner;
                    _sharedCouner++;
                }
            }

            _asyncEmul.Set();
        }

        private Color GetMainColor()
        {
            List<Color> rezults = new List<Color>();
            foreach (IEnumerable<ColorCluster> clusters in _clusterSets)
            {
                var counts = clusters.Select(cluster => cluster.Close).ToList();

                rezults.Add(clusters.ElementAt(counts.IndexOf(counts.Max())).MainColor);
            }

            byte r = (byte)rezults.Select(c => (double)c.R).Average();
            byte g = (byte)rezults.Select(c => (double)c.G).Average();
            byte b = (byte)rezults.Select(c => (double)c.B).Average();

            _sharedCouner = 0;
            _clusterSets.Clear();

            return Color.FromRgb(r, g, b);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
