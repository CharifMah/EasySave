using Microsoft.AspNetCore.SignalR;
using Models;
using Newtonsoft.Json;

namespace SignalRServer.Hubs
{
    public class UserHub : Hub
    {
        #region UpdateClientVm
        public async Task UpdateClientViewModel(string pClientVmJson, string pConnectionId)
        {
            await CheckConnectionId(pClientVmJson, pConnectionId);
            await Clients.Others.SendAsync("UpdateClientViewModel", pClientVmJson, pConnectionId);
            ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} UpdateClientViewModel | Sender : {pConnectionId}");
        }

        public async Task Start(string pClientVmJson, string pConnectionId,string pTargetConnectionId)
        {
            await CheckConnectionId(pClientVmJson, pConnectionId);
            await Clients.User(pTargetConnectionId).SendAsync("Start", pClientVmJson, pConnectionId);
            ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} Start | Sender : {pConnectionId} Target : {pTargetConnectionId}");
        }

        public async Task Pause(string pClientVmJson, string pConnectionId, string pTargetConnectionId)
        {
            await CheckConnectionId(pClientVmJson, pConnectionId);
            await Clients.User(pTargetConnectionId).SendAsync("Pause", pClientVmJson, pConnectionId);
            ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} Pause | Sender : {pConnectionId} Target : {pTargetConnectionId}");
        }

        public async Task Stop(string pClientVmJson, string pConnectionId, string pTargetConnectionId)
        {
            await CheckConnectionId(pClientVmJson, pConnectionId);
            await Clients.User(pTargetConnectionId).SendAsync("Stop", pClientVmJson, pConnectionId);
            ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} Stop | Sender : {pConnectionId} Target : {pTargetConnectionId}");
        }


        public async Task ReceiveClientViewModel(string pClientVmJson, string pConnectionId)
        {
            if (await CheckConnectionId(pClientVmJson, pConnectionId) && pClientVmJson != null)
            {
                ClientsManager.Instance.UpdateClient(pClientVmJson, pConnectionId);
                await UpdateClientViewModel(pClientVmJson, pConnectionId);
                ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} ReceiveClientViewModel | Sender : {Context.ConnectionId}");
            }
            else
                ConsoleExtention.WriteLineError($"{ConsoleExtention.GetDate()} Client null");
        }
        #endregion

        #region Connection

        private async Task<bool> CheckConnectionId(string pJson, string pConnectionId)
        {
            bool lResult = true;
            if (pConnectionId != Context.ConnectionId)
            {
                lResult = false;
                await SyncConnectionId(pConnectionId);
                ConsoleExtention.WriteLineError($"CheckConnectionId | Context ConnectionId {Context.ConnectionId} don't correspond to {pConnectionId} \n {pJson}");
            }

            return lResult;
        }

        public async Task SyncConnectionId(string pConnectionId)
        {
            await Clients.All.SendAsync("SyncConnectionId", Context.ConnectionId, pConnectionId);
        }

        public override Task OnConnectedAsync()
        {
            Task lTask = base.OnConnectedAsync();

            CClient lClient = new CClient();
            lClient.ConnectionId = Context.ConnectionId;
            lClient.ClientId = Guid.NewGuid().ToString();

            _ = Clients.All.SendAsync("OnConnected", JsonConvert.SerializeObject(lClient));

            ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} OnConnectedAsync | Sender : {Context.ConnectionId}");
            return lTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ClientsManager.Instance.Clients.RemoveWhere(Cl => Cl.Contains(this.Context.ConnectionId));

            Task lTask = OnDisconnected(this.Context.ConnectionId);

            ConsoleExtention.WriteLineWarning($"{ConsoleExtention.GetDate()} OnDisconnectedAsync " + exception);

            return base.OnDisconnectedAsync(exception);
        }

        private async Task OnDisconnected(string pConnectionId)
        {
            await Clients.Others.SendAsync("OnDisconnected", pConnectionId);
            ConsoleExtention.WriteLineSucces($"{ConsoleExtention.GetDate()} OnDisconnected | Sender : {pConnectionId}");
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
