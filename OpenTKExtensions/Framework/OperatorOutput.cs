using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExtensions.Framework
{
    public class OperatorOutput
    {
        public string Name { get; private set; }
        public Texture Texture { get; set; }
        public TextureTarget Target { get; set; }

        public OperatorOutput(string name, Texture texture, TextureTarget target = TextureTarget.Texture2D)
        {
            Name = name;
            Texture = texture;
            Target = target;        
        }


    }
}
