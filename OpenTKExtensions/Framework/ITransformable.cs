using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Framework
{
    public interface ITransformable
    {
        Matrix4 ModelView { get; set; }
        Matrix4 Projection { get; set; }
    }
}
