using Microsoft.AspNetCore.SignalR.Client;

namespace EasySaveGUI.ViewModels
{
    public class ClientViewModel
    {
        private HubConnection _Connection;

        public ClientViewModel()
        {
            _Connection = new HubConnectionBuilder().WithUrl($"wss://localhost:7215/UserHub").WithAutomaticReconnect().Build();
        }
    }
}
