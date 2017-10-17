using NLog;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    /// <summary>
    /// Wraps around 
    /// </summary>
    public class ReloadableShaderProgram : ResourceBase, IResource
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static IShaderLoader DefaultLoader { get; set; } = null;

        private Dictionary<ShaderType, string> ShaderFileNames { get; } = new Dictionary<ShaderType, string>();
        public string Variables { get; set; }


        private ShaderProgram shaderProgram = null;

        public ShaderProgram Shader
        {
            get
            {
                return shaderProgram;
            }
        }


        public ReloadableShaderProgram(string name, string variables, params Tuple<ShaderType, string>[] shaderFilenames) : base(name)
        {
            foreach (var s in shaderFilenames)
            {
                ShaderFileNames.Add(s.Item1, s.Item2);
            }
            Variables = variables;
        }

        public ReloadableShaderProgram(string name, string vertexFileName, string fragmentFileName, string variables)
            : this(
                 name,
                 variables,
                 new Tuple<ShaderType, string>(ShaderType.VertexShader, vertexFileName),
                 new Tuple<ShaderType, string>(ShaderType.FragmentShader, fragmentFileName)
                )
        {
        }

        public void Load()
        {
        }

        public void Unload()
        {
        }

        public void Reload()
        {
        }




    }
}
