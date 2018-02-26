using OpenTKExtensions.Resources;

namespace OpenTKExtensions.Framework
{
    public interface IRenderTarget
    {
        bool InheritSizeFromParent { get; set; }

        Texture GetTexture(int slot);
        void SetOutput(int index, TextureSlotParam texparam);
        void SetOutput(int index, Texture texture);
    }
}