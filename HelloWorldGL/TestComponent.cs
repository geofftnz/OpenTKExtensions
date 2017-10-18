using OpenTKExtensions.Framework;
using OpenTK;
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
        private OpenTKExtensions.Resources.VertexBuffer vbuf;
        private OpenTKExtensions.Resources.VertexBuffer ibuf;


        public TestComponent()
        {
            tex1 = @"Resources/Textures/tex1.png".LoadImageToTextureRgba();
            Resources.Add("tex1", tex1);

            prog = new OpenTKExtensions.Resources.ShaderProgram(
                "p1",
                "testshader.glsl|vert",
                "testshader.glsl|frag",
                "vertex"
                );
            Resources.Add(prog);

            vbuf = OpenTKExtensions.Resources.VertexBuffer.CreateVertexBuffer("vbuf");
            ibuf = OpenTKExtensions.Resources.VertexBuffer.CreateIndexBuffer("ibuf");

        }


        public void Render(IFrameRenderData frameData)
        {

        }
    }
}
