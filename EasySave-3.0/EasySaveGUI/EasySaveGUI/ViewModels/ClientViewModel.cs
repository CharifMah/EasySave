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
        private CClient _Client;

        private JobViewModel _JobViewModel;

        public JobViewModel JobViewModel { get => _JobViewModel; set => _JobViewModel = value; }
        public CClient Client { get => _Client; set => _Client = value; }

        public ClientViewModel(CClient pClient, JobViewModel pJobViewModel)
        {
            _Client = pClient;
            _JobViewModel = pJobViewModel;
        }
    }
}
