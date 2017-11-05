using NLog;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
//using OpenTKExtensions.Resources.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace OpenTKExtensions.Resources
{
    public class Texture : ResourceBase, IResource
    {
        public int ID { get; protected set; } = -1;
        public TextureTarget Target { get; private set; }
        public PixelInternalFormat InternalFormat { get; private set; }
        public PixelFormat Format { get; set; }
        public PixelType Type { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsLoaded { get { return ID != -1; } }
        public bool LoadEmpty { get; set; } = false;

        public Dictionary<TextureParameterName, ITextureParameter> Parameters { get; } = new Dictionary<TextureParameterName, ITextureParameter>();

        public Texture(string name, int width, int height, TextureTarget target, PixelInternalFormat internalFormat, PixelFormat format, PixelType type, params ITextureParameter[] texParams) : base(name)
        {
            Target = target;
            InternalFormat = internalFormat;
            Width = width;
            Height = height;
            Format = format;
            Type = type;

            foreach (var p in texParams)
            {
                SetParameter(p);
            }
        }

        public Texture(int width, int height, TextureTarget target, PixelInternalFormat internalFormat, PixelFormat format, PixelType type)
            : this("unnamed", width, height, target, internalFormat, format, type)
        {
        }

        // static helpers
        #region construction helpers
        public static Texture RGBA32f(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, texParams);
        }
        public static Texture RGBA16f(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f, PixelFormat.Rgba, PixelType.HalfFloat, texParams);
        }
        public static Texture RGBA(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte, texParams);
        }
        public static Texture RGB24f(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.Rgb32f, PixelFormat.Rgb, PixelType.Float, texParams);
        }
        public static Texture RGB12f(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.HalfFloat, texParams);
        }
        public static Texture RGB(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte, texParams);
        }
        public static Texture RG32f(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.Rg32f, PixelFormat.Rg, PixelType.Float, texParams);
        }
        public static Texture R32f(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.R32f, PixelFormat.Red, PixelType.Float, texParams);
        }
        public static Texture RG(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.Rg8, PixelFormat.Rg, PixelType.UnsignedByte, texParams);
        }
        public static Texture R(string name, int width, int height, params ITextureParameter[] texParams)
        {
            return new Texture(name, width, height, TextureTarget.Texture2D, PixelInternalFormat.R8, PixelFormat.Red, PixelType.UnsignedByte, texParams);
        }
        #endregion


        public override string ToString()
        {
            return $"{Target} {ID} {Width}x{Height} [{InternalFormat},{Format},{Type}] params: {string.Join(",", Parameters.Select(p => $"{p.Key}={p.Value}"))}";
        }


        public void Load()
        {
            if (!IsLoaded)
            {
                ID = GL.GenTexture();

                // The following errors out
                //if (!GL.IsTexture(ID))
                //  throw new Exception($"Texture.Load ({Name}) generated texture ID {ID} is not a texture");

                LogTrace($"ID = {ID}");

                if (LoadEmpty)
                    UploadEmpty();
                else
                    OnReadyForContent();
            }
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                GL.DeleteTexture(ID);
                LogTrace($"ID {ID} deleted");
                ID = -1;
                OnUnloaded();
            }
        }

        public Texture SetParameter(ITextureParameter param)
        {
            if (Parameters.ContainsKey(param.ParameterName))
            {
                Parameters[param.ParameterName] = param;
            }
            else
            {
                Parameters.Add(param.ParameterName, param);
            }
            return this;
        }

        public Texture Set<T>(TextureParameterName parameterName, T value) where T : struct, IConvertible
        {
            return SetParameter(new TextureParameter<T>(parameterName, value));
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }


        public void ApplyParameters()
        {
            foreach (var param in Parameters.Values)
            {
                param.Apply(Target);
            }
        }

        private void EnsureLoaded([CallerMemberName] string caller = null)
        {
            if (!IsLoaded)
                throw new InvalidOperationException($"Texture.{caller}({Name}): Not loaded");
        }

        public Texture Bind()
        {
            EnsureLoaded();
            GL.BindTexture(Target, ID);
            return this;
        }

        public Texture Bind(TextureUnit unit)
        {
            EnsureLoaded();
            GL.ActiveTexture(unit);
            Bind();
            return this;
        }

        public void Upload<T>(T[] data) where T : struct
        {
            Bind();
            ApplyParameters();
            UploadImage(data);
        }

        public void Upload<T>(T[] data, int level) where T : struct
        {
            Bind();
            ApplyParameters();
            UploadImage(data, level);
        }

        public void UploadZero<T>(int channels) where T : struct
        {
            int length = Width * Height * channels;
            var data = new T[length];
            Upload(data);
        }

        public void UploadEmpty()
        {
            LogTrace("");
            Bind();
            ApplyParameters();
            GL.TexImage2D(Target, 0, InternalFormat, Width, Height, 0, Format, Type, IntPtr.Zero);
        }

        public void UploadEmpty(TextureTarget target)
        {
            Bind();
            ApplyParameters();
            GL.TexImage2D(target, 0, InternalFormat, Width, Height, 0, Format, Type, IntPtr.Zero);
        }

        private void LogUploading(string message = "uploading...", [CallerMemberName]string caller = null)
        {
            LogTrace(message, caller);
        }
        private void LogDataUploaded<T>(T[] data, [CallerMemberName]string caller = null)
        {
            LogTrace($"uploaded {data.Length} texels of {data.GetType().Name}", caller);
        }

        public void UploadImage<T>(TextureTarget target, T[] data) where T : struct
        {
            LogUploading($"uploading to target {target}...");
            GL.TexImage2D<T>(target, 0, InternalFormat, Width, Height, 0, Format, Type, data);
            LogDataUploaded(data);
        }

        public void UploadImage<T>(T[] data) where T : struct
        {
            LogUploading();
            GL.TexImage2D<T>(Target, 0, InternalFormat, Width, Height, 0, Format, Type, data);
            LogDataUploaded(data);
        }

        public void UploadImage<T>(T[] data, int level) where T : struct
        {
            LogUploading();
            GL.TexImage2D<T>(Target, level, InternalFormat, Width >> level, Height >> level, 0, Format, Type, data);
            LogDataUploaded(data);
        }

        public void RefreshImage<T>(T[] data) where T : struct
        {
            LogUploading();
            Bind();
            GL.TexSubImage2D<T>(Target, 0, 0, 0, Width, Height, Format, Type, data);
            LogDataUploaded(data);
        }

        public void RefreshImage<T>(T[] data, int level) where T : struct
        {
            LogUploading();
            Bind();
            GL.TexSubImage2D<T>(Target, level, 0, 0, Width, Height, Format, Type, data);
            LogDataUploaded(data);
        }

        public void RefreshImage<T>(T[] data, int xoffset, int yoffset, int width, int height) where T : struct
        {
            LogUploading();
            Bind();
            GL.TexSubImage2D<T>(Target, 0, xoffset, yoffset, width, height, Format, Type, data);
            LogDataUploaded(data);
        }

        public void RefreshImage<T>(BufferObject<T> buffer)
        {
            LogUploading("uploading from buffer...");
            buffer.Bind();
            Bind();
            GL.TexSubImage2D(Target, 0, 0, 0, Width, Height, Format, Type, (IntPtr)IntPtr.Zero);
            LogTrace($"uploaded");
        }

        public int GetLevelWidth(int level)
        {
            return GetTextureParameterInt(level, GetTextureParameter.TextureWidth);
        }
        public int GetLevelHeight(int level)
        {
            return GetTextureParameterInt(level, GetTextureParameter.TextureHeight);
        }

        private int GetTextureParameterInt(int level, GetTextureParameter p)
        {
            int n;
            GL.GetTexLevelParameter(Target, level, p, out n);
            return n;
        }

        public float[] GetLevelDataFloat(int level)
        {
            Bind();

            int width = GetLevelWidth(level);
            int height = GetLevelHeight(level);

            float[] data = new float[width * height * 4];

            GL.GetTexImage(Target, level, PixelFormat.Rgba, PixelType.Float, data);

            return data;
        }

        public Vector4[] GetLevelDataVector4(int level)
        {
            Bind();

            int width = GetLevelWidth(level);
            int height = GetLevelHeight(level);

            Vector4[] data = new Vector4[width * height];

            GL.GetTexImage(Target, level, PixelFormat.Rgba, PixelType.Float, data);

            return data;
        }
    }
}
