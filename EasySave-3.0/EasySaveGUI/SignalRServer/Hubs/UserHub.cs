using Microsoft.AspNetCore.SignalR;
using Models;
using Newtonsoft.Json;

namespace SignalRServer.Hubs
{
    public class UserHub : Hub
    {
        private async Task UpdateClients(string pClientsJson, string pConnectionId)
        {
            await Clients.Others.SendAsync("UpdateClients", pClientsJson, pConnectionId);
            ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} Clients Updated | Sender : {pConnectionId}");
        }

        public async Task ReceiveClientViewModel(string pClientsJson, string pConnectionId)
        {
            string? lClientViewModel = pClientsJson;
            if (lClientViewModel != null)
            {
                string? lClientVmJson = ClientsManager.Instance.Clients.FirstOrDefault(cl => cl.Contains(Context.ConnectionId));
                if (string.IsNullOrEmpty(lClientVmJson))
                    ClientsManager.Instance.Clients.Add(lClientViewModel);
                else
                {
                    ClientsManager.Instance.Clients.Remove(lClientVmJson);
                    ClientsManager.Instance.Clients.Add(lClientViewModel);
                    ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} New ClientAdded | Sender : {Context.ConnectionId}");
                }

                await UpdateClients(JsonConvert.SerializeObject(ClientsManager.Instance.Clients), pConnectionId);
            }
            else
                ConsoleExtention.WriteLineError($"{ConsoleExtention.GetDate()} Client null");
        }

        public override Task OnConnectedAsync()
        {
            Task lTask = base.OnConnectedAsync();
            ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} ClientConnected | Sender : {Context.ConnectionId}");
            return lTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ClientsManager.Instance.Clients.RemoveWhere(Cl => Cl.Contains(this.Context.ConnectionId));

            UpdateClients(JsonConvert.SerializeObject(ClientsManager.Instance.Clients), this.Context.ConnectionId);

            ConsoleExtention.WriteLineWarning($"{ConsoleExtention.GetDate()} User Disconnected " + exception);

            return base.OnDisconnectedAsync(exception);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
