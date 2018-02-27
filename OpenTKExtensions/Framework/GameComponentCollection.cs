using NLog;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace OpenTKExtensions.Framework
{
    public class GameComponentCollection : List<IGameComponent>, ICollection<IGameComponent>
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public GameComponentCollection()
        {

        }

        public void Load()
        {
            foreach (var component in this.OrderBy(c => c.LoadOrder))
            {
                component.Load();
            }
        }

        public void Unload()
        {
            foreach (var component in this.OrderByDescending(c => c.LoadOrder))
            {
                component.Unload();
            }
        }

        public void Do<T>(Action<T> action) where T : class
        {
            foreach (var component in this.SelectMany(c => (c as T).Enum()))
            {
                action(component);
            }
        }


        public void Render<F>(F frameData) where F : IFrameRenderData
        {
            foreach (var component in this.OfType<IRenderable>().Where(c => c.Visible).OrderBy(c => c.DrawOrder))
            {
                //LogTrace($"PreRender: {component.GetType().Name}");
                component.OnPreRender();

                //LogTrace($"Render: {component.GetType().Name}");
                (component as ITimedComponent)?.StartRenderTimer();
                component.Render(frameData);
                (component as ITimedComponent)?.StopRenderTimer();
            }
        }

        public void Update<F>(F frameData) where F : IFrameUpdateData
        {
            Do<IUpdateable>(c => c.Update(frameData));
        }

        public void Reload()
        {
            Do<IReloadable>(c => c.Reload());
        }

        public void Resize(int width, int height)
        {
            Do<IResizeable>(c => c.Resize(width, height));
        }


        public void Add(IGameComponent component, int loadOrder)
        {
            component.LoadOrder = loadOrder;
            Add(component);
        }

        public bool ProcessKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            foreach (var component in this.OfType<IKeyboardControllable>().OrderBy(c => c.KeyboardPriority))
            {
                if (component.ProcessKeyDown(e))
                    return true;
            }
            return false;
        }

        public bool ProcessKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            foreach (var component in this.OfType<IKeyboardControllable>().OrderBy(c => c.KeyboardPriority))
            {
                if (component.ProcessKeyUp(e))
                    return true;
            }
            return false;
        }

        public Matrix4 ProjectionMatrix
        {
            set
            {
                Do<ITransformable>(c => c.ProjectionMatrix = value);
            }
        }

        public Matrix4 ViewMatrix
        {
            set
            {
                Do<ITransformable>(c => c.ViewMatrix = value);
            }
        }

        public Matrix4 ModelMatrix
        {
            set
            {
                Do<ITransformable>(c => c.ModelMatrix = value);
            }
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
