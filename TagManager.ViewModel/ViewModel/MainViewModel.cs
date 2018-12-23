using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TagManager.ViewModel.WebRequests;
using TagManager.ViewModel.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive;

namespace TagManager.ViewModel
{
    public class MainViewModel : ReactiveObject 
    {
        #region Fields
        private bool _isPlayerMode = true;
        private const int ISRCCount = 12;
        private bool _isISRCSearching;
        private bool _isSaving;
        private int _counterForID = 1;
        #endregion

        #region Services

        IDialogService _dialogService;
        IResourceService _resourceService;
        IDispatcherService _dispatcherService;

        #endregion

        #region Properties

        public List<string> OpenedFolders { get; private set; } = new List<string>();

        public ObservableCollection<TrackViewModel> Tracks { get; private set; } = new ObservableCollection<TrackViewModel>();

        public ObservableCollection<TrackViewModel> SelectedItems
        {
            get;
            set;
        } = new ObservableCollection<TrackViewModel>();

        private readonly ObservableAsPropertyHelper<string> _progress;
        public string Progress
        {
            get
            {
                return _progress.Value;
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
                this.RaiseAndSetIfChanged(ref _isInspecting, value);
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
                this.RaiseAndSetIfChanged(ref _value, value);
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
                this.RaiseAndSetIfChanged(ref _max, value);
            }
        }

        public bool IsISRCSearching
        {
            get { return _isISRCSearching;}
            set
            {
                this.RaiseAndSetIfChanged(ref _isISRCSearching, value);
            }
        }

        public bool IsSaving
        {
            get { return _isSaving; }
            set
            {
                this.RaiseAndSetIfChanged(ref _isSaving, value);
            }
        }

        private readonly ObservableAsPropertyHelper<TrackViewModel> _tempTrack;
        public TrackViewModel TempTrack
        {
            get
            {
                return _tempTrack.Value;
            }
        }

        public bool IsPlayerMode
        {
            get { return _isPlayerMode; }
            set
            {
                this.RaiseAndSetIfChanged(ref _isPlayerMode, value);
            }
        }

        private readonly ObservableAsPropertyHelper<bool> _hasItems;
        public bool HasItems
        {
            get { return _hasItems.Value; }
        }

        private readonly ObservableAsPropertyHelper<bool> _hasSelected;
        public bool HasSelected
        {
            get { return _hasSelected.Value; }
        }
        #endregion

        #region Commands

        private ReactiveCommand<Unit, Unit> _nextTrackcomaand;
        private ReactiveCommand<Unit, Unit> _openDialogCommand;
        private ReactiveCommand<object, Unit> _selectionChangedCommand;
        private ReactiveCommand<string, Unit> _changeModeCommand;
        private ReactiveCommand<Unit, Unit> _isrcSearchCommand;
        private ReactiveCommand<Unit, Unit> _insertISRCCommand;
        private ReactiveCommand<Unit, Unit> _saveTags;
        private ReactiveCommand<Unit, Unit> _removeItemsCommand;
        private ReactiveCommand<Unit, Unit> _unselelectAllCommand;

        public ReactiveCommand<Unit, Unit> UnselectAllCommand
        {
            get
            {
                return _unselelectAllCommand ?? (_unselelectAllCommand = ReactiveCommand.Create(() =>
                {
                    foreach (TrackViewModel track in Tracks)
                        track.IsSelected = false;
                }));
            }
        }

        public ReactiveCommand<Unit, Unit> RemoveItemsCommand
        {
            get
            {
                return _removeItemsCommand ?? (_removeItemsCommand = ReactiveCommand.Create (()=> 
                {
                    
                    _isRemoving = true;
                    if (MessageBox.Show("Do you want to remove this item(-s)", "Remove dialog", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                        var itemsToRemove = SelectedItems.ToArray();
                        foreach (var item in itemsToRemove)
                        {
                            Tracks.Remove(item);
                        }

                        _counterForID = 1;

                        foreach (var item in Tracks)
                        {
                            item.ID = _counterForID;
                            _counterForID += 1;
                        }
                    };
                    _isRemoving = false;
                }));
            }
        }

        public ReactiveCommand<Unit, Unit> NextTrackCommand
        {
            get
            {
                return _nextTrackcomaand ?? (_nextTrackcomaand = ReactiveCommand.Create(() =>
                {
                }));
            }
        }

        public ReactiveCommand<Unit, Unit> OpenDialogCommand
        {
            get
            {
                return _openDialogCommand ?? (_openDialogCommand = ReactiveCommand.Create(() =>
                  {
                      OpenFoldersDialogViewModel vm = new OpenFoldersDialogViewModel(this);
                      _dialogService.ShowOpenFolderDialog(vm);
                      var selectedFolders = vm.GetSelectedFolders();

                      if (selectedFolders != null)
                        InspectFolders(selectedFolders);
                  }));
            }
        }

        public ReactiveCommand<object, Unit> SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = ReactiveCommand.Create<object, Unit>(items =>
                  {
                      if (_isRemoving || _isInspecting)
                          return Unit.Default;
                      IEnumerable<object> set = items as IEnumerable<object>;

                      if (set == null)
                          throw new InvalidOperationException();

                      SelectedItems.Clear();
                      foreach (TrackViewModel track in set)
                      {
                          SelectedItems.Add(track);
                      }

                      return Unit.Default;
                  }));
            }
        }

        public ReactiveCommand<string, Unit> ChangeModeCommand
        {
            get
            {
                return _changeModeCommand ?? (_changeModeCommand = ReactiveCommand.Create<string, Unit>((val) =>
                  {
                      IsPlayerMode = Convert.ToBoolean(val);
                      return Unit.Default;
                  }));
            }
        }

        public ReactiveCommand<Unit, Unit> InsertISRCCommand
        {
            get
            {
                return _insertISRCCommand ?? (_isrcSearchCommand = ReactiveCommand.Create((() =>
                           {
                               TempTrack.ISRC = Clipboard.GetText();
                           })));
            }
        }

        public ReactiveCommand<Unit, Unit> ISRCSearchCommand
        {
            get { return _isrcSearchCommand ?? (_isrcSearchCommand = ReactiveCommand.Create(() => { SearchISRCAsync(); }));}
        }

        public ReactiveCommand<Unit, Unit> SaveTags
        {
            get
            { return _saveTags ?? (_saveTags = ReactiveCommand.Create(() =>
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

        private async void SearchISRCAsync()
        {
            if (_isrcSearchTask == null)
            {
                _isrcSearchTask = Task.Run(() =>
                {
                    IsISRCSearching = true;
                    ISRCRequestManager rm = ISRCRequestManager.Inst;
                    DisplayDoc result = null;
                    var re = new Regex("#.*?;");
                    var isrc = re.Replace(TempTrack.ISRC, ";");
                    if (isrc.Length == ISRCCount)
                    {
                        result = rm.GetFromISRC(isrc).DisplayDocs.FirstOrDefault();

                        TempTrack.Artist = string.IsNullOrEmpty(result.ArtistName) ? TempTrack.Artist : result.ArtistName;
                        TempTrack.Title = string.IsNullOrEmpty(result.TrackTitle) ? TempTrack.Title : result.TrackTitle;
                        TempTrack.Year = string.IsNullOrEmpty(result.RecordingYear) ? TempTrack.Year : result.RecordingYear;
                    }
                    else if (!string.IsNullOrEmpty(TempTrack.Artist) && !String.IsNullOrEmpty(TempTrack.Title))
                    {
                        var response = rm.GetFromData(TempTrack.Artist, TempTrack.Title).DisplayDocs;
                        var req = response.Where(x =>
                            string.CompareOrdinal(x.ArtistName, TempTrack.Artist) == 0 &&
                            string.CompareOrdinal(x.TrackTitle, TempTrack.Title) == 0).ToArray();
                        if (req.Any())
                        {
                            if (req.Length > 1 && string.IsNullOrEmpty(TempTrack.Year.ToString()))
                            {
                                result = req.FirstOrDefault();
                            }
                            else
                            {
                                result = req.FirstOrDefault();
                            }

                            TempTrack.ISRC = result.IsrcCode;
                        }
                        else
                        {
                            MessageBox.Show("Записей не найдено");
                            return;
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

        private async void SaveAsync(IEnumerable<TrackViewModel> tracks)
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

            await _saveTask;
        }
        #endregion

        #region Methods


        private async void InspectFolders(IEnumerable<string> selectedFolders)
        {
            Value = 1;
            IsInsp = true;
            Max = selectedFolders.Select(o => Directory.GetFiles(o, "*.mp3", SearchOption.TopDirectoryOnly).Count()).Sum();
            await Task.Run(() =>
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
                            _dispatcherService.RunInUIThread(() =>
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
                
                _dispatcherService.RunInUIThread(() =>
                {
                    foreach (var item in tracks)
                    {
                        Tracks.Add(item);
                    }
                    IsInsp = false;

                });
                System.Diagnostics.Debug.WriteLine("end   " + (DateTime.Now - date));
            });
        }

        private TrackViewModel GenerateTempTrack()
        {
            var temp = new TrackViewModel();

            return temp;
        }
        #endregion

        public MainViewModel(IDialogService dialogService,
                             IDispatcherService dispatcherService,
                             IResourceService resourceService)
        {
            _dialogService = dialogService;
            _dispatcherService = dispatcherService;
            _resourceService = resourceService;

            _progress = this.WhenAnyValue(x => x.Value, x => x.Max)
                        .Select(val => $"{val.Item1} / {val.Item2}")
                        .ToProperty(this, x => x.Progress, string.Empty, false, ImmediateScheduler.Instance);

            _tempTrack = SelectedItems.WhenAnyValue(x => x.Count)
                         .Select(t =>
                         {
                             if (t == 0)
                             {
                                 return GenerateTempTrack();
                             }

                             return SelectedItems.FirstOrDefault();
                         })
                         .ToProperty(this, x => x.TempTrack, null, false, ImmediateScheduler.Instance);

            _hasSelected = SelectedItems.WhenAnyValue(x => x.Count)
                               .Select(val => val > 0)
                               .ToProperty(this, x => x.HasSelected, false, false, ImmediateScheduler.Instance);

            _hasItems = Tracks.WhenAnyValue(x => x.Count)
                              .Select(val => val != 0)
                              .ToProperty(this, x => x.HasItems, false, false, ImmediateScheduler.Instance);

            IEnumerable<string> folders = _resourceService.GetSetting<IEnumerable<string>>("OpenedFolders")?.Cast<string>();
            if (folders != null)
            {
                InspectFolders(folders);
            }
        }

    }
}