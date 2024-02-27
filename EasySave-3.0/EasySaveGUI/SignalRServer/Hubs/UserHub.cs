using Microsoft.AspNetCore.SignalR;
using Models;
using Newtonsoft.Json;

namespace SignalRServer.Hubs
{
    public class UserHub : Hub
    {

        public async Task UpdateJobs(string pJobsJson)
        {
            await Clients.All.SendAsync("UpdateJobs", pJobsJson);
            Console.WriteLine("JobUpdated");
        }

        private async Task UpdateClients(string pClientsJson)
        {
            await Clients.All.SendAsync("UpdateClients", pClientsJson);
            Console.WriteLine("pClients Updated");
        }

        public override Task OnConnectedAsync()
        {
            Task lTask = base.OnConnectedAsync();
            Console.WriteLine("User Connected");

            CClient lClient = new CClient();
            lClient.ConnectionId = this.Context.ConnectionId;
            ClientsManager.Instance.Clients.Add(lClient);

            UpdateClients(JsonConvert.SerializeObject(ClientsManager.Instance.Clients));

            return lTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ClientsManager.Instance.Clients.RemoveWhere(Cl => Cl.ConnectionId == this.Context.ConnectionId);

            UpdateClients(JsonConvert.SerializeObject(ClientsManager.Instance.Clients));

            Console.WriteLine("User Disconnected " + exception);

            return base.OnDisconnectedAsync(exception);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
