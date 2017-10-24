using NLog;
using OpenTK;
using OpenTKExtensions.Resources;
using OpenTK.Graphics.OpenGL4;
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
    public class RenderTargetBase : CompositeGameComponent, IGameComponent, IRenderable, IResizeable, IReloadable
    {
        protected GBuffer OutputBuffer;

        public RenderTargetBase(bool wantDepth = false, int width = 256, int height = 256)
        {
            Resources.Add(OutputBuffer = new GBuffer("gbuffer", wantDepth, width, height));
        }

        public void SetOutput(int index, TextureSlotParam texparam)
        {
            OutputBuffer.SetSlot(index, texparam);
        }

        public void SetOutput(int index, Texture texture)
        {
            OutputBuffer.SetSlot(index, texture);
        }

        public override void Render(IFrameRenderData frameData)
        {
            OutputBuffer.BindForWriting();

            Components.Render(frameData);

            OutputBuffer.UnbindFromWriting();
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);
            OutputBuffer?.Resize(width, height);
        }
    }
}
