using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using NLog;
using OpenTK;

namespace OpenTKExtensions.Resources
{
    public class FrameBuffer : ResourceBase, IResource
    {
        public int ID { get; private set; } = -1;
        public FramebufferTarget Target { get; private set; }
        public FramebufferErrorCode Status { get; private set; }

        public bool IsLoaded { get { return ID != -1; } }

        public bool IsStatusOK
        {
            get
            {
                return Status == FramebufferErrorCode.FramebufferComplete;
            }
        }

        public FrameBuffer(string name, FramebufferTarget target) : base(name)
        {
            Target = target;
            Status = FramebufferErrorCode.FramebufferUndefined;
        }
        public FrameBuffer(string name)
            : this(name, FramebufferTarget.Framebuffer)
        {
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                ID = GL.GenFramebuffer();
                LogInfo($" ID = {ID}");
            }
        }

        public void Unload()
        {
            if (IsLoaded)
            {
                GL.DeleteFramebuffer(ID);
                ID = -1;
            }
        }

        public void Bind()
        {
            Bind(Target);
        }
        public void Bind(FramebufferTarget target)
        {
            if (!IsLoaded)
                throw new InvalidOperationException($"FrameBuffer.Bind ({Name}) not loaded");

            GL.BindFramebuffer(target, ID);
        }

        public void Unbind()
        {
            Unbind(Target);
        }
        public void Unbind(FramebufferTarget target)
        {
            GL.BindFramebuffer(target, 0);
        }


        public FramebufferErrorCode GetStatus()
        {
            Status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            return Status;
        }

        public void ClearColourBuffer(int drawBuffer, Vector4 colour)
        {
            GL.ClearBuffer(ClearBuffer.Color, drawBuffer, new float[] { colour.X, colour.Y, colour.Z, colour.W });
        }


    }
}
