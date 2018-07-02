using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        #endregion

        #region Properties

        public string Source { get; set; } = @"C:/Users/danyil.hryhoriev/Desktop/cover.jpg";

        public ObservableCollection<TrackViewModel> Fols { get; private set; } = new ObservableCollection<TrackViewModel>();

        public List<TrackViewModel> SelectedItems { get; set; } = new List<TrackViewModel>();

        public TrackViewModel TempTrack
        {
            get
            {
                if (SelectedItems == null)
                {
                    return null;
                }
                return SelectedItems.Count == 1 ? SelectedItems.FirstOrDefault() : GenerateTempTrack();
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
            get { return Fols.Any(); }
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
                      int index = 1;
                      var s = vm.GetSelectedFolders();
                      foreach (var item in s)
                      {
                          var files = Directory.GetFiles(item);
                          foreach (var file in files.Where(o => o.Split('.').LastOrDefault() == "mp3"))
                          {
                              Fols.Add(new TrackViewModel(file,item,index++));
                          }
                      }
                  }));
            }
        }

        public RelayCommand<object> SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand<object>((items) =>
                  {
                      SelectedItems = ((items as IEnumerable<object>) ?? throw new InvalidOperationException()).Cast<TrackViewModel>().ToList();
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
        { get
            {
                return _insertISRCCommand ?? (_isrcSearchCommand = new RelayCommand((() =>
                           {
                               TempTrack.ISRC = Clipboard.GetText();
                           })));
            }
        }

        public RelayCommand ISRCSearchCommand
        {
            get { return _isrcSearchCommand ?? (_isrcSearchCommand = new RelayCommand(() =>
            {
                ISRCRequestManager rm = ISRCRequestManager.Inst;
                DisplayDoc result = null;
                var re = new Regex("#.*?;");
                var isrc = re.Replace(TempTrack.ISRC, ";");
                if (isrc.Length == ISRCCount)
                {
                    result = rm.GetFromISRC(isrc).DisplayDocs.FirstOrDefault();
                    
                    TempTrack.Artist = string.IsNullOrEmpty(result.ArtistName) ? TempTrack.Artist : result.ArtistName;
                    TempTrack.Title = string.IsNullOrEmpty(result.TrackTitle) ? TempTrack.Title : result.TrackTitle;
                    TempTrack.Year = string.IsNullOrEmpty(result.RecordingYear) ? TempTrack.Year : Int32.Parse(result.RecordingYear);
                }
                else if(!string.IsNullOrEmpty(TempTrack.Artist) && !String.IsNullOrEmpty(TempTrack.Title))
                {
                    var response = rm.GetFromData(TempTrack.Artist, TempTrack.Title).DisplayDocs;
                    var req = response.Where(x => string.CompareOrdinal(x.ArtistName, TempTrack.Artist) == 0 && string.CompareOrdinal(x.TrackTitle, TempTrack.Title) == 0).ToArray();
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
            }));}
        }

        #endregion


        private TrackViewModel GenerateTempTrack()
        {
            var temp = new TrackViewModel();

            return temp;
        }

        public MainViewModel()
        {
            Fols.CollectionChanged += Fols_CollectionChanged;
        }

        private void Fols_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasItems));
        }
    }
}