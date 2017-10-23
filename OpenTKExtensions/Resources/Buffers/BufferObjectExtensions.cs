using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTKExtensions.Resources
{
    public static class BufferObjectExtensions
    {
        public static BufferObject<T> ToBufferObject<T>(this IEnumerable<T> data, string name = "vbo", BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint usageHint = BufferUsageHint.StaticDraw) where T : struct
        {
            return new BufferObject<T>(name, target, usageHint, data.ToArray());
        }
    }
}
