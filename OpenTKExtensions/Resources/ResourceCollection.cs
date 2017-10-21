using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Resources
{
    public class ResourceCollection : Dictionary<string, IResource>, IDictionary<string, IResource>
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public ResourceCollection()
        {

        }

        public void Load()
        {
            foreach (var resource in this.Values)
            {
                resource.Load();
            }
        }

        public void Unload()
        {
            foreach (var resource in this.Values)
            {
                resource.Unload();
            }
        }

        public void Reload()
        {
            string message;

            foreach (var resource in this.Values.OfType<IReloadableResource>())
            {
                if (resource.TryReload(out message))
                {
                    log.Info(() => message);
                }
                else
                {
                    log.Error(() => message);
                }
            }
        }

        private string GetNextKey()
        {
            // TODO: what if this key has already been added?
            return $"__res_{Count:00000}";
        }

        public string Add(IResource resource)
        {
            string key = resource.Name;

            if (ContainsKey(key))
                key = GetNextKey();

            Add(key, resource);
            return key;
        }

        public T Get<T>(string name) where T : class, IResource
        {
            IResource value;

            if (this.TryGetValue(name, out value))
            {
                T tvalue = value as T;
                if (tvalue != null)
                    return tvalue;
                else
                    throw new InvalidOperationException($"Resource '{name}' found, but was of type {value.GetType().FullName}, not {typeof(T).FullName}.");
            }
            else
            {
                throw new KeyNotFoundException($"Resource '{name}' not found.");
            }
        }

        public Texture Texture(string name)
        {
            return Get<Texture>(name);
        }

    }
}
