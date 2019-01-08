using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TagManager;

namespace ViewModel
{
    public class FolderViewModel : ViewModelBase
    {
        private bool _isSelected;
        private bool _isExpanded;
        private bool _isLoadingChildren;

        public string Path
        {
            get;
        }

        public string FolderName
        {
            get
            {
                if (Parent?.IsLoadingChildrens ?? false)
                {
                    return "Loading...";
                }
                var parts = Path.Split('\\');

                return parts.Length >= 2 && !string.IsNullOrEmpty(parts.LastOrDefault()) ? (parts.LastOrDefault().Length > 40 ? $"{parts.LastOrDefault().Substring(0, 30)}..." : parts.LastOrDefault()) : parts.FirstOrDefault();
            }
        }

        public FolderViewModel Parent { get; }

        public ObservableCollection<FolderViewModel> SubFolders
        {
            get;
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

        public FolderViewModel(string path, FolderViewModel parent = null, bool subFolders = true)
        {
            Path = path;
            Parent = parent;
            if (subFolders && !string.IsNullOrEmpty(path) && Directory.GetDirectories(Path).Any())
            {
                SubFolders.Add(new FolderViewModel(string.Empty, this));
            }
        }

        private Task GetSubFolders(bool value)
        {
            if (value && SubFolders.Any(o => string.IsNullOrEmpty(o.FolderName)))
            {
                IsLoadingChildrens = true;
                SubFolders.Clear();
                SubFolders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), this, false));
                var subfolders = Directory.GetDirectories(Path);
                var toSet = new List<FolderViewModel>();

                return Task.Run(() =>
                {
                    foreach (var item in subfolders)
                    {
                        try
                        {
                            toSet.Add(new FolderViewModel(item, this));
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    IsLoadingChildrens = false;

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        SubFolders.Clear();
                        foreach (var folder in toSet)
                        {
                            SubFolders.Add(folder);
                        }

                    });
                });

            }

            return null;
        }

        public bool IsLoadingChildrens
        {
            get { return _isLoadingChildren; }
            set
            {
                _isLoadingChildren = value;
                RaisePropertyChanged(nameof(IsLoadingChildrens));
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