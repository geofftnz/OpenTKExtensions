using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Diagnostics;
using OpenTKExtensions.Resources;

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
            log.Info("GameComponentBase.Load({0}) loading", GetType().Name);

            if (Status != ComponentStatus.New && Status != ComponentStatus.Unloaded)
            {
                log.Info("GameComponentBase.Load({0}) already loaded", GetType().Name);
                return;
            }

            Status = ComponentStatus.Loading;
            OnLoading(EventArgs.Empty);
            Resources.Load();
            Status = ComponentStatus.Loaded;
            OnLoaded(EventArgs.Empty);

            log.Info("GameComponentBase.Load({0}) loaded", GetType().Name);
        }

        public void Unload()
        {
            log.Info("GameComponentBase.Unload({0}) unloading", GetType().Name);

            if (Status != ComponentStatus.Loaded)
            {
                log.Info("GameComponentBase.Unload({0}) already unloaded", GetType().Name);
                return;
            }

            Status = ComponentStatus.Unloading;
            OnUnloading(EventArgs.Empty);
            Resources.Unload();
            Status = ComponentStatus.Unloaded;
            log.Info("GameComponentBase.Unload({0}) unloaded", GetType().Name);
        }


        /// <summary>
        /// Occurs when this component is being loaded.
        /// Derived classes should handle this event to load resources.
        /// </summary>
        public event EventHandler<EventArgs> Loading;


        public virtual void OnLoading(EventArgs e)
        {
            Loading?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when this component is being unloaded (all resources being released)
        /// Derived classes should handle this event
        /// </summary>
        public event EventHandler<EventArgs> Unloading;

        public virtual void OnUnloading(EventArgs e)
        {
            Unloading?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs after the component has been loaded.
        /// </summary>
        public event EventHandler<EventArgs> Loaded;

        public virtual void OnLoaded(EventArgs e)
        {
            Loaded?.Invoke(this, e);
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
