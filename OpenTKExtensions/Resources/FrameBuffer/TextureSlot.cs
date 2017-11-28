using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Resources
{
    /// <summary>
    /// Represents an output of a Multi-RenderTarget GBuffer
    /// </summary>
    public class TextureSlot
    {

        /// <summary>
        /// Slot has texture defined
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Texture is externally defined (does not need to be managed by GBuffer)
        /// </summary>
        public bool External { get; set; } = false;

        /// <summary>
        /// Index of fragment data output
        /// </summary>
        public int Slot { get; set; } = 0;

        /// <summary>
        /// Parameters for texture creation
        /// </summary>
        public TextureSlotParam TextureParam { get; set; }

        /// <summary>
        /// Texture bound to this slot
        /// </summary>
        public Texture Texture { get; set; } = null;

        public int TextureID { get { return Texture?.ID ?? -1; } }

        public DrawBuffersEnum DrawBufferSlot { get { return DrawBuffersEnum.ColorAttachment0 + Slot; } }
        public FramebufferAttachment FramebufferAttachmentSlot { get { return FramebufferAttachment.ColorAttachment0 + Slot; } }

        public TextureSlot()
        {

        }

        public TextureSlot(int colourAttachmentSlot = 0, Texture texture = null, TextureTarget target = TextureTarget.Texture2D)
        {
            Enabled = true;
            External = texture != null;
            Slot = colourAttachmentSlot;
            TextureParam = new TextureSlotParam() { Target = target };
            Texture = texture;
        }

        public void InitTexture(int Width, int Height)
        {
            if (!Enabled) return;

            if (Texture == null && !External)
            {
                Texture = new Texture($"tex_{Slot:00}", Width, Height, TextureTarget.Texture2D, TextureParam.InternalFormat, TextureParam.Format, TextureParam.Type);
            }

            if (!External)
            {
                TextureParam.ApplyParametersTo(Texture);
                Texture.Load();
                Texture.UploadEmpty();
                if (TextureParam.MipMaps)
                {
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                }
            }
        }
        public void UnloadTexture()
        {
            if (Enabled && !External && Texture != null)
            {
                Texture.Unload();
                Texture = null;
                Enabled = false;
            }
        }
        public void AttachToFramebuffer(FramebufferTarget target)
        {
            GL.FramebufferTexture2D(target, FramebufferAttachmentSlot, TextureParam.Target, TextureID, 0);
        }
    }
}
