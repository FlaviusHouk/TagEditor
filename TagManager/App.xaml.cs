using System.Windows;
using TagManager.Services;
using TagManager.ViewModel;
using TagManager.ViewModel.Services;

namespace TagManager
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        public MainViewModel MainViewModel
        {
            get;
        }

        public App()
        {
            IDialogService dialogService = new DialogService();
            IDispatcherService dispatcherService = new DispatcherService();
            IResourceService resourceService = new ResourceService();

            MainViewModel = new MainViewModel(dialogService, dispatcherService, resourceService);

            Resources.Add("MainViewModel", MainViewModel);
        }
	}
}
