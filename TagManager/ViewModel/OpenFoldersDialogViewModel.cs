﻿using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ViewModel
{

    public class OpenFoldersDialogViewModel : ViewModelBase
    {
        private int _sumOfLength;
        private int _count;
        private string _curFolderPath;
        private FolderViewModel _selectedTreeViewFolder;

        public ObservableCollection<FolderViewModel> Folders { get; set; } = new ObservableCollection<FolderViewModel>();

        public FolderViewModel SelectedTreeFolder
        {
            get { return _selectedTreeViewFolder; }
            set
            {
                _selectedTreeViewFolder = value;
                CurrentFolderPath = _selectedTreeViewFolder.Path;
                RaisePropertyChanged(nameof(SelectedTreeFolder));
            }
        }

        public ObservableCollection<FolderViewModel> SelectedListFolders
        {
            get; set;
        } = new ObservableCollection<FolderViewModel>();

        private RelayCommand<object> _selectionTreeChangedCommand;

        public RelayCommand<object> SelectionTreeChangedCommand
        {
            get
            {
                return _selectionTreeChangedCommand ?? (_selectionTreeChangedCommand = new RelayCommand<object>((item) =>
                {
                    SelectedTreeFolder = item as FolderViewModel;
                }));
            }
        }

        public void SelectInListBox(FolderViewModel folderViewModel)
        {
            folderViewModel.IsSelected = true;
            folderViewModel.Parent.IsExpanded = true;
        }

        private RelayCommand<object> _selectionListChangedCommand;

        public RelayCommand<object> SelectionListChangedCommand
        {
            get
            {
                return _selectionListChangedCommand ?? (_selectionListChangedCommand = new RelayCommand<object>((item) =>
                {
                    SelectedListFolders.Clear();
                    var items = (item as IEnumerable<object>).Cast<FolderViewModel>();
                    foreach (var fold in items)
                    {
                        SelectedListFolders.Add(fold);
                    }
                }));
            }
        }

        public string CurrentFolderPath
        {
            get
            {
                return SelectedTreeFolder == null ? "Выберите папку" : _curFolderPath;
            }

            set
            {
                _curFolderPath = value;
                RaisePropertyChanged(nameof(CurrentFolderPath));
            }
        }

        public OpenFoldersDialogViewModel()
        {
            Folders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)));
            Folders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.Favorites)));
            Folders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
            Folders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));

            var drives = Environment.GetLogicalDrives();
            Folders.Add(new FolderViewModel(drives.FirstOrDefault()));
            Folders.Add(new FolderViewModel(Environment.GetLogicalDrives().LastOrDefault()));
        }

        private void GetAvgLength(IEnumerable<FolderViewModel> fw)
        {
            _count += fw.Count();
            foreach (var item in fw)
            {
                _sumOfLength += item.FolderName.Length;
                GetAvgLength(item.SubFolders);
            }
        }

        public string[] GetSelectedFolders()
        {
            return SelectedListFolders.Select(o => o.Path).ToArray();
        }
    }
}