using OpenTKExtensions.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK;

namespace OpenTKExtensions.Components
{
    public class TextureViewer : GameComponentBase, IRenderable, IResizeable, IKeyboardControllable, ITransformable
    {
        public int DrawOrder { get; set; }
        public bool Visible { get; set; } = true;
        public int KeyboardPriority { get; set; }

        public Matrix4 ViewMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
        public Matrix4 ProjectionMatrix { get; set; } = Matrix4.Identity;

        public TextureViewer() : base()
        {

        }

        public void Render(IFrameRenderData frameData)
        {
            
        }

        public void Resize(int width, int height)
        {

        }

        public bool ProcessKeyDown(KeyboardKeyEventArgs e)
        {
            return true;
        }

        public bool ProcessKeyUp(KeyboardKeyEventArgs e)
        {
            return true;
        }
    }
}
