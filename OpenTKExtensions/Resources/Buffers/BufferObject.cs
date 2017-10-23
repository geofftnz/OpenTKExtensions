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
    public class BufferObject<T> : ResourceBase, IResource
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

        private T[] InitialData;

        public BufferObject(string name, BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint usageHint = BufferUsageHint.StaticDraw, T[] initialData = null) : base(name)
        {
            Target = target;
            UsageHint = usageHint;
            InitialData = initialData;
        }

        public static BufferObject<T> CreateVertexBuffer(string name, T[] initialData = null)
        {
            return new BufferObject<T>(name, BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, initialData);
        }
        public static BufferObject<T> CreateIndexBuffer(string name, T[] initialData = null)
        {
            return new BufferObject<T>(name, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw, initialData);
        }

        private void EnsureLoaded([CallerMemberName] string caller = null)
        {
            if (!IsLoaded)
                throw new InvalidOperationException($"Buffer.{caller}({Name}): Not loaded");
        }


        public void Load()
        {
            if (!IsLoaded)
            {
                GL.GenBuffers(1, out handle);

                //if (!GL.IsBuffer(handle))
                //throw new Exception($"Buffer.Load ({Name}): Handle {Handle} is not a buffer");

                log.Trace($"Buffer.Load ({Name}): Handle is {Handle}");

                if (InitialData != null)
                {
                    // HACK: Attempt to load data using known types
                    SetData(InitialData as Vector4[]);
                    SetData(InitialData as Vector3[]);
                    SetData(InitialData as Vector2[]);
                    SetData(InitialData as uint[]);
                    SetData(InitialData as byte[]);
                    SetData(InitialData as float[]);

                    // discard data after load.
                    InitialData = null;
                }

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

        public void SetData<TT>(TT[] data, int elementSizeInBytes, VertexAttribPointerType pointerType, int fieldsPerElement) where TT : struct
        {
            if (data == null)
                return;  // silently fail

            EnsureLoaded();
            log.Trace($"Buffer.SetData ({Name}): Loading...");

            GL.BindBuffer(this.Target, this.Handle);

            this.length = data.Length;
            this.stride = elementSizeInBytes;
            this.arraySize = length * stride;
            this.pointerType = pointerType;
            this.fieldsPerElement = fieldsPerElement;

            GL.BufferData<TT>(this.Target, new IntPtr(arraySize), data, this.UsageHint);
            this.HasData = true;
            GL.BindBuffer(this.Target, 0);
            log.Trace($"Buffer.SetData ({Name}): Loaded {data.Length} elements, {arraySize} bytes");
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
                log.Warn($"Buffer.Unmap ({Name}): Buffer not mapped");
            }
            GL.UnmapBuffer(this.Target);
            IsMapped = false;
        }
    }

    public class BufferObject : BufferObject<object>, IResource
    {
        public BufferObject(string name, BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint usageHint = BufferUsageHint.StaticDraw)
            : base(name, target, usageHint, null)
        {
        }

        public static BufferObject CreateVertexBuffer(string name)
        {
            return new BufferObject(name, BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);
        }
        public static BufferObject CreateIndexBuffer(string name)
        {
            return new BufferObject(name, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw);
        }

    }
}
