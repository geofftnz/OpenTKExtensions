using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Resources
{
    /// <summary>
    /// Represents OpenGL resources.
    /// These generally have a handle and go through a Load/Unload cycle.
    /// 
    /// 
    /// </summary>
    public interface IResource
    {
        string Name { get; }
        void Load();
        void Unload();

        /// This event fires when the OpenGL handle has been allocated and basic setup has completed.
        /// Event handlers can use this to set content
        event EventHandler<EventArgs> ReadyForContent;

        event EventHandler<EventArgs> Unloaded;

    }
}
