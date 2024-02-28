using Microsoft.AspNetCore.SignalR.Client;

namespace Services
{
    public class UserSignalRService
    {
        private readonly HubConnection _Connection;

        public event Action<string, string> ClientsUpdated;


        public UserSignalRService(HubConnection pConnection)
        {
            _Connection = pConnection;
            _Connection.On<string,string>("UpdateClients",(lClients,pConnectionId) => ClientsUpdated?.Invoke(lClients,pConnectionId));

        }

        public async Task SendClientViewModel(string pClientViewModel,string pConnectionId)
        {
            await _Connection.SendAsync("ReceiveClientViewModel", pClientViewModel, pConnectionId);
        }
    }
}
