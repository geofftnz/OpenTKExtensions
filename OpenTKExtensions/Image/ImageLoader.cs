using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKExtensions.Image
{
    public static class ImageLoader
    {

        public struct ImageInfo
        {
            public int Width;
            public int Height;
            public int Channels;
            public System.Drawing.Imaging.PixelFormat PixelFormat;
        }

        /// <summary>
        /// Loads an image from a file into a newly-created texture.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Resources.Texture LoadImageToTexture(this string fileName, TextureTarget target = TextureTarget.Texture2D, PixelInternalFormat internalFormat = PixelInternalFormat.Rgb, PixelFormat pixelFormat = PixelFormat.Rgb, PixelType pixelType = PixelType.UnsignedByte)
        {
            ImageInfo info;
            var imageData = fileName.LoadImage(out info);

            Resources.Texture tex = new Resources.Texture(fileName, info.Width, info.Height, target, internalFormat, pixelFormat, pixelType);
            tex
                .Set(TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear)
                .Set(TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear)
                .Set(TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge)
                .Set(TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            tex.ReadyForContent += (s, e) =>
            {
                tex.Upload(imageData);
            };

            return tex;
        }

        public static Resources.Texture LoadImageToTextureRgba(this string filename)
        {
            return filename.LoadImageToTexture(TextureTarget.Texture2D, PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte);
        }

        public static byte[] LoadImage(this string s)
        {
            ImageInfo info;
            return s.LoadImage(out info);
        }

        public static byte[] LoadImage(this string s, out ImageInfo info)
        {
            var image = (Bitmap)Bitmap.FromFile(s);

            info = new ImageInfo();
            info.Width = image.Width;
            info.Height = image.Height;
            info.PixelFormat = image.PixelFormat;
            info.Channels = 3;

            switch (image.PixelFormat)
            {
                //case System.Drawing.Imaging.PixelFormat.Alpha:
                //    break;
                case System.Drawing.Imaging.PixelFormat.Canonical:
                    info.Channels = 4;
                    return GetImageBytesRGBA32(image);
                /*
            case System.Drawing.Imaging.PixelFormat.DontCare:
                break;
            case System.Drawing.Imaging.PixelFormat.Extended:
                break;
            case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
                break;
            case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                break;
            case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                break;
            case System.Drawing.Imaging.PixelFormat.Format16bppRgb565:
                break;
            case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                break;*/
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return GetImageBytesRGB24(image);
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    info.Channels = 4;
                    return GetImageBytesRGBA32(image);
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    info.Channels = 4;
                    return GetImageBytesRGBA32(image);
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    info.Channels = 4;
                    return GetImageBytesRGBA32(image);
                /*case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format4bppIndexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Gdi:
                    break;
                case System.Drawing.Imaging.PixelFormat.Indexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Max:
                    break;
                case System.Drawing.Imaging.PixelFormat.PAlpha:
                    break;*/
                default:
                    throw new InvalidOperationException("Unsupported image format: " + image.PixelFormat);
            }


        }

        private static byte[] GetImageBytesRGB24(Bitmap image)
        {
            byte[] ret;
            var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);
            unsafe
            {
                int i = 0;
                ret = new byte[data.Width * data.Height * 4];

                for (int y = 0; y < data.Height; y++)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int j = 0;

                    for (int x = 0; x < data.Width; x++)
                    {
                        ret[i++] = row[j + 2];//R
                        ret[i++] = row[j + 1];//G
                        ret[i++] = row[j + 0];//B
                        ret[i++] = 255;//A

                        j += 3;
                    }
                }

            }
            image.UnlockBits(data);
            return ret;
        }

        private static byte[] GetImageBytesRGBA32(Bitmap image)
        {
            byte[] ret;
            var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);
            unsafe
            {
                int i = 0;
                ret = new byte[data.Width * data.Height * 4];

                for (int y = 0; y < data.Height; y++)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int j = 0;

                    for (int x = 0; x < data.Width; x++)
                    {
                        ret[i++] = row[j + 2];//R
                        ret[i++] = row[j + 1];//G
                        ret[i++] = row[j + 0];//B
                        ret[i++] = row[j + 3];//A

                        j += 4;
                    }
                }

            }
            image.UnlockBits(data);
            return ret;
        }

        public static byte[] ExtractChannelFromRGBA(this byte[] input, int channelIndex)
        {
            if (channelIndex < 0 || channelIndex > 3)
            {
                throw new InvalidOperationException("channelIndex must be between 0 and 3");
            }

            var len = input.Length;

            if (channelIndex >= len)
            {
                throw new InvalidOperationException("channelIndex is outside of source data range");
            }

            var b = new byte[len / 4];
            int edi = 0;
            int esi = channelIndex;

            for (int i = 0; i < len / 4; i++)
            {
                b[edi] = input[esi];
                edi++;
                esi += 4;
            }

            return b;
        }

    }
}
