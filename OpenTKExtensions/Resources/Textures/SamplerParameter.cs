using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    public class SamplerParameter<T> : ISamplerParameter where T : struct, IConvertible
    {
        public SamplerParameterName ParameterName { get; set; }
        public T Value { get; set; }

        public SamplerParameter(SamplerParameterName name, T value)
        {
            ParameterName = name;
            Value = value;
        }

        public void Apply(uint samplerID)
        {
            // HACK this is nasty.
            if (Value is Enum)
            {
                GL.SamplerParameter(samplerID, ParameterName, Convert.ToInt32(Value));
                return;
            }
            if (Value is int)
            {
                GL.SamplerParameter(samplerID, ParameterName, Convert.ToInt32(Value));
                return;
            }
            if (Value is float)
            {
                GL.SamplerParameter(samplerID, ParameterName, Convert.ToSingle(Value));
                return;
            }
        }

    }

    public static class SamplerParameter
    {
        public static SamplerParameter<U> Create<U>(SamplerParameterName name, U value) where U : struct, IConvertible
        {
            return new SamplerParameter<U>(name, value);
        }
    }
}
