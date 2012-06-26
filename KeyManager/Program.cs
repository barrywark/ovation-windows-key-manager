using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

namespace KeyManager
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "/console")
            {
                AllocConsole();

                var app = new KeyRepositoryApp();
                app.Start();

                string input = string.Empty;
                Console.Write("KeyRepositoryApp Console started. Type 'Ctrl-Z then Return' to stop the application... ");

                // Wait for the user to exit the application
                while (input != null) input = Console.ReadLine();

                // Stop the application.
                app.Stop();
            }
            else
            {
                var servicesToRun = new ServiceBase[] { new KeyManagerWindowsService() };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
