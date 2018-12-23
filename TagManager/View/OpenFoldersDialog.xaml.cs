using ReactiveUI;
using System.Windows.Input;
using TagManager.Controls;
using TagManager.ViewModel;

namespace TagManager.View
{
    /// <summary>
    /// Interaction logic for OpenFoldersDialog.xaml
    /// </summary>
    public partial class OpenFoldersDialog : CustomWindow, IViewFor<OpenFoldersDialogViewModel>
    {
        public OpenFoldersDialog()
        {
            InitializeComponent();
        }

        public OpenFoldersDialogViewModel ViewModel { get => DataContext as OpenFoldersDialogViewModel; set => DataContext = value; }
        object IViewFor.ViewModel { get => DataContext; set => DataContext = value; }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ViewModel.SelectInListBox(foldLB.SelectedItem as FolderViewModel);
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
