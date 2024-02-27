using Microsoft.AspNet.SignalR.Client;

namespace Services
{
    public class UserSignalRService
    {
        private readonly HubConnection _Connection;

        public UserSignalRService(HubConnection pConnection)
        {
            _Connection = pConnection;
        }
    }
}
