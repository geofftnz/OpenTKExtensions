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
            get { return this.components; }
        }

        public bool Visible { get; set; } = true;
        public int DrawOrder { get; set; } = 0;

        public int KeyboardPriority { get; set; } = 0;

        public CompositeGameComponent()
            : base()
        {
            this.Loading += CompositeGameComponent_Loading;
            this.Unloading += CompositeGameComponent_Unloading;
        }

        private void CompositeGameComponent_Loading(object sender, EventArgs e)
        {
            this.Components.Load();
        }

        private void CompositeGameComponent_Unloading(object sender, EventArgs e)
        {
            this.Components.Unload();
        }

        public void Add(IGameComponent component)
        {
            this.Components.Add(component);
        }

        public void Remove(IGameComponent component)
        {
            this.Components.Remove(component);
        }

        public virtual void Resize(int width, int height)
        {
            this.Components.Resize(width, height);
        }

        public virtual void Reload()
        {
            this.Components.Reload();
        }

        public virtual void Update(IFrameUpdateData frameData)
        {
            this.Components.Update(frameData);
        }

        public virtual void Render(IFrameRenderData frameData)
        {
            this.Components.Render(frameData);
        }

        public virtual bool ProcessKeyDown(KeyboardKeyEventArgs e)
        {
            return this.Components.ProcessKeyDown(e);
        }

        public virtual bool ProcessKeyUp(KeyboardKeyEventArgs e)
        {
            return this.Components.ProcessKeyUp(e);
        }
    }
}
