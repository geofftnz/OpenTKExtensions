using OpenTKExtensions.Framework;
using OpenTK;
using OpenTKExtensions.Image;
using OpenTKExtensions.Generators;
using System.Linq;
using OpenTKExtensions.Resources;
using OpenTK.Graphics.OpenGL4;

namespace HelloWorldGL
{
    public class TestComponent : GameComponentBase, IGameComponent, IRenderable, ITransformable
    {
        public Matrix4 ViewMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ProjectionMatrix { get; set; } = Matrix4.Identity;
        public int DrawOrder { get; set; } = 0;
        public bool Visible { get; set; } = true;


        public TestComponent()
        {
            Resources.Add("tex1", @"Resources/Textures/tex1.png".LoadImageToTextureRgba());
            Resources.Add(new OpenTKExtensions.Resources.ShaderProgram(
                "p1",
                "testshader.glsl|vert",
                "testshader.glsl|frag",
                "vertex"
                ));

            Resources.Add(OpenTKExtensions.Resources.Buffer<Vector3>.CreateVertexBuffer("vbuf", ScreenTri.Vertices().ToArray()));
            Resources.Add(OpenTKExtensions.Resources.Buffer<uint>.CreateIndexBuffer("ibuf", ScreenTri.Indices().ToArray()));

            ModelMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -0.1f);
            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 0.0f,1.0f, 0.001f, 2.0f);

            
        }


        public void Render(IFrameRenderData frameData)
        {
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            var shader = Resources.Get<ShaderProgram>("p1");
            var vbuf = Resources.Get<Buffer<Vector3>>("vbuf");
            var ibuf = Resources.Get<Buffer<uint>>("ibuf");


            shader.Use()
                .SetUniform("projectionMatrix", ProjectionMatrix)
                .SetUniform("modelMatrix", ModelMatrix)
                .SetUniform("viewMatrix", ViewMatrix);

            vbuf.Bind(shader.VariableLocations["vertex"]);
            ibuf.Bind();
            GL.DrawElements(BeginMode.Triangles, ibuf.Length, DrawElementsType.UnsignedInt, 0);

        }
    }
}
