using Microsoft.AspNetCore.SignalR.Client;

namespace Services
{
    public class UserSignalRService
    {
        private readonly HubConnection _Connection;

        public event Action<string> JobUpdated;
        public event Action<string> ClientsUpdated;


        public UserSignalRService(HubConnection pConnection)
        {
            _Connection = pConnection;
            _Connection.On<string>("UpdateJobs", (lJobs) => JobUpdated?.Invoke(lJobs));
            _Connection.On<string>("UpdateClients",(lClients) => ClientsUpdated?.Invoke(lClients));
        }

        public async Task SendJobsRunning(string pJobs)
        {
            await _Connection.SendAsync("UpdateJobs", pJobs);
        }

        public async Task SendClientViewModel(string pClientViewModel)
        {
            await _Connection.SendAsync("ReceiveClientViewModel", pClientViewModel);
        }
    }
}
