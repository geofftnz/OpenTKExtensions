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
    /// Base class for an "operator" component: one that renders a quad using a shader, almost always to a render target. 
    /// </summary>
    public class OperatorComponentBase : GameComponentBase, IGameComponent, IRenderable, IReloadable
    {
        public int DrawOrder { get; set; }
        public bool Visible { get; set; } = true;

        public Action TextureBinds { get; set; }
        public Action<ShaderProgram> SetShaderUniforms { get; set; }

        protected ReloadableResource<ShaderProgram> Shader;
        protected BufferObject<Vector3> VertexBuffer;
        protected BufferObject<uint> IndexBuffer;

        //protected const int MAXOUTPUTS = 16;
        //protected OperatorOutput[] outputs = new OperatorOutput[MAXOUTPUTS];


        public OperatorComponentBase(bool usingFilenames = true, params Tuple<ShaderType, string>[] shaderSourceOrFilenames)
        {
            Vector3[] vertex = {
                                    new Vector3(-1f,1f,0f),
                                    new Vector3(-1f,-1f,0f),
                                    new Vector3(1f,1f,0f),
                                    new Vector3(1f,-1f,0f)
                                };
            Resources.Add(VertexBuffer = vertex.ToBufferObject("vertex"));

            uint[] index = {
                                0,1,2,
                                1,3,2
                            };
            Resources.Add(IndexBuffer = index.ToBufferObject("index", BufferTarget.ElementArrayBuffer));

            if (shaderSourceOrFilenames != null && shaderSourceOrFilenames.Length > 0)
            {
                Shader = new ReloadableResource<ShaderProgram>("shader", () => new ShaderProgram("shader_internal", "vertex", "", usingFilenames, shaderSourceOrFilenames), (s) => new ShaderProgram(s));

                Resources.Add(Shader);
            }
        }

        public OperatorComponentBase(string vertexShader, string fragmentShader) : this(true, new Tuple<ShaderType, string>(ShaderType.VertexShader, vertexShader), new Tuple<ShaderType, string>(ShaderType.FragmentShader, fragmentShader))
        {
        }

        public void Render(IFrameRenderData frameData)
        {
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            TextureBinds?.Invoke();

            Shader.Resource.Use();
            SetShaderUniforms?.Invoke(Shader.Resource);

            VertexBuffer.Bind(Shader.Resource.VariableLocations["vertex"]);
            IndexBuffer.Bind();
            GL.DrawElements(BeginMode.Triangles, IndexBuffer.Length, DrawElementsType.UnsignedInt, 0);
        }


        public void Reload()
        {
            Resources.Reload();
        }
    }
}
