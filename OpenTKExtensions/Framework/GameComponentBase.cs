using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Diagnostics;

namespace OpenTKExtensions.Framework
{
    public class GameComponentBase : IGameComponent, ITimedComponent
    {
        protected static Logger log = LogManager.GetCurrentClassLogger();

        public ComponentStatus Status { get; protected set; } = ComponentStatus.New;
        public int LoadOrder
        {
            get;
            set;
        }

        private Stopwatch renderTimer = new Stopwatch();
        private TimeSpan lastRenderTime = TimeSpan.Zero;
        public TimeSpan LastRenderTime
        {
            get
            {
                return lastRenderTime;
            }
        }

        public ResourceCollection Resources { get; } = new ResourceCollection();

        public GameComponentBase()
        {
        }

        /// <summary>
        /// Loads the component and all subcomponents
        /// </summary>
        public void Load()
        {
            log.Info("GameComponentBase.Load({0}) loading", this.GetType().Name);

            if (this.Status != ComponentStatus.New && this.Status != ComponentStatus.Unloaded)
            {
                log.Info("GameComponentBase.Load({0}) already loaded", this.GetType().Name);
                return;
            }

            this.Status = ComponentStatus.Loading;
            this.OnLoading(EventArgs.Empty);
            this.Resources.Load();
            this.Status = ComponentStatus.Loaded;
            this.OnLoaded(EventArgs.Empty);

            log.Info("GameComponentBase.Load({0}) loaded", this.GetType().Name);
        }

        public void Unload()
        {
            log.Info("GameComponentBase.Unload({0}) unloading", this.GetType().Name);

            if (this.Status != ComponentStatus.Loaded)
            {
                log.Info("GameComponentBase.Unload({0}) already unloaded", this.GetType().Name);
                return;
            }

            this.Status = ComponentStatus.Unloading;
            this.OnUnloading(EventArgs.Empty);
            this.Resources.Unload();
            this.Status = ComponentStatus.Unloaded;
            log.Info("GameComponentBase.Unload({0}) unloaded", this.GetType().Name);
        }


        /// <summary>
        /// Occurs when this component is being loaded.
        /// Derived classes should handle this event to load resources.
        /// </summary>
        public event EventHandler<EventArgs> Loading;


        public virtual void OnLoading(EventArgs e)
        {
            this.Loading?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when this component is being unloaded (all resources being released)
        /// Derived classes should handle this event
        /// </summary>
        public event EventHandler<EventArgs> Unloading;

        public virtual void OnUnloading(EventArgs e)
        {
            this.Unloading?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs after the component has been loaded.
        /// </summary>
        public event EventHandler<EventArgs> Loaded;

        public virtual void OnLoaded(EventArgs e)
        {
            this.Loaded?.Invoke(this, e);
        }

        public void StartRenderTimer()
        {
            renderTimer.Reset();
            renderTimer.Start();
        }

        public void StopRenderTimer()
        {
            renderTimer.Stop();
            lastRenderTime = renderTimer.Elapsed;
        }
    }
}
