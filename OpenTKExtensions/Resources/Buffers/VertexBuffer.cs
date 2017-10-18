using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using NLog;
using System.Runtime.CompilerServices;
using OpenTK;

namespace OpenTKExtensions.Resources
{
    //TODO: make this into a generic class, accepting an array on construction, or fire the ReadyForContent event if null.
    public class VertexBuffer : ResourceBase, IResource
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private uint handle = 0;
        public uint Handle { get { return handle; } }

        public BufferTarget Target { get; set; }
        public BufferUsageHint UsageHint { get; set; }

        public bool IsLoaded { get { return handle != 0; } }
        public bool HasData { get; protected set; } = false;
        public bool IsMapped { get; protected set; } = false;

        private int arraySize;
        private int stride;
        private VertexAttribPointerType pointerType;
        private int fieldsPerElement;
        private int length;

        public int Length
        {
            get
            {
                if (HasData)
                {
                    return length;
                }
                throw new InvalidOperationException("Length is not defined until the VBO has been filled with data");
            }
        }

        public int Size
        {
            get
            {
                if (HasData)
                {
                    return arraySize;
                }
                throw new InvalidOperationException("Size is not defined until the VBO has been filled with data");
            }
        }



        public VertexBuffer(string name, BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint usageHint = BufferUsageHint.StaticDraw)
        {
            this.Name = name;
            this.Target = target;
            this.UsageHint = usageHint;
        }

        public static VertexBuffer CreateVertexBuffer(string name)
        {
            return new VertexBuffer(name, BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);
        }
        public static VertexBuffer CreateIndexBuffer(string name)
        {
            return new VertexBuffer(name, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw);
        }

        private void EnsureLoaded([CallerMemberName] string caller = null)
        {
            if (!IsLoaded)
                throw new InvalidOperationException($"VertexBuffer.{caller}({Name}): Not loaded");
        }


        public void Load()
        {
            if (!IsLoaded)
            {
                GL.GenBuffers(1, out handle);

                if (!GL.IsBuffer(handle))
                    throw new Exception($"VertexBuffer.Load ({Name}): Handle {Handle} is not a buffer");

                log.Trace($"VertexBuffer.Load ({Name}): Handle is {Handle}");

                OnReadyForContent();
            }
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                GL.DeleteBuffer(Handle);
                handle = 0;
                HasData = false;

                OnUnloaded();
            }
        }

        public void SetData<T>(T[] data, int elementSizeInBytes, VertexAttribPointerType pointerType, int fieldsPerElement) where T : struct
        {
            EnsureLoaded();
            log.Trace($"VertexBuffer.SetData ({Name}): Loading...");

            GL.BindBuffer(this.Target, this.Handle);

            this.length = data.Length;
            this.stride = elementSizeInBytes;
            this.arraySize = length * stride;
            this.pointerType = pointerType;
            this.fieldsPerElement = fieldsPerElement;

            GL.BufferData<T>(this.Target, new IntPtr(arraySize), data, this.UsageHint);
            this.HasData = true;
            GL.BindBuffer(this.Target, 0);
            log.Trace($"VertexBuffer.SetData ({Name}): Loaded {data.Length} elements, {arraySize} bytes");
        }

        public void SetData(Vector4[] data)
        {
            this.SetData(data, Vector4.SizeInBytes, VertexAttribPointerType.Float, 4);
        }
        public void SetData(Vector3[] data)
        {
            this.SetData(data, Vector3.SizeInBytes, VertexAttribPointerType.Float, 3);
        }
        public void SetData(Vector2[] data)
        {
            this.SetData(data, Vector2.SizeInBytes, VertexAttribPointerType.Float, 2);
        }
        public void SetData(uint[] data)
        {
            this.SetData(data, sizeof(uint), VertexAttribPointerType.UnsignedInt, 1);
        }
        public void SetData(byte[] data)
        {
            this.SetData(data, sizeof(byte), VertexAttribPointerType.UnsignedByte, 1);
        }
        public void SetData(float[] data)
        {
            this.SetData(data, sizeof(float), VertexAttribPointerType.Float, 1);
        }

        public void Bind(int index)
        {
            EnsureLoaded();
            GL.BindBuffer(this.Target, this.Handle);

            if (this.Target != BufferTarget.ElementArrayBuffer)
            {
                GL.EnableVertexAttribArray(index);
                GL.VertexAttribPointer(index, fieldsPerElement, pointerType, false, stride, 0);
            }
        }

        public void Bind()
        {
            EnsureLoaded();
            GL.BindBuffer(this.Target, this.Handle);
        }

        public void Unbind()
        {
            GL.BindBuffer(this.Target, 0);
        }

        public IntPtr Map(BufferAccess access)
        {
            EnsureLoaded();
            var ptr = GL.MapBuffer(this.Target, access);
            IsMapped = true;
            return ptr;
        }

        public void Unmap()
        {
            if (!IsMapped)
            {
                log.Warn("VBO.Unmap ({0}): buffer not mapped", this.Name);
            }
            GL.UnmapBuffer(this.Target);
            IsMapped = false;
        }

    }
}
