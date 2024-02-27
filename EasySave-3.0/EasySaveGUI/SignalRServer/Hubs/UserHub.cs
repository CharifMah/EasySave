using Microsoft.AspNetCore.SignalR;
using Models;
using Newtonsoft.Json;

namespace SignalRServer.Hubs
{
    public class UserHub : Hub
    {
        private async Task UpdateClients(string pClientsJson)
        {
            await Clients.All.SendAsync("UpdateClients", pClientsJson);
            Console.WriteLine("pClients Updated");
        }

        public async Task ReceiveClientViewModel(string pClientsJson)
        {
            string? lClientViewModel = pClientsJson;
            if (lClientViewModel != null)
            {
                ClientsManager.Instance.Clients.Add(lClientViewModel);
                await UpdateClients(JsonConvert.SerializeObject(ClientsManager.Instance.Clients));

                Console.WriteLine("User Connected");
            }
            else
                Console.WriteLine("Client null");
        }

        public override Task OnConnectedAsync()
        {
            Task lTask = base.OnConnectedAsync();

            return lTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ClientsManager.Instance.Clients.RemoveWhere(Cl => Cl.Contains(this.Context.ConnectionId));

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
