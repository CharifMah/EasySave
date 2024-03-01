using EasySaveGUI.UserControls;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Newtonsoft.Json;
using Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasySaveGUI.ViewModels
{
    class UserViewModel : BaseViewModel
    {
        #region Attributes
        private bool _IsConnectedToLobby;
        private HubConnection _Connection;
        private UserSignalRService _UserSignalRService;
        private ClientViewModel _ClientViewModel;
        private ObservableCollection<ClientViewModel> _Clients;
        private JobViewModel _TempJobViemModel;
        #endregion

        #region Property
        public UserSignalRService UserSignalRService { get => _UserSignalRService; set => _UserSignalRService = value; }
        public ClientViewModel ClientVm { get => _ClientViewModel; set => _ClientViewModel = value; }
        public ObservableCollection<ClientViewModel> Clients { get => _Clients; set => _Clients = value; }
        #endregion

        #region CTOR
        private static UserViewModel _Instance;
        public static UserViewModel Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new UserViewModel();
                return _Instance;
            }
        }

        public HubConnection Connection { get => _Connection; set => _Connection = value; }

        public UserViewModel()
        {
            _Clients = new ObservableCollection<ClientViewModel>();
            _Connection = new HubConnectionBuilder().WithUrl($"wss://localhost:7215/UserHub").WithAutomaticReconnect().Build();
            _UserSignalRService = new UserSignalRService(_Connection);
            _UserSignalRService.OnConnected += _UserSignalRService_OnConnected; ;
            _UserSignalRService.OnDisconnected += _UserSignalRService_OnDisconnected;
            _UserSignalRService.ClientViewModelUpdated += _UserSignalRService_ClientViewModelUpdated;
            _UserSignalRService.OnSyncConnectionId += _UserSignalRService_OnSyncConnectionId;
            _UserSignalRService.OnStart += _UserSignalRService_OnStart;
            _UserSignalRService.OnPause += _UserSignalRService_OnPause;
            _UserSignalRService.OnStop += _UserSignalRService_OnStop;
            _IsConnectedToLobby = false;
        }

        private void _UserSignalRService_OnStop(string pClientVmJson, string pConnectionId, string pTargetConnectionId)
        {
            ClientViewModel? lClientVmDistant = JsonConvert.DeserializeObject<ClientViewModel>(pClientVmJson);
            this.ClientVm.JobVm.Stop(this.ClientVm.JobVm.JobsRunning.ToList());
        }

        private void _UserSignalRService_OnPause(string pClientVmJson, string pConnectionId, string pTargetConnectionId)
        {
            ClientViewModel? lClientVmDistant = JsonConvert.DeserializeObject<ClientViewModel>(pClientVmJson);
            this.ClientVm.JobVm.Pause(this.ClientVm.JobVm.JobsRunning.ToList());
        }

        private async void _UserSignalRService_OnStart(string pClientVmJson, string pConnectionId, string pTargetConnectionId)
        {
            ClientViewModel? lClientVmDistant = JsonConvert.DeserializeObject<ClientViewModel>(pClientVmJson);
            this.ClientVm.JobVm.RunJobs(this.ClientVm.JobVm.JobsRunning.ToList());
        }

        private void _UserSignalRService_OnSyncConnectionId(string pConnectionId, string pOldConnectionId)
        {
            _ClientViewModel.Client.ConnectionId = _Connection.ConnectionId;
            if (_Clients.Count > 0)
            {
                ClientViewModel? lClientVm = _Clients.FirstOrDefault(c => c.Client.ConnectionId == pOldConnectionId);
                if (lClientVm != null) lClientVm.Client.ConnectionId = pConnectionId;
            }
        }

        #endregion

        /// <summary>
        /// Connecte le lobby a la connection
        /// </summary>
        /// <param name="pJobViewModel">Current job view model</param>
        /// <returns>Task</returns>
        public async Task ConnectLobby(JobViewModel pJobViewModel)
        {
            if (_Connection.State == HubConnectionState.Connected)
                _IsConnectedToLobby = true;
            else
            {
                await _Connection.StartAsync().ContinueWith(task =>
                {
                    if (task.Exception != null)
                        _IsConnectedToLobby = false;
                    else
                    {
                        _IsConnectedToLobby = true;
                        _TempJobViemModel = pJobViewModel;
                    }
                });
            }
        }

        private async void _UserSignalRService_OnConnected(string pClientJson)
        {
            CClient? lClient = JsonConvert.DeserializeObject<CClient>(pClientJson);
            if (lClient != null)
            {
                while (_Connection.State == HubConnectionState.Connecting)
                {
                    await Task.Delay(500);
                }
                if (_Connection.State == HubConnectionState.Connected)
                {
                    _ClientViewModel = new ClientViewModel(lClient, _TempJobViemModel);
                    _ClientViewModel.Client.ConnectionId = _Connection.ConnectionId;
                    await _UserSignalRService.SendClientViewModel(_ClientViewModel.ToJson(), _ClientViewModel.Client.ConnectionId);
                }
            }
            else
            {
                throw new System.Exception("Client non reçu");
            }

        }

        private void UpdateClientViewModel(string pClientVm, string pSenderConnectionId)
        {

            App.Current.Dispatcher.BeginInvoke(() =>
            {
                ClientViewModel? lClientVmDistant = JsonConvert.DeserializeObject<ClientViewModel>(pClientVm);
                ClientViewModel? lClientLocal = _Clients.FirstOrDefault(c => c.Client.ConnectionId == pSenderConnectionId);
                MainWindow lMainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;

                if (lClientVmDistant != null && lClientLocal != null && lClientLocal.Client.ConnectionId == pSenderConnectionId)
                {
                    int lIndex = _Clients.IndexOf(lClientLocal);

                    if (lIndex != -1)
                    {
                        _Clients[lIndex] = lClientVmDistant;
                    }
                    lMainWindow.MainVm.ConnectionMenuControl.UpdateClientViewModel(pSenderConnectionId);
                }
                else
                {
                    _Clients.Add(lClientVmDistant);

            
                    lMainWindow.MainVm.ConnectionMenuControl.UpdateListClients(_Clients);
                }

               
            });

            NotifyPropertyChanged("Clients");

        }

        private void _UserSignalRService_ClientViewModelUpdated(string pClientVm, string pConnectionId)
        {
            UpdateClientViewModel(pClientVm, pConnectionId);
        }

        private void _UserSignalRService_OnDisconnected(string pConnectionId)
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                _Clients.Remove(_Clients.FirstOrDefault(c => c.Client.ConnectionId == pConnectionId));
            });
        }

        public async Task Start(ClientViewModel pClientViewModel)
        {
            await _UserSignalRService.Start(pClientViewModel.ToJson(),_Connection.ConnectionId, pClientViewModel.Client.ConnectionId);
        }

        public async Task Stop(ClientViewModel pClientViewModel)
        {
            await _UserSignalRService.Stop(pClientViewModel.ToJson(), _Connection.ConnectionId, pClientViewModel.Client.ConnectionId);

        }

        public async Task Pause(ClientViewModel pClientViewModel)
        {
            await _UserSignalRService.Pause(pClientViewModel.ToJson(), _Connection.ConnectionId, pClientViewModel.Client.ConnectionId);
        }

        /// <summary>
        /// DisposeConnection the connection
        /// </summary>
        /// <returns></returns>
        public async Task DisposeConnection()
        {
            if (_Connection != null)
            {
                await _Connection.DisposeAsync();
            }
            _IsConnectedToLobby = false;
        }
    }
}
