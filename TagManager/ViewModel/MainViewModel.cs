using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TagManager.View;
using TagManager.WebRequest;
using ViewModel;

namespace TagManager.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private bool _isPlayerMode = true;
        private const int ISRCCount = 12;
        private bool _isISRCSearching;
        private bool _isSaving;
        private int _counterForId = 1;
        #endregion

        #region Properties

        public List<string> OpenedFolders { get; } = new List<string>();

        public ObservableCollection<TrackViewModel> Tracks { get; } = new ObservableCollection<TrackViewModel>();

        public List<TrackViewModel> SelectedItems { get; set; } = new List<TrackViewModel>();

        public string Progress
        {
            get
            {
                return $"{Value} / {Max}";
            }
        }

        public bool IsInsp
        {
            get
            {
                return _isInspecting;
            }
            set
            {
                _isInspecting = value;
                RaisePropertyChanged(nameof(IsInsp));
                
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged(nameof(Value));
                    RaisePropertyChanged(nameof(Progress));
                }
            }
        }

        public int Max
        {
            get
            {
                return _max;
            }
            set
            {
                if (_max != value)
                {
                    _max = value;
                    RaisePropertyChanged(nameof(Max));
                }
            }
        }

        public bool IsISRCSearching
        {
            get { return _isISRCSearching;}
            set
            {
                _isISRCSearching = value;
                RaisePropertyChanged(nameof(IsISRCSearching));
            }
        }

        public bool IsSaving
        {
            get { return _isSaving; }
            set
            {
                _isSaving = value;
                RaisePropertyChanged(nameof(IsSaving));
            }
        }

        public TrackViewModel TempTrack
        {
            get
            {
                if (SelectedItems == null)
                {
                    return null;
                }
                return SelectedItems.Count != 0 ? SelectedItems.FirstOrDefault() : GenerateTempTrack();
            }
        }

        public bool IsPlayerMode
        {
            get { return _isPlayerMode; }
            set
            {
                _isPlayerMode = value;
                RaisePropertyChanged(nameof(IsPlayerMode));
            }
        }

        public bool HasItems
        {
            get { return Tracks.Any(); }
        }

        public bool HasSelected
        {
            get { return SelectedItems.Any(); }
        }
        #endregion

        #region Commands

        private RelayCommand _nextTrackcomaand;
        private RelayCommand _openDialogCommand;
        private RelayCommand<object> _selectionChangedCommand;
        private RelayCommand<bool> _changeModeCommand;
        private RelayCommand _isrcSearchCommand;
        private RelayCommand _insertISRCCommand;
        private RelayCommand _saveTags;
        private RelayCommand _removeItemsCommand;

        public RelayCommand RemoveItemsCommand
        {
            get
            {
                return _removeItemsCommand ?? (_removeItemsCommand = new RelayCommand(()=> 
                {
                    _isRemoving = true;
                    if (MessageBox.Show("Do you want to remove this item(-s)", "Remove dialog", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                        var itemsToRemove = SelectedItems.ToArray();
                        foreach (var item in itemsToRemove)
                        {
                            Tracks.Remove(item);
                        }

                        _counterForId = 1;

                        foreach (var item in Tracks)
                        {
                            item.ID = _counterForId;
                            _counterForId += 1;
                        }
                    }

                    _isRemoving = false;
                }));
            }
        }

        public RelayCommand OpenDialogCommand
        {
            get
            {
                return _openDialogCommand ?? (_openDialogCommand = new RelayCommand(() =>
                  {
                      var vm = new OpenFoldersDialogViewModel();
                      var view = new OpenFoldersDialog() { Owner = Application.Current.MainWindow, DataContext = vm };
                      if (view.ShowDialog() != true) return;
                      var selectedFolders = vm.GetSelectedFolders();
                      InspectFolders(selectedFolders);
                  }));
            }
        }

        public RelayCommand<object> SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand<object>(items =>
                  {
                      if (_isRemoving || _isInspecting)
                          return;
                      IEnumerable<object> set = items as IEnumerable<object>;

                      if (set == null)
                          throw new InvalidOperationException();

                      SelectedItems = set.Cast<TrackViewModel>().ToList();
                      RaisePropertyChanged(nameof(TempTrack));
                      RaisePropertyChanged(nameof(HasSelected));
                  }));
            }
        }

        public RelayCommand<bool> ChangeModeCommand
        {
            get
            {
                return _changeModeCommand ?? (_changeModeCommand = new RelayCommand<bool>((val) =>
                  {
                      IsPlayerMode = val;
                  }));
            }
        }

        public RelayCommand InsertISRCCommand
        {
            get
            {
                return _insertISRCCommand ?? (_isrcSearchCommand = new RelayCommand((() =>
                           {
                               TempTrack.ISRC = Clipboard.GetText();
                           })));
            }
        }

        public RelayCommand ISRCSearchCommand
        {
            get { return _isrcSearchCommand ?? (_isrcSearchCommand = new RelayCommand(SearchIsrcAsync));}
        }

        public RelayCommand SaveTags
        {
            get
            { return _saveTags ?? (_saveTags = new RelayCommand(() =>
                {
                    if (SelectedItems.Count == 1)
                    {
                        SaveAsync(new[] { TempTrack });
                    }
                }));
            }
        }

        #endregion

        #region AsyncItems
        private Task _isrcSearchTask;
        private Task _saveTask;
        private bool _isRemoving;
        private bool _isInspecting;
        private int _max;
        private int _value;

        private async void SearchIsrcAsync()
        {
            if (_isrcSearchTask == null)
            {
                _isrcSearchTask = Task.Run(() =>
                {
                    IsISRCSearching = true;
                    ISRCRequestManager rm = ISRCRequestManager.Inst;
                    DisplayDoc result;
                    var re = new Regex("#.*?;");
                    var isrc = re.Replace(TempTrack.ISRC, ";");
                    if (isrc.Length == ISRCCount)
                    {
                        result = rm.GetFromIsrc(isrc).DisplayDocs.FirstOrDefault();

                        TempTrack.Artist = string.IsNullOrEmpty(result?.ArtistName) ? TempTrack.Artist : result.ArtistName;
                        TempTrack.Title = string.IsNullOrEmpty(result?.TrackTitle) ? TempTrack.Title : result.TrackTitle;
                        TempTrack.Year = string.IsNullOrEmpty(result?.RecordingYear) ? TempTrack.Year : result.RecordingYear;
                    }
                    else if (!string.IsNullOrEmpty(TempTrack.Artist) && !string.IsNullOrEmpty(TempTrack.Title))
                    {
                        var response = rm.GetFromData(TempTrack.Artist, TempTrack.Title).DisplayDocs;
                        var req = response.Where(x =>
                            string.CompareOrdinal(x.ArtistName, TempTrack.Artist) == 0 &&
                            string.CompareOrdinal(x.TrackTitle, TempTrack.Title) == 0).ToArray();

                        if (req.Any())
                        {
                            if (req.Length > 1 && string.IsNullOrEmpty(TempTrack.Year))
                            {
                                result = req.FirstOrDefault();
                            }
                            else
                            {
                                result = req.FirstOrDefault();
                            }

                            TempTrack.ISRC = result?.IsrcCode;
                        }
                        else
                        {
                            MessageBox.Show("Записей не найдено");
                        }
                    }
                }).ContinueWith(o =>
                {
                    IsISRCSearching = false;
                    _isrcSearchTask = null;
                });
                await _isrcSearchTask;
            }
        }

        private Task SaveAsync(IEnumerable<TrackViewModel> tracks)
        {
            if (_saveTask == null)
            {
                IsSaving = true;
                _saveTask = Task.Run(() =>
                {
                    foreach (var item in tracks)
                    {
                        item.SaveTag();
                    }
                }).ContinueWith(o =>
                {
                    _saveTask = null;
                    IsSaving = false;
                });
            }

            return _saveTask;
        }
        #endregion

        #region Methods


        private Task InspectFolders(IEnumerable<string> selectedFolders)
        {
            var enumerable = selectedFolders.ToList();

            IsInsp = true;
            Max = selectedFolders.Select(o => Directory.GetFiles(o, "*.mp3", SearchOption.TopDirectoryOnly).Count()).Sum();
            return Task.Run(() =>
             {
                 var date = DateTime.Now;
                 System.Diagnostics.Debug.WriteLine("start");
                 if (!selectedFolders?.Any() ?? false)
                     return;
                 List<TrackViewModel> tracks = new List<TrackViewModel>();

                 foreach (var item in selectedFolders)
                 {
                     if (!OpenedFolders.Contains(item))
                     {
                         OpenedFolders.Add(item);
                     }
                     var files = Directory.GetFiles(item, "*.mp3");
                     foreach (var file in files.Where(o => o.Split('.').LastOrDefault() == "mp3"))
                     {
                         if (Tracks.Any(o => string.Compare(o.Path, file, StringComparison.OrdinalIgnoreCase) == 0))
                             continue;
                         try
                         {
                             App.Current.Dispatcher.Invoke(() =>
                             {
                                 Value++;
                             });
                             tracks.Add(new TrackViewModel(file, item, _counterForID++));
                         }
                         catch (Exception e)
                         {
                             MessageBox.Show(e.Message);
                         }

                     }
                 }
                 App.Current.Dispatcher.Invoke(() =>
                 {
                     foreach (var item in tracks)
                     {
                         Tracks.Add(item);
                     }

                 });
                 System.Diagnostics.Debug.WriteLine("end   " + (DateTime.Now - date));
             });
        }

        private TrackViewModel GenerateTempTrack()
        {
            var temp = new TrackViewModel();

            return temp;
        }

        private void Fols_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasItems));
        }
        #endregion
        
        public MainViewModel()
        {
            if (Properties.Settings.Default.OpenedFolders != null)
            {
                InspectFolders(Properties.Settings.Default.OpenedFolders.Cast<string>());
            }
            Tracks.CollectionChanged += Fols_CollectionChanged;
        }

    }
}