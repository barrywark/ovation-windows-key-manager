using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServiceClientConsole.ServiceReference1;

namespace ServiceClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000);
            
            var km = new KeyRepositoryClient();

            Console.WriteLine("Writing key...");
            km.WriteKey("Service", "Client", "Awesome", "some-key");
            Console.WriteLine("  Done. Hit 'Return' to exit...");

            Console.ReadKey();
        }
    }
}
