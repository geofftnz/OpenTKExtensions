using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using NLog;
using OpenTKExtensions.Exceptions;

namespace OpenTKExtensions
{
    public class Shader
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public int Handle { get; private set; } = -1;
        public string Source { get; set; }
        public ShaderType Type { get; set; }

        public string Name { get; set; }


        public Shader(string name, ShaderType type, string source)
        {
            this.Type = type;
            this.Source = source;
            this.Name = name;
        }

        public Shader(string name)
            : this(name, ShaderType.FragmentShader, string.Empty)
        {
        }

        public Shader()
            : this("unnamed")
        {
        }

        public void Load()
        {
            Init();
        }

        public int Init()
        {
            if (this.Handle == -1)
            {
                this.Handle = GL.CreateShader(this.Type);
            }

            if (this.Handle != -1 && !string.IsNullOrEmpty(Source))
                Compile();

            return this.Handle;
        }

        public void Compile()
        {
            if (this.Handle == -1)
                Init();


            GL.ShaderSource(this.Handle, this.Source);
            GL.CompileShader(this.Handle);

            string infoLog = GL.GetShaderInfoLog(this.Handle).TrimEnd();

            int compileStatus;
            GL.GetShader(this.Handle, ShaderParameter.CompileStatus, out compileStatus);

            if (compileStatus != 1)
            {
                var ex = new ShaderCompileException(this.Name, infoLog, this.Source);
                log.Error("Shader.Compile ({0}): {1}", this.Name, ex.DetailedError);
                throw ex;
            }
            else
            {
                log.Trace("Shader.Compile ({0}): {1}", this.Name, infoLog);
            }
        }


        public void Unload()
        {
            if (this.Handle != -1)
            {
                GL.DeleteShader(this.Handle);
                this.Handle = -1;
            }
        }

    }
}
