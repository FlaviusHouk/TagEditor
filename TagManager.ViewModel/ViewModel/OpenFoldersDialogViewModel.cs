using ReactiveUI;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;

namespace TagManager.ViewModel
{

    public class OpenFoldersDialogViewModel : ReactiveObject
    {
        private int _sumOfLength;
        private int _count;
        private string _curFolderPath;
        private FolderViewModel _selectedTreeViewFolder;

        public MainViewModel Owner { get; }

        public ObservableCollection<FolderViewModel> Folders { get; set; } = new ObservableCollection<FolderViewModel>();

        public FolderViewModel SelectedTreeFolder
        {
            get { return _selectedTreeViewFolder; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTreeViewFolder, value);

                CurrentFolderPath = _selectedTreeViewFolder.Path;
            }
        }

        public ObservableCollection<FolderViewModel> SelectedListFolders
        {
            get; set;
        } = new ObservableCollection<FolderViewModel>();

        private ReactiveCommand<object, Unit> _selectionTreeChangedCommand;

        public ReactiveCommand<object, Unit> SelectionTreeChangedCommand
        {
            get
            {
                return _selectionTreeChangedCommand ?? (_selectionTreeChangedCommand = ReactiveCommand.Create<object, Unit>((item) =>
                {
                    SelectedTreeFolder = item as FolderViewModel;

                    return Unit.Default;
                }));
            }
        }

        private readonly ObservableAsPropertyHelper<bool> _canOK;
        public bool CanOK
        {
            get { return _canOK.Value; }
        }

        private readonly ObservableAsPropertyHelper<bool> _hasItem;
        public bool HasItem
        {
            get { return _hasItem.Value; }
        }

        public void SelectInListBox(FolderViewModel folderViewModel)
        {
            folderViewModel.IsSelected = true;
            folderViewModel.Parent.IsExpanded = true;
        }

        private ReactiveCommand<object, Unit> _selectionListChangedCommand;

        public ReactiveCommand<object, Unit> SelectionListChangedCommand
        {
            get
            {
                return _selectionListChangedCommand ?? (_selectionListChangedCommand = ReactiveCommand.Create<object, Unit>((item) =>
                {
                    SelectedListFolders.Clear();
                    var items = (item as IEnumerable<object>).Cast<FolderViewModel>();
                    foreach (var fold in items)
                    {
                        SelectedListFolders.Add(fold);
                    }

                    return Unit.Default;
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
                this.RaiseAndSetIfChanged(ref _curFolderPath, value);
            }
        }

        public bool HasFolderTracks
        {
            get { return CanOK && Directory.GetFiles(SelectedTreeFolder.Path).Where(o => o.Split('.').LastOrDefault() == "mp3").Any(); }
        }

        private readonly ObservableAsPropertyHelper<string> _listPlaceText;
        public string ListPlaceText
        {
            get
            {
                return _listPlaceText.Value;
            }
        }

        public OpenFoldersDialogViewModel(MainViewModel owner)
        {
            Owner = owner;

            _hasItem = this.WhenAnyValue(x => x.SelectedTreeFolder)
                            .Select(val => val != null && Directory.GetDirectories(val.Path).Any())
                            .ToProperty(this, x => x.HasItem, false, false, System.Reactive.Concurrency.ImmediateScheduler.Instance);

            _canOK = this.WhenAnyValue(x => x.SelectedTreeFolder, x => x.SelectedListFolders.Count)
                         .Select(val => val.Item2 > 0 || val.Item1 != null)
                         .ToProperty(this, x => x.CanOK, false, false, System.Reactive.Concurrency.ImmediateScheduler.Instance);

            _listPlaceText = this.WhenAnyValue(x => x.SelectedTreeFolder)
                                 .Select(val =>
                                 {
                                     if (val != null && HasFolderTracks)
                                     {
                                         return $"В папке {Directory.GetFiles(val.Path).Where(o => Path.GetExtension(o) == ".mp3").Count()} трек(-а)";
                                     }

                                     else if (val != null && !HasItem)
                                     {
                                         return "Папка пуста";
                                     }

                                     return "Выберите папку";
                                 })
                                 .ToProperty(this, x => x.ListPlaceText, string.Empty, false, System.Reactive.Concurrency.ImmediateScheduler.Instance);
            

            Folders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)));
            Folders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.Favorites)));
            Folders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
            Folders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            Folders.Add(new FolderViewModel(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));

            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (var item in drives.Where(drive=>drive.IsReady))
            {
                Folders.Add(new FolderViewModel(item.Name));
            }
        }

        public string[] GetSelectedFolders()
        {
            if (SelectedListFolders.Any())
            {
                return SelectedListFolders.Select(o => o.Path).ToArray();
            }
            else if (SelectedTreeFolder !=null)
            {
                return new []{ SelectedTreeFolder.Path};
            }

            return null;
        }
    }
}