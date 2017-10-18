using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Resources
{
    public abstract class ResourceBase
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public string Name { get; protected set; }

        public ResourceBase(string name)
        {
            log.Info($"Resource({GetType().Name}).ctor({name})");
            this.Name = name;            
        }

        public ResourceBase() : this("UnamedResource")
        {
        }

        public event EventHandler<EventArgs> ReadyForContent;
        protected void OnReadyForContent(EventArgs e)
        {
            ReadyForContent?.Invoke(this, e);
        }
        protected void OnReadyForContent()
        {
            OnReadyForContent(new EventArgs());
        }

        public event EventHandler<EventArgs> Unloaded;
        protected void OnUnloaded(EventArgs e)
        {
            Unloaded?.Invoke(this, e);
        }
        protected void OnUnloaded()
        {
            OnUnloaded(new EventArgs());
        }


    }
}
