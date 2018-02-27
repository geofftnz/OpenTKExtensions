using NLog;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    /// <summary>
    /// GBuffer - a framebuffer & collection of textures used for render-to-texture and MRT.
    /// </summary>
    public class GBuffer : ResourceBase, IResource
    {
        public const int MAXSLOTS = 16;

        private TextureSlot[] TextureSlots = new TextureSlot[MAXSLOTS];

        public Texture DepthTexture { get; private set; }
        public FrameBuffer FBO { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool WantDepth { get; set; }

        private int[] previousViewport = new int[4];

        public class ResizedEventArgs : EventArgs
        {
            public int Width { get; private set; }
            public int Height { get; private set; }
            public ResizedEventArgs(int width, int height) : base()
            {
                Width = width;
                Height = height;
            }
        }
        public event EventHandler<ResizedEventArgs> Resized;

        public virtual void OnResized(ResizedEventArgs e)
        {
            Resized?.Invoke(this, e);
        }


        public GBuffer(string name, bool wantDepth, int width, int height) : base(name)
        {
            WantDepth = wantDepth;
            Width = width;
            Height = height;
            FBO = new FrameBuffer(Name + "_GBuffer");

            for (int i = 0; i < MAXSLOTS; i++)
            {
                TextureSlots[i] = new TextureSlot();
            }
        }

        public GBuffer(string name, bool wantDepth)
            : this(name, wantDepth, 256, 256)
        {
        }

        public GBuffer(string name)
            : this(name, true)
        {
        }

        public void Load()
        {
            LogTrace($"Creating G-Buffer of size {Width}x{Height}");
            FBO.Load();

            FBO.Bind();
            UnloadAndDestroyAllTextures();
            InitAllTextures();

            if (WantDepth)
                InitAndAttachDepthTexture();

            SetDrawBuffers();

            var status = FBO.GetStatus();
            LogTrace($"FBO state is {FBO.Status}");

            FBO.Unbind();
        }

        public void Unload()
        {
            UnloadAndDestroyAllTextures();
            FBO.Unload();
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            Load();

            OnResized(new ResizedEventArgs(Width, Height));
        }

        public GBuffer SetSlot(int slot, TextureSlotParam texparam)
        {
            if (slot < 0 || slot >= MAXSLOTS)
            {
                throw new InvalidOperationException("GBuffer.SetSlotTextureParams: slot out of range.");
            }

            TextureSlots[slot].Enabled = true;
            TextureSlots[slot].External = false;
            TextureSlots[slot].Slot = slot;
            TextureSlots[slot].TextureParam = texparam;

            LogTrace($"Internal texture {slot} = {texparam}");

            return this;
        }

        public GBuffer SetSlot(int slot, Texture texture)
        {
            if (slot < 0 || slot >= MAXSLOTS)
            {
                throw new InvalidOperationException("GBuffer.SetSlotTextureParams: slot out of range.");
            }

            if (!TextureSlots[slot].External)
                UnloadTextureAtSlot(slot);

            TextureSlots[slot].Enabled = true;
            TextureSlots[slot].External = true;
            TextureSlots[slot].Slot = slot;
            TextureSlots[slot].Texture = texture;
            TextureSlots[slot].TextureParam = new TextureSlotParam(texture.InternalFormat, texture.Format, texture.Type);


            LogTrace($"External texture {slot} = {TextureSlots[slot].TextureParam}");

            return this;
        }

        public IEnumerable<Texture> GetTextures()
        {
            for (int i = 0; i < MAXSLOTS; i++)
            {
                var t = GetTextureAtSlotOrNull(i);
                if (t != null)
                {
                    yield return t;
                }
            }
        }

        private void InitAllTextures()
        {
            for (int i = 0; i < MAXSLOTS; i++)
            {
                if (TextureSlots[i].Enabled)
                {
                    if (!TextureSlots[i].External)
                    {
                        LogTrace($"Init internal texture for slot {TextureSlots[i].Slot}");
                        TextureSlots[i].InitTexture(Width, Height);
                    }
                    LogTrace($"Attaching slot {TextureSlots[i].Slot} to {FBO.Target}");
                    TextureSlots[i].AttachToFramebuffer(FBO.Target);
                }
            }
        }

        private void UnloadAndDestroyAllTextures()
        {
            for (int i = 0; i < MAXSLOTS; i++)
            {
                UnloadTextureAtSlot(i);
            }
        }

        private void UnloadTextureAtSlot(int i)
        {
            if (TextureSlots[i].Enabled && !TextureSlots[i].External)
            {
                TextureSlots[i].UnloadTexture();
            }
        }

        private void SetDrawBuffers()
        {
            var buffers = TextureSlots.Where(s => s.Enabled).Select(s => s.DrawBufferSlot).ToArray();
            if (buffers.Length > 0)
            {
                GL.DrawBuffers(buffers.Length, buffers);
            }
        }

        private void InitAndAttachDepthTexture()
        {
            // dump old depth texture
            if (DepthTexture != null)
            {
                DepthTexture.Unload();
                DepthTexture = null;
            }
            // create & bind depth texture
            DepthTexture = new Texture(Width, Height, TextureTarget.Texture2D, PixelInternalFormat.DepthComponent32, PixelFormat.DepthComponent, PixelType.Float);

            DepthTexture.Set(TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge);
            DepthTexture.Set(TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge);
            DepthTexture.Set(TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest);
            DepthTexture.Set(TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest);
            DepthTexture.Set(TextureParameterName.TextureCompareMode, TextureCompareMode.None);
            DepthTexture.Set(TextureParameterName.TextureCompareFunc, All.None);

            DepthTexture.Load();
            LogTrace($"Depth Texture: {DepthTexture}");
            DepthTexture.UploadEmpty();
            DepthTexture.Bind();
            GL.FramebufferTexture2D(FBO.Target, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthTexture.ID, 0);
        }

        private void SaveViewport()
        {
            GL.GetInteger(GetPName.Viewport, previousViewport);
        }

        private void RestoreViewport()
        {
            GL.Viewport(previousViewport[0], previousViewport[1], previousViewport[2], previousViewport[3]);
        }

        public void BindForWriting()
        {
            SaveViewport();
            FBO.Bind(FramebufferTarget.DrawFramebuffer);
            GL.Viewport(0, 0, Width, Height);
            SetDrawBuffers();
            if (WantDepth)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthMask(true);
            }
        }

        public void BindForWritingMulti()
        {
            BindForWritingTo(TextureSlots.Where(s => s.Enabled).ToArray());
        }

        /// <summary>
        /// Bind GBuffer for writing to the supplied textures
        /// </summary>
        /// <param name="outputTextures"></param>
        public void BindForWritingTo(params TextureSlot[] outputTextures)
        {
            // shouldn't call this if we've got any texture slots defined.
            SaveViewport();
            FBO.Bind(FramebufferTarget.DrawFramebuffer);
            GL.Viewport(0, 0, Width, Height);

            for (int i = 0; i < outputTextures.Length; i++)
            {
                outputTextures[i].AttachToFramebuffer(FBO.Target);
            }

            GL.DrawBuffers(outputTextures.Length, outputTextures.Select(t => t.DrawBufferSlot).ToArray());
        }

        public void UnbindFromWriting()
        {
            FBO.Unbind(FramebufferTarget.DrawFramebuffer);
            RestoreViewport();

            // generate any requested mipmaps
            foreach (var ts in TextureSlots.Where(s => s.Enabled && s.TextureParam.MipMaps))
            {
                GL.Enable(EnableCap.Texture2D);
                ts.Texture.Bind();
                ts.Texture.ApplyParameters();
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }

        public void BindForReading()
        {
        }

        public Texture GetTextureAtSlot(int slot)
        {
            if (slot < 0 || slot >= MAXSLOTS)
                throw new InvalidOperationException($"GBuffer.SetSlotTextureParams ({Name}): slot {slot} out of range.");

            if (TextureSlots[slot] == null || !TextureSlots[slot].Enabled)
                throw new InvalidOperationException($"GBuffer.SetSlotTextureParams ({Name}): no texture at slot {slot}.");

            return TextureSlots[slot].Texture;
        }

        public Texture GetTextureAtSlotOrNull(int slot)
        {
            if (slot < 0 || slot >= MAXSLOTS)
            {
                return null;
            }
            if (TextureSlots[slot] == null || !TextureSlots[slot].Enabled)
            {
                return null;
            }

            return TextureSlots[slot].Texture;
        }

        public void ClearColourBuffer(int drawBuffer, Vector4 colour)
        {
            FBO.ClearColourBuffer(drawBuffer, colour);
        }
        public void ClearAllColourBuffers(Vector4 colour)
        {
            foreach (var slot in TextureSlots.Where(s => s.Enabled).Select(s => s.Slot))
                FBO.ClearColourBuffer(slot, colour);
        }

    }
}
