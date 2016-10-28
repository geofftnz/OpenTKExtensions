using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Framework
{
    public interface IKeyboardControllable
    {
        int KeyboardPriority { get; set; }
        bool ProcessKeyDown(OpenTK.Input.KeyboardKeyEventArgs e);
        bool ProcessKeyUp(OpenTK.Input.KeyboardKeyEventArgs e);
    }
}
