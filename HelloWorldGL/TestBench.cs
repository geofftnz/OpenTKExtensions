using NLog;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTKExtensions.Components;
using OpenTKExtensions.Filesystem;
using OpenTKExtensions.Framework;
using OpenTKExtensions.Resources;
using OpenTKExtensions.Text;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HelloWorldGL
{
    public class TestBench : GameWindow
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private const string SHADERPATH = @"../../Resources/Shaders;Resources/Shaders";

        private GameComponentCollection components = new GameComponentCollection();
        private MultiPathFileSystemPoller shaderUpdatePoller = new MultiPathFileSystemPoller(SHADERPATH.Split(';'));
        //private double lastShaderPollTime = 0.0;
        private Stopwatch timer = new Stopwatch();

        private Font font;
        private RenderTargetBase renderTarget;
        private TestComponent testcomp1;
        private TestComponent2 testcomp2;


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

            components.Add(font = new Font("Resources/Fonts/consolab.ttf_sdf_512.png", "Resources/Fonts/consolab.ttf_sdf_512.txt"));
            
            components.Add(renderTarget = new RenderTargetBase(false, false, 512, 512) { DrawOrder = 1 });
            renderTarget.Loading += (s, e) =>
            {
                renderTarget.SetOutput(0, new TextureSlotParam(TextureTarget.Texture2D, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float, false,
                    Texture.Params().FilterNearest().Repeat().ToArray()
                    ));
            };
            renderTarget.Add(testcomp2 = new TestComponent2());
            
            components.Add(testcomp1 = new TestComponent() { DrawOrder = 2 });
            testcomp1.PreRender += (s, e) => { testcomp1.tex2 = renderTarget.GetTexture(0); };

            components.Add(new FrameCounter(font) { DrawOrder = 3 });


            timer.Start();
        }


        private void TestBench_Resize(object sender, System.EventArgs e)
        {
            GL.Viewport(ClientRectangle);
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
            //GL.Disable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Blend);


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


        protected void LogTrace(string message, [CallerMemberName] string caller = null)
        {
            log.Trace($"{this.GetType().Name}.{caller}: {message}");
        }
        protected void LogInfo(string message, [CallerMemberName] string caller = null)
        {
            log.Info($"{this.GetType().Name}.{caller}: {message}");
        }
        protected void LogWarn(string message, [CallerMemberName] string caller = null)
        {
            log.Warn($"{this.GetType().Name}.{caller}: {message}");
        }
        protected void LogError(string message, [CallerMemberName] string caller = null)
        {
            log.Error($"{this.GetType().Name}.{caller}: {message}");
        }

    }
}
