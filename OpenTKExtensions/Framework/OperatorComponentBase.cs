using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework
{

    /// <summary>
    /// Base class for an "operator" component: one that renders to one or more render targets. 
    /// </summary>
    public class OperatorComponentBase : GameComponentBase, IGameComponent, IRenderable, IResizeable
    {
        public int DrawOrder { get; set; }
        public bool Visible { get; set; } = true;

        public void Render(IFrameRenderData frameData)
        {
        }

        public void Resize(int width, int height)
        {
        }
    }
}
