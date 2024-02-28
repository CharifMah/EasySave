using Microsoft.AspNetCore.SignalR.Client;

namespace Services
{
    public class UserSignalRService
    {
        private readonly HubConnection _Connection;

        public event Action<string> OnDisconnected;
        public event Action<string, string> ClientViewModelUpdated;
        public event Action<string> OnConnected;
        public event Action<string, string> OnSyncConnectionId;

        public UserSignalRService(HubConnection pConnection)
        {
            _Connection = pConnection;
            _Connection.On<string>("OnDisconnected", (pConnectionId) => OnDisconnected?.Invoke(pConnectionId));
            _Connection.On<string, string>("UpdateClientViewModel", (lClients, pConnectionId) => ClientViewModelUpdated?.Invoke(lClients, pConnectionId));
            _Connection.On<string>("OnConnected", (pConnectionId) => OnConnected?.Invoke(pConnectionId));
            _Connection.On<string, string>("SyncConnectionId", (pConnectionId,pOldConnectionId) => OnSyncConnectionId?.Invoke(pConnectionId, pOldConnectionId));

        }

        public async Task SendClientViewModel(string pClientViewModel,string pConnectionId)
        {
            await _Connection.SendAsync("ReceiveClientViewModel", pClientViewModel, pConnectionId);
        }
    }
}
