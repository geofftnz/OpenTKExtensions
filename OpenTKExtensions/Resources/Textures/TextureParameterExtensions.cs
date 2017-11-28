using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{

    /// <summary>
    /// Generates texture parameters
    /// </summary>
    public static class TextureParameterExtensions
    {
      

        public static IEnumerable<ITextureParameter> FilterNearest(this IEnumerable<ITextureParameter> ps)
        {
            foreach (var p in ps)
                yield return p;

            yield return TextureParameter.Create(TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest);
            yield return TextureParameter.Create(TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest);
        }
        public static IEnumerable<ITextureParameter> FilterLinear(this IEnumerable<ITextureParameter> ps)
        {
            foreach (var p in ps)
                yield return p;

            yield return TextureParameter.Create(TextureParameterName.TextureMagFilter, TextureMagFilter.Linear);
            yield return TextureParameter.Create(TextureParameterName.TextureMinFilter, TextureMinFilter.Linear);
        }
        public static IEnumerable<ITextureParameter> FilterMipMaps(this IEnumerable<ITextureParameter> ps)
        {
            foreach (var p in ps)
                yield return p;

            yield return TextureParameter.Create(TextureParameterName.TextureMagFilter, TextureMagFilter.Linear);
            yield return TextureParameter.Create(TextureParameterName.TextureMinFilter, TextureMinFilter.LinearMipmapLinear);
        }

        public static IEnumerable<ITextureParameter> ClampToBorder(this IEnumerable<ITextureParameter> ps)
        {
            foreach (var p in ps)
                yield return p;

            yield return TextureParameter.Create(TextureParameterName.TextureWrapS, TextureWrapMode.ClampToBorder);
            yield return TextureParameter.Create(TextureParameterName.TextureWrapT, TextureWrapMode.ClampToBorder);
        }
        public static IEnumerable<ITextureParameter> ClampToEdge(this IEnumerable<ITextureParameter> ps)
        {
            foreach (var p in ps)
                yield return p;

            yield return TextureParameter.Create(TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge);
            yield return TextureParameter.Create(TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge);
        }
        public static IEnumerable<ITextureParameter> Repeat(this IEnumerable<ITextureParameter> ps)
        {
            foreach (var p in ps)
                yield return p;

            yield return TextureParameter.Create(TextureParameterName.TextureWrapS, TextureWrapMode.Repeat);
            yield return TextureParameter.Create(TextureParameterName.TextureWrapT, TextureWrapMode.Repeat);
        }
    }
}
