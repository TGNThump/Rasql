using Neutronium.Core.JavascriptFramework;
using Neutronium.WebBrowserEngine.ChromiumFx;
using Neutronium.JavascriptFramework.Vue;
using Neutronium.WPF;
using System.Linq;
using System;
using System.Threading;
using System.Diagnostics;
using GroupProjectRASQL.Framework;
using System.Windows;
using Chromium.Event;

namespace GroupProjectRASQL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ChromiumFxWebBrowserApp
    {
        public ApplicationMode Mode { get; private set; }
        public bool RunTimeOnly => (Mode != ApplicationMode.Dev);
        public bool Debug => (Mode != ApplicationMode.Production);
        public Uri Uri => (Mode == ApplicationMode.Dev) ?
                                new Uri("http://localhost:8080/index.html") : new Uri($"pack://application:,,,/View/dist/index.html");

        public static App MainApplication => Current as App;

        protected override IJavascriptFrameworkManager GetJavascriptUIFrameworkManager()
        {
            return new VueSessionInjector();
        }

        protected override void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
            beforeLineCommand.CommandLine.AppendSwitch("disable-gpu");
        }

#if DEBUG
        protected static Thread MainThread;
            protected static Job job;
            protected static void NPMThread()
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = false;
                cmd.StartInfo.WorkingDirectory = "../../View";
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                job = new Job();
                job.AddProcess(cmd.Handle);

                cmd.StandardInput.WriteLine("npm run integrated 2>&1");
                cmd.StandardInput.Flush();

                while (true)
                {
                    String line = cmd.StandardOutput.ReadLine();
                    if (line == null) break;
                    Console.WriteLine(line);
                    if (line.Contains("Project is running at"))
                    {
                        waitForNPM.Set();
                    }
                }
            }
            protected static ManualResetEvent waitForNPM = new ManualResetEvent(false);

            protected override void OnExit(ExitEventArgs e)
            {
                job.Close();
                base.OnExit(e);
            }
        #endif

        protected override void OnStartUp(IHTMLEngineFactory factory)
        {
            Mode = GetApplicationMode(Args);
            #if DEBUG
                if (Mode == ApplicationMode.Dev)
                {
                    MainThread = Thread.CurrentThread;
                    Thread t = new Thread(new ThreadStart(NPMThread));
                    t.Start();
                    waitForNPM.WaitOne();
                }
            #endif
            factory.RegisterJavaScriptFrameworkAsDefault(new VueSessionInjectorV2 { RunTimeOnly = RunTimeOnly });
            base.OnStartUp(factory);
        }

        private static ApplicationMode GetApplicationMode(string[] args)
        {
#if DEBUG
            var normalizedArgs = args.Select(arg => arg.ToLower()).ToList();

            return (normalizedArgs.Contains("-dev")) ? ApplicationMode.Dev : (normalizedArgs.Contains("-prod"))? ApplicationMode.Production: ApplicationMode.Test;
#else
            return ApplicationMode.Production;
#endif
        }
    }
}
