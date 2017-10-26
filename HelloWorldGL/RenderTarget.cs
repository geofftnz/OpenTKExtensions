using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace HelloWorldGL
{
    public class RenderTarget : RenderTargetBase
    {
        public RenderTarget() : base(false, false, 512, 512)
        {
            SetOutput(0, new TextureSlotParam(PixelInternalFormat.Rgb32f, PixelFormat.Rgb, PixelType.Float));
        }
    }
}
