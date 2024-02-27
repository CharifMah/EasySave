using Microsoft.AspNetCore.SignalR.Client;

namespace Services
{
    public class UserSignalRService
    {
        private readonly HubConnection _Connection;


        public event Action<string> JobUpdated;


        public UserSignalRService(HubConnection pConnection)
        {
            _Connection = pConnection;
            _Connection.On<string>("UpdateJobs", (lJobs) => JobUpdated?.Invoke(lJobs));
        }

        public async Task SendJobsRunning(string pJobs)
        {
            await _Connection.SendAsync("UpdateJobs", pJobs);
        }
    }
}
