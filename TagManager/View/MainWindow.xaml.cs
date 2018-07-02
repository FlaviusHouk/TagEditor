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

namespace TagManager.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : CustomWindow
	{
	    private RelayCommand _unselelectAllCommand;

        public RelayCommand UnselectAllCommand
        {
            get { return _unselelectAllCommand ?? (_unselelectAllCommand = new RelayCommand(() =>
            {
                ListView.UnselectAll();
            })); }
        }


		public MainWindow()
		{
			InitializeComponent();
			Closing += (s, e) => ViewModelLocator.Cleanup();
		}

    }
}