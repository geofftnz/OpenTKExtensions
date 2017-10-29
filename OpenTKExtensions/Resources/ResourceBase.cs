using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace OpenTKExtensions.Resources
{
    public abstract class ResourceBase
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public string Name { get; protected set; }

        public ResourceBase(string name)
        {
            Name = name;
            LogTrace($" constructed");
        }

        public ResourceBase() : this("UnamedResource")
        {
        }

        public event EventHandler<EventArgs> ReadyForContent;
        protected void OnReadyForContent(EventArgs e)
        {
            if (ReadyForContent != null)
                LogTrace($" ready for content");

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

        private string LogPad()
        {
            return string.Join("", Enumerable.Range(0, new StackTrace().FrameCount).Select(i => " "));
        }

        protected void LogTrace(string message, [CallerMemberName] string caller = null)
        {
            log.Trace($"{LogPad()}{this.GetType().Name}.{caller}({Name}): {message}");
        }
        protected void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            log.Info($"{LogPad()}{this.GetType().Name}.{caller}({Name}): {message}");
        }
        protected void LogWarn(string message, [CallerMemberName] string caller = null)
        {
            log.Warn($"{LogPad()}{this.GetType().Name}.{caller}({Name}): {message}");
        }
        protected void LogError(string message, [CallerMemberName] string caller = null)
        {
            log.Error($"{LogPad()}{this.GetType().Name}.{caller}({Name}): {message}");
        }

    }
}
