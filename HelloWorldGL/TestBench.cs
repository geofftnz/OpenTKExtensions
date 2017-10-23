using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Filesystem;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using System.Diagnostics;
using System.Threading;

namespace HelloWorldGL
{
    public class TestBench : GameWindow
    {
        private const string SHADERPATH = @"../../Resources/Shaders;Resources/Shaders";

        private GameComponentCollection components = new GameComponentCollection();
        private MultiPathFileSystemPoller shaderUpdatePoller = new MultiPathFileSystemPoller(SHADERPATH.Split(';'));
        //private double lastShaderPollTime = 0.0;
        private Stopwatch timer = new Stopwatch();


        public class RenderData : IFrameRenderData, IFrameUpdateData
        {
            public double Time { get; set; }
        }
        public RenderData frameData = new RenderData();


        public TestBench() :
            base(
                 800, 600,
                 GraphicsMode.Default, "OpenTKExtensions TestBench",
                 GameWindowFlags.Default, DisplayDevice.Default,
                 4, 3, GraphicsContextFlags.ForwardCompatible
                )
        {
            VSync = VSyncMode.Off;

            // set static loader
            ShaderProgram.DefaultLoader = new OpenTKExtensions.Loaders.MultiPathFileSystemLoader(SHADERPATH);


            Load += TestBench_Load;
            Unload += TestBench_Unload;
            UpdateFrame += TestBench_UpdateFrame;
            RenderFrame += TestBench_RenderFrame;
            Resize += TestBench_Resize;

            // TODO: Components.
            components.Add(new TestComponent());

            // set default shader loader
            //ShaderProgram.DefaultLoader = new OpenTKExtensions.Loaders.MultiPathFileSystemLoader(SHADERPATH);


        }

        private void TestBench_Resize(object sender, System.EventArgs e)
        {
            GL.Viewport(this.ClientRectangle);
            components.Resize(ClientRectangle.Width, ClientRectangle.Height);
        }

        private void TestBench_RenderFrame(object sender, FrameEventArgs e)
        {
            if (shaderUpdatePoller.Poll())
            {
                components.Reload();
                shaderUpdatePoller.Reset();
            }



            GL.ClearColor(0.0f, 0.1f, 0.2f, 1.0f);
            GL.ClearDepth(1.0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            components.Render(frameData);

            SwapBuffers();
            Thread.Sleep(0);

        }

        private void TestBench_UpdateFrame(object sender, FrameEventArgs e)
        {
            frameData.Time = timer.Elapsed.TotalSeconds;
            components.Update(frameData);
        }

        private void TestBench_Unload(object sender, System.EventArgs e)
        {
            components.Unload();
        }

        private void TestBench_Load(object sender, System.EventArgs e)
        {
            components.Load();
            timer.Start();
        }
    }
}
