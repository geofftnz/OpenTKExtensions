using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWorldGL
{
    static class Program
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //try
            //{
                using (var v = new TestBench())
                {
                    v.Run(60);
                }
            //}
            //catch (Exception ex)
            //{
                //log.Error(ex, "uncaught exception");
                //throw;
            //}

        }
    }
}
