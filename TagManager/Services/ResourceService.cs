using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagManager.ViewModel.Services;

namespace TagManager.Services
{
    class ResourceService : IResourceService
    {
        public string GetResource(string key)
        {
            return Properties.Resources.ResourceManager.GetString(key);
        }

        public T GetResource<T>(string key)
        {
            object resource = Properties.Resources.ResourceManager.GetObject(key);

            return resource is T ? (T)resource : default(T);
        }

        public string GetSetting(string key)
        {
            return Properties.Settings.Default[key] as string;
        }

        public T GetSetting<T>(string key)
        {
            object setting = Properties.Settings.Default[key];

            if (setting is StringCollection && typeof(T).Equals(typeof(IEnumerable<string>)))
                return (T)((setting as StringCollection)?.OfType<string>());

            return setting is T ? (T)setting : default(T);
        }
    }
}