using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;

namespace OpenTKExtensions.Framework
{
    public class CompositeGameComponent : GameComponentBase, IResizeable, IReloadable, IUpdateable, IRenderable, IKeyboardControllable
    {
        protected GameComponentCollection components = new GameComponentCollection();
        public GameComponentCollection Components
        {
            get { return components; }
        }

        public bool Visible { get; set; } = true;
        public int DrawOrder { get; set; } = 0;

        public int KeyboardPriority { get; set; } = 0;

        public CompositeGameComponent()
            : base()
        {
            Loading += CompositeGameComponent_Loading;
            Unloading += CompositeGameComponent_Unloading;
        }

        private void CompositeGameComponent_Loading(object sender, EventArgs e)
        {
            Components.Load();
        }

        private void CompositeGameComponent_Unloading(object sender, EventArgs e)
        {
            Components.Unload();
        }

        public void Add(IGameComponent component)
        {
            Components.Add(component);
        }

        public void Remove(IGameComponent component)
        {
            Components.Remove(component);
        }

        public virtual void Resize(int width, int height)
        {
            Components.Resize(width, height);
        }

        public virtual void Reload()
        {
            Components.Reload();
        }

        public virtual void Update(IFrameUpdateData frameData)
        {
            Components.Update(frameData);
        }

        public virtual void Render(IFrameRenderData frameData)
        {
            Components.Render(frameData);
        }

        public virtual bool ProcessKeyDown(KeyboardKeyEventArgs e)
        {
            return Components.ProcessKeyDown(e);
        }

        public virtual bool ProcessKeyUp(KeyboardKeyEventArgs e)
        {
            return Components.ProcessKeyUp(e);
        }
    }
}
