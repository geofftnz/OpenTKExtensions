using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Framework
{
    public interface ITransformable
    {
        /// <summary>
        /// Matrix describing the current view in world-space.
        /// </summary>
        Matrix4 ViewMatrix { get; set; }

        /// <summary>
        /// Matrix describing the position of this component in the world.
        /// </summary>
        Matrix4 ModelMatrix { get; set; }

        /// <summary>
        /// Matrix describing the transform from world-space to screen-space.
        /// </summary>
        Matrix4 ProjectionMatrix { get; set; }
    }
}
