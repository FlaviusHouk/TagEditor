using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagManager.ViewModel.Services
{
    public interface IDialogService
    {
        void ShowOpenFolderDialog(OpenFoldersDialogViewModel vm);
    }
}
