using EasySaveGUI.UserControls;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Newtonsoft.Json;
using Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #endregion
        public UserSignalRService UserSignalRService { get => _UserSignalRService; set => _UserSignalRService = value; }

        public ClientViewModel ClientViewModel { get => _ClientViewModel; set => _ClientViewModel = value; }
        public ObservableCollection<ClientViewModel> Clients { get => _Clients; set => _Clients = value; }

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

        public UserViewModel()
        {
            _Clients = new ObservableCollection<ClientViewModel>();
            _Connection = new HubConnectionBuilder().WithUrl($"wss://localhost:7215/UserHub").WithAutomaticReconnect().Build();
            _UserSignalRService = new UserSignalRService(_Connection);
            _UserSignalRService.ClientsUpdated += _UserSignalRService_ClientsUpdated;
            _IsConnectedToLobby = false;

        }
        #endregion



        /// <summary>
        /// Connecte le lobby a la connection
        /// </summary>
        /// <returns></returns>
        public async Task ConnectLobby(JobViewModel pJobViewModel)
        {
            if (_Connection.State == HubConnectionState.Connected)
                _IsConnectedToLobby = true;
            else
            {
                await _Connection.StartAsync().ContinueWith(async task =>
                {
                    if (task.Exception != null)
                        _IsConnectedToLobby = false;
                    else
                    {
                        _IsConnectedToLobby = true;
                        CClient lClient = new CClient();
                        lClient.ConnectionId = _Connection.ConnectionId;
                        _ClientViewModel = new ClientViewModel(lClient, pJobViewModel);

                        await _UserSignalRService.SendClientViewModel(_ClientViewModel.ToJson());
                    }
                });
            }
        }

        private void _UserSignalRService_ClientsUpdated(string obj)
        {
            HashSet<string>? lClients = JsonConvert.DeserializeObject<HashSet<string>>(obj);
            ObservableCollection<ClientViewModel> lClientViewModels = new ObservableCollection<ClientViewModel>();
            foreach (string lClient in lClients)
            {
                ClientViewModel? lClientVm = JsonConvert.DeserializeObject<ClientViewModel>(lClient);
                lClientViewModels.Add(lClientVm);
            }
            if (lClients != null && lClients.Count > 0)
            {
                _Clients = lClientViewModels;
                NotifyPropertyChanged("Clients");
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                MainWindow lMainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
                if (lMainWindow.MainVm.LayoutVm.ElementsContent.Content is ConnectionMenuControl)
                {
                    (lMainWindow.MainVm.LayoutVm.ElementsContent.Content as ConnectionMenuControl).UpdateListClients(_Clients);
                }
            });
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
