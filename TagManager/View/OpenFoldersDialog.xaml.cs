using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;
using TagManager.Controls;
using TagManager.ViewModel;
using ViewModel;

namespace TagManager.View
{
    /// <summary>
    /// Interaction logic for OpenFoldersDialog.xaml
    /// </summary>
    public partial class OpenFoldersDialog : CustomWindow
    {
        private RelayCommand _OKCommand;

        public RelayCommand OKCommand
        {
            get
            {
                return _OKCommand ?? (_OKCommand = new RelayCommand(() =>
                {
                    DialogResult = true;
                    Close();
                }));
            }

        }

        public OpenFoldersDialog()
        {
            InitializeComponent();
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                (this.DataContext as OpenFoldersDialogViewModel).SelectInListBox(foldLB.SelectedItem as FolderViewModel);
            }
        }
    }
}
