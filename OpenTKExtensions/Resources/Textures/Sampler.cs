using NLog;
using System;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    public class Sampler : ResourceBase, IResource
    {
        public uint ID { get; private set; } = 0;
        public bool IsLoaded { get { return ID != 0; } }

        public Dictionary<SamplerParameterName, ISamplerParameter> Parameters { get; } = new Dictionary<SamplerParameterName, ISamplerParameter>();

        public Sampler(string name) : base(name)
        {
        }

        public Sampler() : base("unnamed-sampler")
        {
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                ID = (uint)GL.GenSampler();
                LogTrace($"ID = {ID}");
            }
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                GL.DeleteSampler(ID);
            }
        }

        public void ApplyParameters()
        {
            if (IsLoaded)
            {
                foreach (var param in Parameters.Values)
                {
                    param.Apply(ID);
                }
            }
        }

        public Sampler SetParameter(ISamplerParameter param)
        {
            if (Parameters.ContainsKey(param.ParameterName))
                Parameters[param.ParameterName] = param;
            else
                Parameters.Add(param.ParameterName, param);

            return this;
        }

        public Sampler Set<T>(SamplerParameterName name, T value) where T : struct, IConvertible
        {
            SetParameter(SamplerParameter.Create<T>(name, value));
            return this;
        }

        public Sampler Bind(TextureUnit textureUnit)
        {
            Bind(textureUnit - TextureUnit.Texture0);
            return this;
        }

        public Sampler Bind(int textureUnit)
        {
            if (IsLoaded)
            {
                GL.BindSampler(textureUnit, (int)ID);
            }
            return this;
        }
        public static void Unbind(int textureUnit)
        {
            GL.BindSampler(textureUnit, 0);
        }
        public static void Unbind(TextureUnit textureUnit)
        {
            Unbind(textureUnit - TextureUnit.Texture0);
        }

    }
}
