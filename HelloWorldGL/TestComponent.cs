using OpenTKExtensions.Framework;
using OpenTK;
using OpenTKExtensions.Image;
using OpenTKExtensions.Generators;
using System.Linq;
using OpenTKExtensions.Resources;
using OpenTK.Graphics.OpenGL4;
using System;

namespace HelloWorldGL
{
    public class TestComponent : GameComponentBase, IGameComponent, IRenderable, ITransformable, IReloadable,IResizeable
    {
        public Matrix4 ViewMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ProjectionMatrix { get; set; } = Matrix4.Identity;
        public int DrawOrder { get; set; } = 0;
        public bool Visible { get; set; } = true;
        public int Width { get; set; } = 256;
        public int Height { get; set; } = 256;

        private ReloadableResource<ShaderProgram> shader;
        private BufferObject<Vector3> vertexBuffer;
        private BufferObject<uint> indexBuffer;
        private Texture tex1;

        public Texture tex2 { get; set; }
        //public Func<Texture> GetTex2 { get; set; } = null;

        public TestComponent()
        {
            Resources.Add("tex1", tex1 = @"Resources/Textures/tex1.png".LoadImageToTextureRgba());

            Resources.Add(shader = new ReloadableResource<ShaderProgram>("p1r", () => new ShaderProgram(
                 "p1",
                 "vertex",
                 "",
                 true,
                 "testshader.glsl|vert",
                 "testshader.glsl|frag"
                 ), (s) => new ShaderProgram(s)));

            Resources.Add(vertexBuffer = BufferObject<Vector3>.CreateVertexBuffer("vbuf", ScreenTri.Vertices().ToArray()));
            Resources.Add(indexBuffer = BufferObject<uint>.CreateIndexBuffer("ibuf", ScreenTri.Indices().ToArray()));

            ModelMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -0.1f);
            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 0.0f, 1.0f, 0.001f, 2.0f);


        }


        public void Render(IFrameRenderData frameData)
        {
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            tex1.Bind(TextureUnit.Texture0);

            if (tex2 != null)
            {
                tex2.Bind(TextureUnit.Texture1);
            }
            else
            {
                LogWarn("Could not get tex2");
            }
            

            shader.Resource.Use()
                .SetUniform("projectionMatrix", ProjectionMatrix)
                .SetUniform("modelMatrix", ModelMatrix)
                .SetUniform("viewMatrix", ViewMatrix)
                .SetUniform("tex1", 0)
                .SetUniform("tex_new", 1)
                ;

            vertexBuffer.Bind(shader.Resource.VariableLocations["vertex"]);
            indexBuffer.Bind();

            //GL.DrawArrays(PrimitiveType.Triangles, 0, indexBuffer.Length);
            GL.DrawElements(BeginMode.Triangles, indexBuffer.Length, DrawElementsType.UnsignedInt, 0);

        }

        public void Reload()
        {
            Resources.Reload();
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
