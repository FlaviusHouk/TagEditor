using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ViewModel
{
    public class FolderViewModel : ViewModelBase
    {
        private bool _isSelected;
        private bool _isExpanded;

        public string Path
        {
            get;
            private set;
        }

        public string FolderName
        {
            get
            {
                var parts = Path.Split('\\');
                return parts.Length >= 2 && !string.IsNullOrEmpty(parts.LastOrDefault()) ? (parts.LastOrDefault().Length > 40 ? $"{parts.LastOrDefault().Substring(0, 30)}..." : parts.LastOrDefault()) : parts.FirstOrDefault();
            }
        }

        public FolderViewModel Parent { get; private set; }

        public ObservableCollection<FolderViewModel> SubFolders
        {
            get; private set;
        } = new ObservableCollection<FolderViewModel>();

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                GetSubFolders(value);
                RaisePropertyChanged(nameof(IsSelected));
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                _isExpanded = value;
                if (_isExpanded && Parent != null)
                {
                    Parent.IsExpanded = true;
                }
                GetSubFolders(value);
                RaisePropertyChanged(nameof(IsExpanded));
            }
        }

        public FolderViewModel(string path, FolderViewModel parent = null)
        {
            Path = path;
            Parent = parent;
            if (!string.IsNullOrEmpty(path) && Directory.GetDirectories(Path).Any())
            {
                SubFolders.Add(new FolderViewModel(string.Empty, this));
            }
        }

        private void GetSubFolders(bool value)
        {
            if (value && SubFolders.Any(o => string.IsNullOrEmpty(o.FolderName)))
            {
                SubFolders.Clear();
                var subfolders = Directory.GetDirectories(Path);
                foreach (var item in subfolders)
                {
                    try
                    {
                        SubFolders.Add(new FolderViewModel(item, this));
                    }
                    catch (Exception e) { }
                }
            }
        }

        public void ExpandAndSelectParent()
        {
            if (Parent != null)
            {
                Parent.IsExpanded = true;
                Parent.IsSelected = true;
            }
        }
    }
}