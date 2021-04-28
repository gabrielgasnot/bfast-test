using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace client_console
{
    class Program
    {
        static HubConnection hubConnection;
        static string currentUser;
        const string FILE_IN = "in.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Type your username (empty text to quit):");
            currentUser = Console.ReadLine();

            if (string.IsNullOrEmpty(currentUser))
            {
                return;
            }

            Console.WriteLine("Type your text (empty text to quit):");
            if (!File.Exists(FILE_IN))
            {
                using (var fs = File.Create(FILE_IN)) { }
            }
            FileListener();
            InputData();
        }

        static void InputData()
        {
            while (true)
            {
                var txt = Console.ReadLine();

                // End of input.
                if (string.IsNullOrEmpty(txt.Trim()))
                {
                    break;
                }

                using (var fw = new FileStream(FILE_IN, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fw))
                    {
                        sw.WriteLineAsync(txt);
                    }
                }
            }
        }

        static async void FileListener()
        {
            hubConnection = new HubConnectionBuilder()
                                        .WithUrl("http://localhost:5000/hubs/iohub")
                                        .Build();

            Microsoft.AspNetCore.SignalR.Client.HubConnectionExtensions.On(hubConnection, "ReceiveOutput",
                                    (string user, string message) => { Console.WriteLine($"{user} says {message}"); });

            await hubConnection.StartAsync();

            while (true)
            {
                using (var fr = new FileStream(FILE_IN, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fr))
                    {
                        if (!sr.EndOfStream)
                        {
                            var line = sr.ReadLineAsync();
                            var t = Send(line.Result);
                            await t;
                            if (t.IsCompletedSuccessfully)
                            {
                                // Remove read line from file.
                                var lines = File.ReadAllLines(FILE_IN);
                                File.WriteAllLines(FILE_IN, lines.Skip(1).ToArray());
                            }
                        }

                    }
                }
            }

        }

        static async Task Send(string line)
        {
            try
            {
                await hubConnection.SendAsync("SendContent", currentUser, line);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serveur indisponible, log du message pour envoi futur.");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
