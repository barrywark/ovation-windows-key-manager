using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

namespace KeyManager
{
    static class NativeMethods
    {

        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();

    }

    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                NativeMethods.AllocConsole();

                var app = new KeyRepositoryApp();
                app.Start();

                Console.Write("KeyRepositoryApp Console started. Type 'Return' to stop the application... ");


                var input = Console.ReadKey(true);
                while(input.Key != ConsoleKey.Enter)
                {
                    Console.Beep();
                    input = Console.ReadKey(true);
                }

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
