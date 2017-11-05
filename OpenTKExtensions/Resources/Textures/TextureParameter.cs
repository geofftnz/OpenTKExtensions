using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    public class TextureParameter<T> : ITextureParameter where T : struct, IConvertible
    {
        public TextureParameterName ParameterName { get; set; }
        public T Value { get; set; }

        public TextureParameter(TextureParameterName name, T value)
        {
            ParameterName = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{ParameterName}={Value}";
        }

        public void Apply(TextureTarget target)
        {
            // HACK this is nasty.
            if (Value is Enum)
            {
                GL.TexParameter(target, ParameterName, Convert.ToInt32(Value));
                return;
            }
            if (Value is int)
            {
                GL.TexParameter(target, ParameterName, Convert.ToInt32(Value));
                return;
            }
            if (Value is float)
            {
                GL.TexParameter(target, ParameterName, Convert.ToSingle(Value));
                return;
            }
        }

    }

    public static class TextureParameter
    {
        public static TextureParameter<U> Create<U>(TextureParameterName name, U value) where U : struct, IConvertible
        {
            return new TextureParameter<U>(name, value);
        }

        public static ITextureParameter SetTo<T>(this TextureParameterName name, T value) where T : struct, IConvertible
        {
            return new TextureParameter<T>(name, value);
        }
    }
}
