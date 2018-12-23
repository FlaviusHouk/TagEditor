using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

using TagManager.ViewModel.Services;

namespace TagManager.Services
{
    class DispatcherService : IDispatcherService
    {
        public void RunInUIThread(Action act)
        {
            App.Current.Dispatcher.Invoke(act);
        }

        public async Task RunInUIThreadAsync(Action act)
        {
            await App.Current.Dispatcher.InvokeAsync(act);
        }
    }
}
