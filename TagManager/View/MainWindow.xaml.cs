using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
	public partial class MainWindow : CustomWindow, IViewFor<MainViewModel>
	{
	    public MainViewModel ViewModel
	    {
	        get
	        {
	            return DataContext as MainViewModel;
	        }
            set
            {
                DataContext = value;
            }
	    }


        object IViewFor.ViewModel { get => DataContext; set => DataContext = value; }

        public MainWindow()
		{
			InitializeComponent();
			Closing += (s, e) =>
			{
			    var folders = ViewModel.OpenedFolders;

                Properties.Settings.Default.OpenedFolders.Clear();
			    Properties.Settings.Default.OpenedFolders = new StringCollection();
			    Properties.Settings.Default.OpenedFolders.AddRange(folders.ToArray());
                Properties.Settings.Default.Save();
			};
		}

    }
}