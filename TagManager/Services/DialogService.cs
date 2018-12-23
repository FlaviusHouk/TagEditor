using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagManager.View;
using TagManager.ViewModel;
using TagManager.ViewModel.Services;

namespace TagManager.Services
{
    class DialogService : IDialogService
    {
        public void ShowOpenFolderDialog(OpenFoldersDialogViewModel vm)
        {
            OpenFoldersDialog dialog = new OpenFoldersDialog() { DataContext = vm, Owner = App.Current.MainWindow };

            dialog.ShowDialog();
        }
    }
}
