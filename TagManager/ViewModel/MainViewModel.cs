using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace TagManager.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
        #region Fields

        #endregion

        #region Properties
        public string Source { get; set; } = @"C:/Users/danyil.hryhoriev/Desktop/cover.jpg";
        #endregion

        #region Commands
        private RelayCommand _nextTrack;


        public RelayCommand NextTrack
        {
            get
            {
                return _nextTrack ?? (_nextTrack = new RelayCommand(() => 
                {
                    Source = @"C:/Users/danyil.hryhoriev/Desktop/1.jpg";
                    RaisePropertyChanged(nameof(Source));
                }));
            }
        }

        #endregion

        public MainViewModel()
		{
		}
	}
}