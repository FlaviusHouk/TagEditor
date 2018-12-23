using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagManager.ViewModel.Services
{
    public interface IResourceService
    { 
        string GetSetting(string key);
        string GetResource(string key);

        T GetResource<T>(string key);
        T GetSetting<T>(string key);
    }
}
