using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagManager.ViewModel.Services
{
    public interface IDispatcherService
    {
        void RunInUIThread(Action act);
        Task RunInUIThreadAsync(Action act);
    }
}
