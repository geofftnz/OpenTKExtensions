using NLog;
using OpenTKExtensions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    public class ReloadableResource<T> : ResourceBase, IResource, IReloadableResource where T : IResource
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public T Resource { get; private set; }

        private Func<T> Create = null;
        private Func<T, T> Reload = null;

        public ReloadableResource(string name, Func<T> create, Func<T, T> reload) : base(name)
        {
            if (create == null)
                throw new ArgumentNullException(nameof(create));
            if (reload == null)
                throw new ArgumentNullException(nameof(reload));

            Create = create;
            Reload = reload;

            Resource = Create();
            if (Resource == null)
                throw new InvalidOperationException($"ReloadableResource.ctor ({Name}): Could not create resource.");
        }

        public void Load()
        {
            Resource?.Load();
        }

        public void Unload()
        {
            Resource?.Unload();
        }

        public bool TryReload(out string message)
        {
            log.Info($"ReloadableResource.TryReload attempting reload of {Name}...");
            try
            {
                T newResource = Reload(Resource);

                if (newResource == null)
                    throw new InvalidOperationException($"ReloadableResource.TryReload: Reload returned null.");

                if (!ReferenceEquals(Resource, newResource))
                {
                    log.Info($"ReloadableResource.TryReload new object created for {Name}...");
                    newResource.Load();
                    Resource.Unload();
                    Resource = newResource;
                }
                else
                {
                    log.Info($"ReloadableResource.TryReload in-place reload of {Name}...");
                }

                message = $"ReloadableResource.TryReload {Resource.Name} reloaded.";
                return true;
            }
            catch (ShaderCompileException ex)
            {
                message = $"{Resource.Name}: {ex.DetailedError}";
                return false;
            }
            catch (Exception ex)
            {
                message = $"{Resource.Name}: {ex.GetType().Name} - {ex.Message}";
                return false;
            }


        }
    }
}