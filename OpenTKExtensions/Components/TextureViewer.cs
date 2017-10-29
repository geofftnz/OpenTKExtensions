﻿using OpenTKExtensions.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace OpenTKExtensions.Components
{
    public class TextureViewer : GameComponentBase, IRenderable, IResizeable, IKeyboardControllable
    {
        public int DrawOrder { get; set; }
        public bool Visible { get; set; } = true;
        public int KeyboardPriority { get; set; }

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
