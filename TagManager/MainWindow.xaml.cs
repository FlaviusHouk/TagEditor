using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TagManager.Controls;
using TagManager.ViewModel;

namespace TagManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : CustomWindow
	{
        public SolidColorBrush MajorColor
        {
            get
            {
                var bitmap = image.Source as BitmapImage;

                int stride = (int)bitmap.PixelWidth * (bitmap.Format.BitsPerPixel / 8);
                byte[] pixels = new byte[(int)bitmap.PixelHeight * stride];

                bitmap.CopyPixels(pixels, stride, 0);

                List<int> R = new List<int>();
                List<int> G = new List<int>();
                List<int> B = new List<int>();

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    R.Add(pixels[i]);
                    G.Add(pixels[i + 1]);
                    B.Add(pixels[i + 2]);
                }

                byte RC = Convert.ToByte(R.Average());
                byte GC = Convert.ToByte(G.Average());
                byte BC = Convert.ToByte(B.Average());

                return new SolidColorBrush(Color.FromRgb(RC, GC, BC));
            }
        }

		public MainWindow()
		{
			InitializeComponent();
			Closing += (s, e) => ViewModelLocator.Cleanup();
			
		}
    }
}