using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    public class TextureSlotParam
    {
        public TextureTarget Target { get; set; }
        public PixelInternalFormat InternalFormat { get; set; }
        public PixelFormat Format { get; set; }
        public PixelType Type { get; set; }
        public List<ITextureParameter> TextureParameters { get; } = new List<ITextureParameter>();
        public bool MipMaps { get; set; }

        public TextureSlotParam()
            : this(TextureTarget.Texture2D, PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte, false, GetDefaultTextureParameters().ToArray())
        {
        }
        public TextureSlotParam(TextureTarget target, PixelInternalFormat internalFormat, PixelFormat format, PixelType type, bool mipmaps, params ITextureParameter[] texParams)
        {
            Target = target;
            InternalFormat = internalFormat;
            Format = format;
            Type = type;
            MipMaps = mipmaps;
            TextureParameters.AddRange(texParams);
        }

        public TextureSlotParam(PixelInternalFormat internalFormat, PixelFormat format, PixelType type)
            : this(TextureTarget.Texture2D, internalFormat, format, type, false, GetDefaultTextureParameters().ToArray())
        {
        }

        public override string ToString()
        {
            return $"[{InternalFormat},{Format},{Type}] params: {string.Join(",", TextureParameters.Select(p => p.ToString()))}";
        }

        public void ApplyParametersTo(Texture t)
        {
            foreach (var tp in TextureParameters)
            {
                t.SetParameter(tp);
            }
        }

        private static IEnumerable<ITextureParameter> GetDefaultTextureParameters()
        {
            yield return TextureParameter.Create(TextureParameterName.TextureMagFilter, TextureMagFilter.Linear);
            yield return TextureParameter.Create(TextureParameterName.TextureMinFilter, TextureMinFilter.Linear);
            yield return TextureParameter.Create(TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge);
            yield return TextureParameter.Create(TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge);
        }
    }
}
