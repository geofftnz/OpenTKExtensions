using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions
{
    /// <summary>
    /// Represents OpenGL resources.
    /// These generally have a handle and go through a Load/Unload cycle.
    /// </summary>
    public interface IResource
    {
        void Load();
        void Unload();
    }
}
