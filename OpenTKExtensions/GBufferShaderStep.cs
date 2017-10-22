using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using NLog;
using OpenTKExtensions.Resources.Old;

namespace OpenTKExtensions
{
    /// <summary>
    /// GBufferShaderStep
    /// 
    /// Applies a shader, outputs to a GBuffer
    /// </summary>
    public class GBufferShaderStep
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public class TextureSlot
        {
            public string Name { get; set; }
            public Texture Texture { get; set; }
            public TextureTarget Target { get; set; }
            public TextureSlot()
            {
            }
            public TextureSlot(string name, Texture tex, TextureTarget target)
            {
                Name = name;
                Texture = tex;
                Target = target;
            }
            public TextureSlot(string name, Texture tex)
                : this(name, tex, TextureTarget.Texture2D)
            {

            }
        }

        public string Name { get; private set; }

        // Needs:
        // Quad vertex VBO
        protected VBO vertexVBO;
        // Quad index VBO
        protected VBO indexVBO;
        // GBuffer to encapsulate our output texture.
        protected GBuffer gbuffer;
        // Shader
        private string vsSource = "";
        private string fsSource = "";
        protected ShaderProgram program;

        // texture slots
        protected const int MAXSLOTS = 16;
        protected TextureSlot[] textureSlot = new TextureSlot[MAXSLOTS];



        public GBufferShaderStep()
            : this("gbstep")
        {

        }
        public GBufferShaderStep(string name)
        {
            Name = name;
            vertexVBO = new VBO(Name + "_v");
            indexVBO = new VBO(Name + "_i", BufferTarget.ElementArrayBuffer);
            gbuffer = new GBuffer(Name + "_gb", false);
            program = new ShaderProgram(Name + "_sp");
        }

        public void SetOutputTexture(int slot, string name, Texture tex, TextureTarget target)
        {
            if (slot < 0 || slot >= MAXSLOTS)
            {
                throw new ArgumentOutOfRangeException("slot");
            }
            textureSlot[slot] = new TextureSlot(name, tex, target);
        }

        public void SetOutputTexture(int slot, string name, Texture tex)
        {
            SetOutputTexture(slot, name, tex, TextureTarget.Texture2D);
        }

        public void ClearOutputTexture(int slot)
        {
            if (slot < 0 || slot >= MAXSLOTS)
            {
                throw new ArgumentOutOfRangeException("slot");
            }
            textureSlot[slot] = null;
        }

        public void Init(string vertexShaderSource, string fragmentShaderSource)
        {
            InitVBOs();
            InitGBuffer();
            InitShader(vertexShaderSource, fragmentShaderSource);

        }

        public virtual void Render(Action textureBinds, Action<ShaderProgram> setUniforms)
        {
            Render(textureBinds, setUniforms, () =>
            {
                vertexVBO.Bind(program.VariableLocation("vertex"));
                indexVBO.Bind();
                GL.DrawElements(BeginMode.Triangles, indexVBO.Length, DrawElementsType.UnsignedInt, 0);
            });
        }

        public virtual void Render(Action textureBinds, Action<ShaderProgram> setUniforms, Action renderAction)
        {
            // start gbuffer
            gbuffer.BindForWriting();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            textureBinds?.Invoke();

            program.UseProgram();

            setUniforms?.Invoke(program);

            renderAction?.Invoke();

            gbuffer.UnbindFromWriting();
        }


        private void InitShader(string vertexShaderSource, string fragmentShaderSource)
        {
            vsSource = vertexShaderSource;
            fsSource = fragmentShaderSource;

            if (!ReloadShader())
                throw new InvalidOperationException("Could not load shader");

        }

        private ShaderProgram LoadShaderProgram()
        {
            var program = new ShaderProgram(Name);

            // setup shader
            program.Init(
                vsSource,
                fsSource,
                new List<Variable> 
                { 
                    new Variable(0, "vertex")
                },
                textureSlot.Where(ts => ts != null).Select(ts => ts.Name).ToArray()
                );

            return program;
        }

        public bool ReloadShader()
        {
            try
            {
                ShaderProgram p = LoadShaderProgram();

                if (p == null)
                {
                    throw new InvalidOperationException("ReloadShader() returned null, but didn't throw an exception");
                }

                if (program != null)
                {
                    program.Unload();
                }
                program = p;
                return true;
            }
            catch (Exception ex)
            {
                if (log != null)
                {
                    log.Warn("Could not reload shader program {0}: {1}", Name, ex.GetType().Name + ": " + ex.Message);
                }
            }
            return false;

        }

        protected virtual void InitGBuffer()
        {
            if (!textureSlot.Any(ts => ts != null && ts.Texture != null))
            {
                throw new InvalidOperationException("No texture slots filled");
            }

            // find first texture slot, set width and height
            int width = textureSlot.Where(ts => ts != null && ts.Texture != null).FirstOrDefault().Texture.Width;
            int height = textureSlot.Where(ts => ts != null && ts.Texture != null).FirstOrDefault().Texture.Height;

            //gbuffer.SetSlot(0, outputTexture);
            for (int slot = 0; slot < MAXSLOTS; slot++)
            {
                if (textureSlot[slot] != null)
                {
                    if (textureSlot[slot].Texture != null)
                    {
                        gbuffer.SetSlot(slot, textureSlot[slot].Texture);
                    }
                }

            }

            gbuffer.Init(width, height);
        }
        private void InitVBOs()
        {
            Vector3[] vertex = {
                                    new Vector3(-1f,-1f,0f),
                                    new Vector3(3f,-1f,0f),
                                    new Vector3(-1f,3f,0f)
                                };
            uint[] index = {
                                0,1,2
                            };

            vertexVBO.SetData(vertex);
            indexVBO.SetData(index);
        }

        private void InitVBOsq()
        {
            Vector3[] vertex = {
                                    new Vector3(-1f,1f,0f),
                                    new Vector3(-1f,-1f,0f),
                                    new Vector3(1f,1f,0f),
                                    new Vector3(1f,-1f,0f)
                                };
            uint[] index = {
                                0,1,2,
                                1,3,2
                            };

            vertexVBO.SetData(vertex);
            indexVBO.SetData(index);
        }

        public void ClearColourBuffer(int drawBuffer, Vector4 colour)
        {
            gbuffer.ClearColourBuffer(drawBuffer, colour);
        }

        public int ShaderVariableLocation(string name)
        {
            return program.VariableLocation(name);
        }

    }
}
