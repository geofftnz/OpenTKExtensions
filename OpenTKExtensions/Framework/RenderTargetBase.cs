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
        public bool InheritSizeFromParent { get; set; } = false;

        public RenderTargetBase(bool wantDepth = false, bool inheritSize = false, int width = 256, int height = 256)
        {
            InheritSizeFromParent = inheritSize;
            Resources.Add(OutputBuffer = new GBuffer("gbuffer", wantDepth, width, height));
            OutputBuffer.Resized += OutputBuffer_Resized;
        }

        private void OutputBuffer_Resized(object sender, GBuffer.ResizedEventArgs e)
        {
            base.Resize(e.Width, e.Height);
        }

        public void SetOutput(int index, TextureSlotParam texparam)
        {
            OutputBuffer.SetSlot(index, texparam);
        }

        public void SetOutput(int index, Texture texture)
        {
            OutputBuffer.SetSlot(index, texture);
        }

        public Texture GetTexture(int slot)
        {
            return OutputBuffer.GetTextureAtSlot(slot);
        }

        public override void Render(IFrameRenderData frameData)
        {
            OutputBuffer.BindForWriting();

            Components.Render(frameData);

            OutputBuffer.UnbindFromWriting();
        }

        public override void Resize(int width, int height)
        {
            if (InheritSizeFromParent)
            {
                OutputBuffer?.Resize(width, height);
            }
        }
    }
}
