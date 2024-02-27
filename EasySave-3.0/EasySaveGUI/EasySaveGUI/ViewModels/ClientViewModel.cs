using EasySaveGUI.UserControls;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Models.Backup;
using Newtonsoft.Json;
using Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace EasySaveGUI.ViewModels
{
    public class ClientViewModel : BaseViewModel
    {
        private bool _IsConnectedToLobby;
        private HubConnection _Connection;
        private UserSignalRService _UserSignalRService;
        private JobViewModel _JobViewModel;
        private ObservableCollection<CClient> _Clients;
        public UserSignalRService UserSignalRService { get => _UserSignalRService; set => _UserSignalRService = value; }
        private static ClientViewModel _Instance;
        public static ClientViewModel Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ClientViewModel();
                return _Instance;
            }
        }

        public JobViewModel JobViewModel { get => _JobViewModel; set => _JobViewModel = value; }
        public ObservableCollection<CClient> Clients { get => _Clients; set => _Clients = value; }

        private ClientViewModel()
        {
            _Connection = new HubConnectionBuilder().WithUrl($"wss://localhost:7215/UserHub").WithAutomaticReconnect().Build();
            _UserSignalRService = new UserSignalRService(_Connection);
            _IsConnectedToLobby = false;
            _UserSignalRService.JobUpdated += _UserSignalRService_JobUpdated;
            _UserSignalRService.ClientsUpdated += _UserSignalRService_ClientsUpdated;
            _JobViewModel = new JobViewModel();
            _Clients = new ObservableCollection<CClient>();
        }

        private void _UserSignalRService_ClientsUpdated(string obj)
        {
            HashSet<CClient>? lClients = JsonConvert.DeserializeObject<HashSet<CClient>>(obj);
            if (lClients != null && lClients.Count > 0)
            {
                _Clients = new ObservableCollection<CClient>(lClients);
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

        private void _UserSignalRService_JobUpdated(string obj)
        {
            JobViewModel? lJobs = JsonConvert.DeserializeObject<JobViewModel>(obj);
            _JobViewModel = lJobs;
        }

        /// <summary>
        /// Connecte le lobby a la connection
        /// </summary>
        /// <returns></returns>
        public async Task ConnectLobby()
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
                        _IsConnectedToLobby = true;
                });
            }
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
