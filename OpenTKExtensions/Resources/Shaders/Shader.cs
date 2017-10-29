using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    public class Shader : ResourceBase, IResource
    {
        public int Handle { get; private set; } = -1;
        public string Source { get; set; }
        public ShaderType Type { get; set; }

        public bool IsLoaded { get { return Handle > 0; } }

        public Shader(string name, ShaderType type, string source) : base(name)
        {
            Type = type;
            Source = source;
        }


        public void Load()
        {
            if (!IsLoaded)
            {
                Handle = GL.CreateShader(Type);

                if (Handle<=0)
                    throw new Exception($"Shader.Load ({Name}) could not get shader handle.");

                Compile();
            }

        }

        private void Compile()
        {
            if (!IsLoaded)
                throw new Exception($"Shader.Compile ({Name}) not loaded.");

            GL.ShaderSource(Handle, Source);
            GL.CompileShader(Handle);

            string infoLog = GL.GetShaderInfoLog(Handle).TrimEnd();

            int compileStatus;
            GL.GetShader(Handle, ShaderParameter.CompileStatus, out compileStatus);

            if (compileStatus != 1)
            {
                var ex = new ShaderCompileException(Name, infoLog, Source);
                LogError($"{ex.DetailedError}");
                throw ex;
            }
            else
            {
                LogTrace($"{infoLog}");
            }
        }


        public void Unload()
        {
            if (IsLoaded)
            {
                GL.DeleteShader(Handle);
                Handle = -1;
            }
        }

    }
}
