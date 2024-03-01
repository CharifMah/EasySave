using Microsoft.AspNetCore.SignalR.Client;

namespace Services
{
    /// <summary>
    /// Service pour l'utilsateur permettant d'envoyer et recevoir des donnée au serveur
    /// </summary>
    public class UserSignalRService
    {
        private readonly HubConnection _Connection;

        public event Action<string> OnDisconnected;
        public event Action<string, string> ClientViewModelUpdated;
        public event Action<string> OnConnected;
        public event Action<string, string> OnSyncConnectionId;
        public event Action<string, string, string> OnStart;
        public event Action<string, string, string> OnPause;
        public event Action<string, string, string> OnStop;

        /// <summary>
        /// Enregistre les Handlers qui seront invoker lors de l'appel du serveur
        /// </summary>
        /// <param name="pConnection"></param>
        public UserSignalRService(HubConnection pConnection)
        {
            _Connection = pConnection;
            _Connection.On<string>("OnDisconnected", (pConnectionId) => OnDisconnected?.Invoke(pConnectionId));
            _Connection.On<string, string>("UpdateClientViewModel", (lClients, pConnectionId) => ClientViewModelUpdated?.Invoke(lClients, pConnectionId));
            _Connection.On<string>("OnConnected", (pConnectionId) => OnConnected?.Invoke(pConnectionId));
            _Connection.On<string, string>("SyncConnectionId", (pConnectionId,pOldConnectionId) => OnSyncConnectionId?.Invoke(pConnectionId, pOldConnectionId));
            _Connection.On<string, string, string>("Start", (pClientVmJson, pConnectionId, pTargetConnectionId) => OnStart?.Invoke(pClientVmJson, pConnectionId, pTargetConnectionId));
            _Connection.On<string, string, string>("Pause", (pClientVmJson, pConnectionId, pTargetConnectionId) => OnPause?.Invoke(pClientVmJson, pConnectionId, pTargetConnectionId));
            _Connection.On<string, string, string>("Stop", (pClientVmJson, pConnectionId, pTargetConnectionId) => OnStop?.Invoke(pClientVmJson, pConnectionId, pTargetConnectionId));
        }
        /// <summary>
        /// Envoie le view model au serveur
        /// </summary>
        /// <param name="pClientViewModel">le view model a envoyer en json</param>
        /// <param name="pConnectionId">le connection id du client view model</param>
        /// <returns>Task</returns>
        public async Task SendClientViewModel(string pClientViewModel,string pConnectionId)
        {
            await _Connection.SendAsync("ReceiveClientViewModel", pClientViewModel, pConnectionId);
        }

        /// <summary>
        /// Envoie le view model au serveur
        /// </summary>
        /// <param name="pClientViewModel">le view model a envoyer en json</param>
        /// <param name="pConnectionId">le connection id du client view model</param>
        /// <returns>Task</returns>
        public async Task Start(string pClientViewModel, string pConnectionId,string pTargetConnectionId)
        {
            await _Connection.SendAsync("Start", pClientViewModel, pConnectionId, pTargetConnectionId);
        }

        /// <summary>
        /// Envoie le view model au serveur
        /// </summary>
        /// <param name="pClientViewModel">le view model a envoyer en json</param>
        /// <param name="pConnectionId">le connection id du client view model</param>
        /// <returns>Task</returns>
        public async Task Pause(string pClientViewModel, string pConnectionId, string pTargetConnectionId)
        {
            await _Connection.SendAsync("Pause", pClientViewModel, pConnectionId, pTargetConnectionId);
        }
        /// <summary>
        /// Envoie le view model au serveur
        /// </summary>
        /// <param name="pClientViewModel">le view model a envoyer en json</param>
        /// <param name="pConnectionId">le connection id du client view model</param>
        /// <returns>Task</returns>
        public async Task Stop(string pClientViewModel, string pConnectionId, string pTargetConnectionId)
        {
            await _Connection.SendAsync("Stop", pClientViewModel, pConnectionId, pTargetConnectionId);
        }

    }
}
