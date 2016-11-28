using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;

namespace OpenTKExtensions.Framework
{
    public class ComponentSwitcher : CompositeGameComponent, IResizeable, IReloadable, IUpdateable, IRenderable, IKeyboardControllable
    {
        public bool SendKeypressesToInvisibleComponents { get; set; } = false;

        private int currentComponentIndex = -1;

        public int CurrentComponentIndex
        {
            get { return currentComponentIndex; }
            set
            {
                if (Components.Count > 0)
                {
                    currentComponentIndex = value;
                    if (currentComponentIndex < 0)
                        currentComponentIndex = 0;
                    else if (currentComponentIndex > Components.Count - 1)
                        currentComponentIndex = Components.Count - 1;
                }
                else
                {
                    currentComponentIndex = -1;
                }
                UpdateVisible();
            }
        }

        private void UpdateVisible()
        {
            for(int i = 0; i < Components.Count; i++)
            {
                var c = (Components[i] as IRenderable);
                if (c != null)
                {
                    c.Visible = (currentComponentIndex == i);
                }
            }
        }

        public IGameComponent CurrentComponent
        {
            get
            {
                return currentComponentIndex >= 0 ? this.Components[currentComponentIndex] : null;
            }
        }

        public ComponentSwitcher() : base()
        {

        }



        public override bool ProcessKeyDown(KeyboardKeyEventArgs e)
        {
            if (SendKeypressesToInvisibleComponents)
                return base.ProcessKeyDown(e);

            return (CurrentComponent as IKeyboardControllable)?.ProcessKeyDown(e) ?? false;
        }
        public override bool ProcessKeyUp(KeyboardKeyEventArgs e)
        {
            if (SendKeypressesToInvisibleComponents)
                return base.ProcessKeyUp(e);

            return (CurrentComponent as IKeyboardControllable)?.ProcessKeyUp(e) ?? false;
        }
    }
}
