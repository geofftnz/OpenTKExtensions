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
    public class OperatorComponentBase : GameComponentBase, IGameComponent, IRenderable, IResizeable, IReloadable
    {
        public int DrawOrder { get; set; }
        public bool Visible { get; set; } = true;

        public Action TextureBinds { get; set; }
        public Action<ShaderProgram> SetShaderUniforms { get; set; }

        protected ReloadableResource<ShaderProgram> Shader;
        protected GBuffer OutputBuffer;
        protected BufferObject<Vector3> VertexBuffer;
        protected BufferObject<uint> IndexBuffer;

        //protected const int MAXOUTPUTS = 16;
        //protected OperatorOutput[] outputs = new OperatorOutput[MAXOUTPUTS];


        public OperatorComponentBase(bool wantDepth = false, int width = 256, int height = 256, bool usingFilenames = true, params Tuple<ShaderType, string>[] shaderSourceOrFilenames)
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

            Resources.Add(OutputBuffer = new GBuffer("gbuffer", wantDepth, width, height));

            if (shaderSourceOrFilenames != null && shaderSourceOrFilenames.Length > 0)
            {
                Shader = new ReloadableResource<ShaderProgram>("shader", () => new ShaderProgram("shader_internal", "vertex", "", usingFilenames, shaderSourceOrFilenames), (s) => new ShaderProgram(s));

                Resources.Add(Shader);
            }

        }

        public void SetOutput(int index, TextureSlotParam texparam)
        {
            OutputBuffer.SetSlot(index, texparam);
        }

        public void SetOutput(int index, Texture texture)
        {
            OutputBuffer.SetSlot(index, texture);
        }

        public void SetShaderProgram(bool usingFilenames, params Tuple<ShaderType, string>[] shaderSourceOrFilenames)
        {
            bool wasLoaded = false;

            if (Resources.ContainsKey("shader"))
            {
                Resources.Remove("shader");
                wasLoaded = Shader?.Resource?.IsLoaded ?? false; 
                Shader.Unload();
                Shader = null;
            }

            if (shaderSourceOrFilenames != null && shaderSourceOrFilenames.Length > 0)
            {
                Shader = new ReloadableResource<ShaderProgram>("shader", () => new ShaderProgram("shader_internal", "vertex", "", usingFilenames, shaderSourceOrFilenames), (s) => new ShaderProgram(s));

                Resources.Add(Shader);
                if (wasLoaded)
                    Shader.Load();
            }
        }



        public void Render(IFrameRenderData frameData)
        {
            OutputBuffer.BindForWriting();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            TextureBinds?.Invoke();

            Shader.Resource.Use();
            SetShaderUniforms?.Invoke(Shader.Resource);

            VertexBuffer.Bind(Shader.Resource.VariableLocations["vertex"]);
            IndexBuffer.Bind();
            GL.DrawElements(BeginMode.Triangles, IndexBuffer.Length, DrawElementsType.UnsignedInt, 0);

            OutputBuffer.UnbindFromWriting();
        }

        public void Resize(int width, int height)
        {
            OutputBuffer?.Resize(width, height);
        }

        public void Reload()
        {
            string message;
            Shader?.TryReload(out message);
        }
    }
}
