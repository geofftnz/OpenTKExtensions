using NLog;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Resources.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace OpenTKExtensions.Resources
{
    public class Texture : ResourceBase, IResource
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public int ID { get; protected set; } = -1;
        public TextureTarget Target { get; private set; }
        public PixelInternalFormat InternalFormat { get; private set; }
        public PixelFormat Format { get; set; }
        public PixelType Type { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsLoaded { get { return ID != -1; } }

        public Dictionary<TextureParameterName, ITextureParameter> Parameters { get; } = new Dictionary<TextureParameterName, ITextureParameter>();

        public Texture(string name, int width, int height, TextureTarget target, PixelInternalFormat internalFormat, PixelFormat format, PixelType type)
        {
            this.Name = name;
            this.Target = target;
            this.InternalFormat = internalFormat;
            this.Width = width;
            this.Height = height;
            this.Format = format;
            this.Type = type;
        }

        public Texture(int width, int height, TextureTarget target, PixelInternalFormat internalFormat, PixelFormat format, PixelType type)
            : this("unnamed", width, height, target, internalFormat, format, type)
        {
        }


        public void Load()
        {
            if (!IsLoaded)
            {
                ID = GL.GenTexture();

                // The following errors out
                //if (!GL.IsTexture(ID))
                  //  throw new Exception($"Texture.Load ({Name}) generated texture ID {ID} is not a texture");

                log.Trace($"Texture.Load ({Name}) generated texture ID {ID}");
                OnReadyForContent();
            }
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                GL.DeleteTexture(ID);
                log.Trace($"Texture.Unload ({Name}) deleted texture ID {ID}");
                ID = -1;
                OnUnloaded();
            }
        }

        public Texture SetParameter(ITextureParameter param)
        {
            if (this.Parameters.ContainsKey(param.ParameterName))
            {
                this.Parameters[param.ParameterName] = param;
            }
            else
            {
                this.Parameters.Add(param.ParameterName, param);
            }
            return this;
        }

        public Texture Set(TextureParameterName parameterName, int value)
        {
            return SetParameter(new TextureParameterInt(parameterName, value));
        }
        public Texture Set(TextureParameterName parameterName, float value)
        {
            return SetParameter(new TextureParameterFloat(parameterName, value));
        }

        public void Resize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
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
            GL.BindTexture(this.Target, this.ID);
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
            int length = this.Width * this.Height * channels;
            var data = new T[length];
            Upload(data);
        }

        public void UploadEmpty()
        {
            Bind();
            ApplyParameters();
            GL.TexImage2D(this.Target, 0, this.InternalFormat, this.Width, this.Height, 0, this.Format, this.Type, IntPtr.Zero);
        }

        public void UploadEmpty(TextureTarget target)
        {
            Bind();
            ApplyParameters();
            GL.TexImage2D(target, 0, this.InternalFormat, this.Width, this.Height, 0, this.Format, this.Type, IntPtr.Zero);
        }

        public void UploadImage<T>(TextureTarget target, T[] data) where T : struct
        {
            log.Trace("Texture.UploadImage ({0}) uploading to target {1}...", this.Name, target.ToString());
            GL.TexImage2D<T>(target, 0, this.InternalFormat, this.Width, this.Height, 0, this.Format, this.Type, data);
            log.Trace("Texture.UploadImage ({0}) uploaded {1} texels of {2}", this.Name, data.Length, data.GetType().Name);
        }

        public void UploadImage<T>(T[] data) where T : struct
        {
            log.Trace("Texture.UploadImage ({0}) uploading...", this.Name);
            GL.TexImage2D<T>(this.Target, 0, this.InternalFormat, this.Width, this.Height, 0, this.Format, this.Type, data);
            log.Trace("Texture.UploadImage ({0}) uploaded {1} texels of {2}", this.Name, data.Length, data.GetType().Name);
        }

        public void UploadImage<T>(T[] data, int level) where T : struct
        {
            log.Trace("Texture.UploadImage ({0}) uploading...", this.Name);
            GL.TexImage2D<T>(this.Target, level, this.InternalFormat, this.Width >> level, this.Height >> level, 0, this.Format, this.Type, data);
            log.Trace("Texture.UploadImage ({0}) uploaded {1} texels of {2}", this.Name, data.Length, data.GetType().Name);
        }

        public void RefreshImage<T>(T[] data) where T : struct
        {
            log.Trace("Texture.RefreshImage ({0}) uploading...", this.Name);
            this.Bind();
            GL.TexSubImage2D<T>(this.Target, 0, 0, 0, this.Width, this.Height, this.Format, this.Type, data);
            log.Trace("Texture.RefreshImage ({0}) uploaded {1} texels of {2}", this.Name, data.Length, data.GetType().Name);
        }

        public void RefreshImage<T>(T[] data, int level) where T : struct
        {
            log.Trace("Texture.RefreshImage ({0}) uploading...", this.Name);
            this.Bind();
            GL.TexSubImage2D<T>(this.Target, level, 0, 0, this.Width, this.Height, this.Format, this.Type, data);
            log.Trace("Texture.RefreshImage ({0}) uploaded {1} texels of {2}", this.Name, data.Length, data.GetType().Name);
        }

        public void RefreshImage<T>(T[] data, int xoffset, int yoffset, int width, int height) where T : struct
        {
            log.Trace("Texture.RefreshImage ({0}) uploading...", this.Name);
            this.Bind();
            GL.TexSubImage2D<T>(this.Target, 0, xoffset, yoffset, width, height, this.Format, this.Type, data);
            log.Trace("Texture.RefreshImage ({0}) uploaded {1} texels of {2}", this.Name, data.Length, data.GetType().Name);
        }

        public void RefreshImage<T>(Buffer<T> buffer)
        {
            log.Trace("Texture.RefreshImage ({0}) uploading from buffer...", this.Name);
            buffer.Bind();
            this.Bind();
            GL.TexSubImage2D(this.Target, 0, 0, 0, this.Width, this.Height, this.Format, this.Type, (IntPtr)IntPtr.Zero);
            log.Trace("Texture.RefreshImage ({0}) uploaded", this.Name);
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
            GL.GetTexLevelParameter(this.Target, level, p, out n);
            return n;
        }

        public float[] GetLevelDataFloat(int level)
        {
            Bind();

            int width = this.GetLevelWidth(level);
            int height = this.GetLevelHeight(level);

            float[] data = new float[width * height * 4];

            GL.GetTexImage(this.Target, level, PixelFormat.Rgba, PixelType.Float, data);

            return data;
        }

        public Vector4[] GetLevelDataVector4(int level)
        {
            Bind();

            int width = this.GetLevelWidth(level);
            int height = this.GetLevelHeight(level);

            Vector4[] data = new Vector4[width * height];

            GL.GetTexImage(this.Target, level, PixelFormat.Rgba, PixelType.Float, data);

            return data;
        }
    }
}
