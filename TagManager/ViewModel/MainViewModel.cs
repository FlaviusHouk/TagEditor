﻿using System;
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
using ViewModel;
using WebRequests;

namespace TagManager.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private bool _isPlayerMode = true;
        private const int ISRCCount = 12;
        private bool _isISRCSearching;
        private bool _isSaving;
        private int CounterForID = 1;
        #endregion

        #region Properties

        public string Source { get; set; } = @"C:/Users/danyil.hryhoriev/Desktop/cover.jpg";

        public List<string> OpenedFolders { get; private set; } = new List<string>();

        public ObservableCollection<TrackViewModel> Tracks { get; private set; } = new ObservableCollection<TrackViewModel>();

        public List<TrackViewModel> SelectedItems { get; set; } = new List<TrackViewModel>();

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

        public RelayCommand NextTrackCommand
        {
            get
            {
                return _nextTrackcomaand ?? (_nextTrackcomaand = new RelayCommand(() =>
                {
                    Source = @"C:/Users/danyil.hryhoriev/Desktop/1.jpg";
                    RaisePropertyChanged(nameof(Source));
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
            get { return _isrcSearchCommand ?? (_isrcSearchCommand = new RelayCommand(() => { SearchISRCAsync(); }));}
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


        private void InspectFolders(IEnumerable<string> selectedFolders)
        {
            if (!selectedFolders?.Any() ?? false)
                return;

            OpenedFolders.AddRange(selectedFolders);
            foreach (var item in selectedFolders)
            {
                var files = Directory.GetFiles(item);
                foreach (var file in files.Where(o => o.Split('.').LastOrDefault() == "mp3"))
                {
                    if (Tracks.Any(o => string.Compare(o.Path, file, StringComparison.OrdinalIgnoreCase) == 0))
                        return;
                    try
                    {
                        Tracks.Add(new TrackViewModel(file, item, CounterForID++));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                }
            }
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
            InspectFolders(Properties.Settings.Default.OpenedFolders.Cast<string>());
            Tracks.CollectionChanged += Fols_CollectionChanged;
        }

    }
}