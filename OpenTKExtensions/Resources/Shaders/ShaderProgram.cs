using NLog;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Loaders;
using System;
using System.Collections.Generic;

namespace OpenTKExtensions.Resources
{

    /// <summary>
    /// A collection of shaders that make up a pipeline.
    /// 
    /// Usually this will be vertex+fragment.
    /// 
    /// </summary>
    public class ShaderProgram : ResourceBase, IResource
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        public static IShaderLoader DefaultLoader { get; set; } = null;

        public int Handle { get; private set; } = 0;
        public string Log { get; private set; }

        public Dictionary<ShaderType, Shader> Shaders { get; } = new Dictionary<ShaderType, Shader>();
        public Dictionary<string, int> VariableLocations { get; } = new Dictionary<string, int>();
        public Dictionary<int, string> FragDataLocation { get; } = new Dictionary<int, string>();
        public Dictionary<string, int> UniformLocations { get; } = new Dictionary<string, int>();

        public bool IsLoaded { get { return Handle > 0; } }

        public ShaderProgram(string name, params Shader[] shaders) : base(name)
        {
            foreach (var shader in shaders)
                AddShader(shader);
        }

        public ShaderProgram(string name, string vertexSourceOrFilename, string fragmentSourceOrFilename, string variables, string fragmentOutputs = null) : base(name)
        {
            IShaderLoader loader = DefaultLoader;
            if (loader == null)
            {
                loader = new MemoryLoader();

                (loader as MemoryLoader).Add("vert", vertexSourceOrFilename);
                vertexSourceOrFilename = "vert";

                (loader as MemoryLoader).Add("frag", fragmentSourceOrFilename);
                fragmentSourceOrFilename = "frag";
            }

            AddShader(new Shader(name + "_vert", ShaderType.VertexShader, loader.Load(vertexSourceOrFilename).Content));
            AddShader(new Shader(name + "_frag", ShaderType.FragmentShader, loader.Load(fragmentSourceOrFilename).Content));
            SetVariables(variables);
            if (!string.IsNullOrWhiteSpace(fragmentOutputs))
                SetFragmentOutputs(fragmentOutputs);
        }

        // TODO: subclass that loads from files
        // TODO: clone & reload shaderprogram via IReloadableResource (returns a clone if successful)



        public void Load()
        {
            if (!IsLoaded)
            {
                Handle = GL.CreateProgram();
                if (Handle == 0)
                    throw new Exception($"ShaderProgram.Load ({Name}): Could not create program object.");

                foreach (var shader in Shaders.Values)
                {
                    shader.Load();
                }

                Link();
            }
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                foreach (var shader in Shaders.Values)
                {
                    shader.Unload();
                }
                GL.DeleteProgram(Handle);
                Handle = 0;
            }
        }

        private void AddShader(Shader shader)
        {
            if (shader == null)
                throw new ArgumentNullException("shader");

            if (Shaders.ContainsKey(shader.Type))
                throw new InvalidOperationException($"ShaderProgram.AddShader ({Name}): Program already contains a shader of type {shader.Type.ToString()}");

            Shaders.Add(shader.Type, shader);
        }

        private void Link()
        {
            if (!IsLoaded)
                throw new InvalidOperationException($"ShaderProgram.Link ({Name}): Program not loaded.");

            if (Shaders.Count < 1)
                throw new InvalidOperationException($"ShaderProgram.Link ({Name}): No shaders in program.");

            foreach (var s in Shaders.Values)
                GL.AttachShader(Handle, s.Handle);

            foreach (var v in VariableLocations)
                GL.BindAttribLocation(Handle, v.Value, v.Key);

            foreach (var i in FragDataLocation.Keys)
                GL.BindFragDataLocation(Handle, i, FragDataLocation[i]);

            GL.LinkProgram(Handle);

            string infoLog = GL.GetProgramInfoLog(Handle).TrimEnd();
            int linkStatus;
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out linkStatus);

            string formattedStatus = $"ShaderProgram.Link ({Name}): {infoLog}";
            if (linkStatus != 1)
            {
                throw new InvalidOperationException(formattedStatus);
            }
            else
            {
                log.Trace(formattedStatus);
            }


        }


        public ShaderProgram Use()
        {
            if (!IsLoaded)
                throw new InvalidOperationException($"ShaderProgram.Use ({Name}): Not loaded.");

            GL.UseProgram(Handle);
            return this;
        }
        public ShaderProgram UseProgram()
        {
            return Use();
        }


        public ShaderProgram AddVariable(int index, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (VariableLocations.ContainsKey(name))
                {
                    VariableLocations[name] = index;
                }
                else
                {
                    VariableLocations.Add(name, index);
                }
            }
            return this;
        }
        public ShaderProgram SetVariables(params string[] variables)
        {
            VariableLocations.Clear();

            // if we've only been passed a single string and it contains commas, then split it.
            if (variables.Length == 1 && variables[0].Contains(","))
                return SetVariables(variables[0].Split(','));

            for (int i = 0; i < variables.Length; i++)
                AddVariable(i, variables[i]);

            return this;
        }


        public ShaderProgram AddFragmentShaderOutput(int colourIndex, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (FragDataLocation.ContainsKey(colourIndex))
                {
                    FragDataLocation[colourIndex] = name;
                }
                else
                {
                    FragDataLocation.Add(colourIndex, name);
                }
            }
            return this;
        }
        public ShaderProgram SetFragmentOutputs(params string[] fragmentOutputs)
        {
            FragDataLocation.Clear();

            // if we've only been passed a single string and it contains commas, then split it.
            if (fragmentOutputs.Length == 1 && fragmentOutputs[0].Contains(","))
                return SetVariables(fragmentOutputs[0].Split(','));

            for (int i = 0; i < fragmentOutputs.Length; i++)
                AddFragmentShaderOutput(i, fragmentOutputs[i]);

            return this;
        }

        protected int LocateUniform(string uniformName, bool softFail = true)
        {
            int location = 0;
            if (UniformLocations.TryGetValue(uniformName, out location))
                return location;

            location = GL.GetUniformLocation(Handle, uniformName);
            if (location == -1)
                if (softFail)
                    log.Warn($"ShaderProgram.LocateUniform ({Name}): WARN: Could not locate {uniformName}");
                else
                    throw new InvalidOperationException($"ShaderProgram.LocateUniform ({Name}): Could not locate {uniformName}");

            UniformLocations.Add(uniformName, location);

            return location;
        }

        protected void SetUniformIfExists(string uniformName, Action<int> setFunc)
        {
            int location = LocateUniform(uniformName, true);
            if (location != -1)
                setFunc(location);
        }

        public ShaderProgram SetUniform(string uniformName, float value)
        {
            SetUniformIfExists(uniformName, (loc) => GL.Uniform1(loc, value));
            return this;
        }
        public ShaderProgram SetUniform(string uniformName, Vector2 value)
        {
            SetUniformIfExists(uniformName, (loc) => GL.Uniform2(loc, value));
            return this;
        }
        public ShaderProgram SetUniform(string uniformName, Vector3 value)
        {
            SetUniformIfExists(uniformName, (loc) => GL.Uniform3(loc, value));
            return this;
        }
        public ShaderProgram SetUniform(string uniformName, Vector4 value)
        {
            SetUniformIfExists(uniformName, (loc) => GL.Uniform4(loc, value));
            return this;
        }
        public ShaderProgram SetUniform(string uniformName, Matrix4 value)
        {
            SetUniformIfExists(uniformName, (loc) => GL.UniformMatrix4(loc, false, ref value));
            return this;
        }
        //TODO: More types


        // TODO: Do we need this?
        /*
        protected void BindFragDataLocation(int colourSlot, string outputName)
        {
            GL.BindFragDataLocation(Handle, colourSlot, outputName);
        }
        */

        public void BindVariable(string variableName, Action<int> bindAction)
        {
            int location;

            if (!VariableLocations.TryGetValue(variableName, out location))
                throw new Exception($"ShaderProgram.BindVariable ({Name}): Variable {variableName} not defined.");

            bindAction?.Invoke(location);
        }

    }
}
