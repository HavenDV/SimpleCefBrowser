using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using SimpleCefBrowser.Forms;

namespace SimpleCefBrowser
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] arguments)
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => OnException(args.ExceptionObject);
            Application.ThreadException += (sender, args) => OnException(args.Exception);

            try
            {
                MainFunction(arguments);
            }
            catch (Exception exception)
            {
                OnException(exception);
            }
        }
        
        public static void MainFunction(string[] arguments)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //For Windows 7 and above, best to include relevant app.manifest entries as well
            Cef.EnableHighDPISupport();
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                //CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
                BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    Environment.Is64BitProcess ? "x64" : "x86",
                    "CefSharp.BrowserSubprocess.exe"),
            };

            //Example of setting a command line argument
            //Enables WebRTC
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

            var browser = new BrowserForm(arguments.Length > 0 ? arguments[0] : "");

            if (arguments.Length > 1)
            {
                var testForm = new FormTestSendMessage(browser);
                testForm.Show();
            }

            Application.Run(browser);
        }

        #region Logging

        private static void OnException(object exceptionObject)
        {
            if (!(exceptionObject is Exception exception))
            {
                exception = new NotSupportedException($"Unhandled exception doesn't derive from System.Exception: {exceptionObject}");
            }

            MessageBox.Show(exception.ToString());
        }

        // Will attempt to load missing assembly from either x86 or x64 subdir
        private static Assembly? Resolver(object sender, ResolveEventArgs args)
        {
            if (!args.Name.StartsWith("CefSharp"))
            {
                return null;
            }

            var assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
            var archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess ? "x64" : "x86",
                assemblyName);

            return File.Exists(archSpecificPath)
                ? Assembly.LoadFrom(archSpecificPath)
                : null;
        }

        #endregion
    }
}
