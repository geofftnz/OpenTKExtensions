using OpenTKExtensions.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTKExtensions.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions;
using OpenTKExtensions.Image;

namespace HelloWorldGL
{
    public class TestComponent : GameComponentBase, IGameComponent, IRenderable, ITransformable
    {
        public Matrix4 ViewMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ProjectionMatrix { get; set; } = Matrix4.Identity;
        public int DrawOrder { get; set; } = 0;
        public bool Visible { get; set; } = true;

        private OpenTKExtensions.Resources.Texture tex1;
        private OpenTKExtensions.Resources.ShaderProgram prog;


        public TestComponent()
        {
            tex1 = @"Resources/Textures/tex1.png".LoadImageToTextureRgba();
            Resources.Add("tex1", tex1);

            //prog = new OpenTKExtensions.Resources.ShaderProgram();


        }


        public void Render(IFrameRenderData frameData)
        {

        }
    }
}
