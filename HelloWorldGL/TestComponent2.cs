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
    public class TestComponent2 : GameComponentBase, IGameComponent, IRenderable, ITransformable, IReloadable
    {
        public Matrix4 ViewMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ProjectionMatrix { get; set; } = Matrix4.Identity;
        public int DrawOrder { get; set; } = 0;
        public bool Visible { get; set; } = true;

        private ReloadableResource<ShaderProgram> shader;
        private BufferObject<Vector3> vertexBuffer;
        private BufferObject<uint> indexBuffer;

        public TestComponent2()
        {
            Resources.Add(shader = new ReloadableResource<ShaderProgram>("p2r", () => new ShaderProgram(
                 "p2",
                 "vertex",
                 "",
                 true,
                 "testshader2.glsl|vert",
                 "testshader2.glsl|frag"
                 ), (s) => new ShaderProgram(s)));

            Resources.Add(vertexBuffer = BufferObject<Vector3>.CreateVertexBuffer("vbuf", ScreenTri.Vertices().ToArray()));
            Resources.Add(indexBuffer = BufferObject<uint>.CreateIndexBuffer("ibuf", ScreenTri.Indices().ToArray()));

            ModelMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -0.1f);
            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 0.0f, 1.0f, 0.001f, 2.0f);


        }


        public void Render(IFrameRenderData frameData)
        {
            var fData = frameData as TestBench.RenderData;

            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            shader.Resource.Use()
                .SetUniform("projectionMatrix", ProjectionMatrix)
                .SetUniform("modelMatrix", ModelMatrix)
                .SetUniform("viewMatrix", ViewMatrix)
                .SetUniform("time", (float)(fData?.Time));

            vertexBuffer.Bind(shader.Resource.VariableLocations["vertex"]);
            indexBuffer.Bind();
            //GL.DrawArrays(PrimitiveType.Triangles, 0, indexBuffer.Length);
            GL.DrawElements(BeginMode.Triangles, indexBuffer.Length, DrawElementsType.UnsignedInt, 0);

        }

        public void Reload()
        {
            Resources.Reload();
        }
    }
}
