using System.Diagnostics;

namespace whistle_mouse
{

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
       
        [STAThread]


        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

             ApplicationConfiguration.Initialize();
            // Application.Run(new Form1());

            if (Debugger.IsAttached)
            {
                Run();
                return;
            }

            Application.ThreadException += ApplicationThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Run();
        }
        static void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
        static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Form5 eid = new Form5(e.Exception);
            eid.ShowDialog();
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
             Form6 eid = new Form6(e.ExceptionObject as Exception);
             eid.ShowDialog();
       
        }

    }
}