using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Mono.Options;
using Physion.Ovation.KeyManager.KeyConsole.ServiceProxy;
using Physion.Ovation.KeyRepositoryService;

namespace Physion.Ovation.KeyManager.KeyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool show_help = false;
            string institution = null;
            string group = null;
            const string product = "Ovation";
            bool system_key = false;

            var p = new OptionSet() {
            { "h|help",  "show this message and exit", 
              v => show_help = v != null },
              {"i=|institution=", "licensed institution",
              (string v) => institution = v},
              {"g=|group=", "licensed group",
              (string v) => group = v},
              {"s|system", "add key to system query server (requires Administrator role)",
                  v => system_key = v != null}
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("keyconsole: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `keyconsole --help' for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(p);
                return;
            }

            if (institution == null)
            {
                Console.WriteLine("You must provide the licensed institution.");
                Console.WriteLine("Try `keyconsole --help' for more information.");
                return;
            }

            if (group == null)
            {
                Console.WriteLine("You must provide the licensed group.");
                Console.WriteLine("Try `keyconsole --help' for more information.");
                return;
            }

            if (system_key && !IsUserAdministrator())
            {
                
                Console.WriteLine("Writing a key to the system (query server) key store requires administrator role.");
                Console.WriteLine("Run keyconsole as Administrator.");
                return;
            }

            var fsManager = new FileSystemKeyRepository();

            Console.Write("Shared ecnryption key:");
            var keyBuilder = ReadConsoleKey();
            Console.WriteLine("");

            Console.WriteLine("Re-enter shared encryption key:");
            var keyBuilderComp = ReadConsoleKey();
            Console.WriteLine("");

            if (!keyBuilder.ToString().Equals(keyBuilderComp.ToString()))
            {
                Console.WriteLine("Keys do not match. Keys have not been modified");
                return;
            }

            try
            {
                fsManager.WriteKey(institution, group, product, keyBuilder.ToString());
                if (system_key)
                {
                    var proxy = new KeyRepositoryClient();
                    proxy.WriteKey(institution, group, product, keyBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.Write("keyconsole: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try `keyconsole --help' for more information.");
            }
        }

        private static StringBuilder ReadConsoleKey()
        {
            var keyBuilder = new StringBuilder();
            var input = Console.ReadKey(true);
            while (input.Key != ConsoleKey.Enter)
            {
                keyBuilder.Append(input.KeyChar);
                input = Console.ReadKey(true);
            }
            return keyBuilder;
        }

        private static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: keyconsole [-system] -i <institution> -g <group>");
            Console.WriteLine("Add/update the shared ecnryption key associated with an Ovation license.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
