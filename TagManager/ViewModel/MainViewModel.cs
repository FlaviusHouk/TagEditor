using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TagManager.View;
using ViewModel;

namespace TagManager.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private bool _isPlayerMode = true;

        #endregion

        #region Properties

        public string Source { get; set; } = @"C:/Users/danyil.hryhoriev/Desktop/cover.jpg";

        public ObservableCollection<TrackViewModel> Fols { get; private set; } = new ObservableCollection<TrackViewModel>();

        public List<TrackViewModel> SelectedItems { get; set; }

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
        #endregion

        #region Commands

        private RelayCommand _nextTrackcomaand;
        private RelayCommand _openDialogCommand;
        private RelayCommand<object> _selectionChangedCommand;
        private RelayCommand<bool> _changeModeCommand;


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
                      var view = new OpenFoldersDialog() { Owner = App.Current.MainWindow, DataContext = vm };
                      if (view.ShowDialog() == true)
                      {
                          int index = 0;
                          var s = vm.GetSelectedFolders();
                          foreach (var item in s)
                          {
                              var files = Directory.GetFiles(item);
                              foreach (var file in files.Where(o => o.Split('.').LastOrDefault() == "mp3"))
                              {
                                  Fols.Add(new TrackViewModel(file,item,index++));
                              }
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
                      SelectedItems = (items as IEnumerable<object>).Cast<TrackViewModel>().ToList();
                      RaisePropertyChanged(nameof(TempTrack));
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
        #endregion

        public MainViewModel()
        {
        }


        private TrackViewModel GenerateTempTrack()
        {
            var temp = new TrackViewModel();

            foreach (var item in SelectedItems)
            {
            }
            return temp;
        }

    }
}