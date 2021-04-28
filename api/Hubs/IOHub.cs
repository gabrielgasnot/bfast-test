using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.IO;
using System.Linq;

namespace api.Hubs
{
    public class IOHub : Hub
    {
        const string FILE_STORAGE = "out.txt";

        public override Task OnConnectedAsync()
        {
            if(File.Exists(FILE_STORAGE)) {
                var txt = File.ReadAllLines(FILE_STORAGE);
                foreach(var line in txt) {
                    Clients.All.SendAsync("ReceiveOutput", "Server history", line);
                }
            }
            else {
                Clients.All.SendAsync("ReceiveOutput", "Server", "Hi client");
            }
            return base.OnConnectedAsync();
        }

        public async Task SendContent(string user, string message)
        {
            var msg = $"({System.DateTime.Now.ToShortDateString()}) => {user} : {message}";
            if (!File.Exists(FILE_STORAGE))
            {
                using (var f = File.CreateText(FILE_STORAGE))
                {
                    f.WriteLine(msg);
                    f.Close();
                }
            }
            else
            {
                using(var fw = new FileStream(FILE_STORAGE, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using(var sw = new StreamWriter(fw))
                    {
                        sw.WriteLine(msg);
                    }
                }
            }
            await Clients.All.SendAsync("ReceiveOutput", user, message);
        }
    }
}

