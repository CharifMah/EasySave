using EasySaveGUI.UserControls;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Models.Backup;
using Newtonsoft.Json;
using Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace EasySaveGUI.ViewModels
{
    /// <summary>
    /// Client view model
    /// </summary>
    [DataContract]
    public class ClientViewModel : BaseViewModel
    {
        [DataMember]
        private CClient _Client;
        [DataMember]
        private JobViewModel _JobViewModel;

        /// <summary>
        /// Le job view model du client
        /// </summary>
        public JobViewModel JobVm { get => _JobViewModel; set => _JobViewModel = value; }
        /// <summary>
        /// Le client
        /// </summary>
        public CClient Client { get => _Client; set => _Client = value; }

        /// <summary>
        /// Initialise le client view model
        /// </summary>
        /// <param name="pClient">Le client</param>
        /// <param name="pJobViewModel">le job view model du client</param>
        public ClientViewModel(CClient pClient, JobViewModel pJobViewModel)
        {
            _Client = pClient;
            _JobViewModel = pJobViewModel;
        }

        /// <summary>
        /// Convertie ClientViewModel en Json
        /// </summary>
        /// <returns>string</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this); 
        }
    }
}
