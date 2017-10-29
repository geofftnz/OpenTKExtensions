using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Diagnostics;
using OpenTKExtensions.Resources;
using System.Runtime.CompilerServices;

namespace OpenTKExtensions.Framework
{
    public class GameComponentBase : IGameComponent, ITimedComponent
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

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
            LogTrace("loading");

            if (Status != ComponentStatus.New && Status != ComponentStatus.Unloaded)
            {
                LogInfo("already loaded");
                return;
            }

            Status = ComponentStatus.Loading;
            OnLoading(EventArgs.Empty);
            Resources.Load();
            Status = ComponentStatus.Loaded;
            OnLoaded(EventArgs.Empty);

            LogTrace("loaded");
        }

        public void Unload()
        {
            LogTrace("unloading");

            if (Status != ComponentStatus.Loaded)
            {
                LogInfo("already unloaded");
                return;
            }

            Status = ComponentStatus.Unloading;
            OnUnloading(EventArgs.Empty);
            Resources.Unload();
            Status = ComponentStatus.Unloaded;
            LogTrace("unloaded");
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

        /// <summary>
        /// Occurs before this component is being rendered (If it is IRenderable).
        /// Derived classes should handle this event to set any per-frame resources.
        /// </summary>
        public event EventHandler<EventArgs> PreRender;

        public void OnPreRender()
        {
            PreRender?.Invoke(this, new EventArgs());
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

        private string LogPad()
        {
            return string.Join("", Enumerable.Range(0, new StackTrace().FrameCount).Select(i => " "));
        }

        protected void LogTrace(string message, [CallerMemberName] string caller = null)
        {
            log.Trace($"{LogPad()}{this.GetType().Name}.{caller}: {message}");
        }
        protected void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            log.Info($"{LogPad()}{this.GetType().Name}.{caller}: {message}");
        }
        protected void LogWarn(string message, [CallerMemberName] string caller = null)
        {
            log.Warn($"{LogPad()}{this.GetType().Name}.{caller}: {message}");
        }
        protected void LogError(string message, [CallerMemberName] string caller = null)
        {
            log.Error($"{LogPad()}{this.GetType().Name}.{caller}: {message}");
        }

    }
}
