using Newtonsoft.Json;

namespace Models
{
    /// <summary>
    /// Singleton ClientManager du serveur sauvegarde l'etat des clients dans une liste
    /// </summary>
    public class ClientsManager
    {
        private HashSet<string> _Clients;
        private static ClientsManager _Instance;
        public static ClientsManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ClientsManager();
                }
                return _Instance;
            }
        }

        private ClientsManager()
        {
            _Clients = new HashSet<string>();
        }

        public HashSet<string> Clients { get => _Clients; set => _Clients = value; }



        public void UpdateClient(string pClientViewModel, string pConnectionId)
        {
            string? lClientVmJson = _Clients.FirstOrDefault(cl => cl.Contains(pConnectionId));
            if (string.IsNullOrEmpty(lClientVmJson))
                _Clients.Add(pClientViewModel);
            else
            {
                _Clients.Remove(lClientVmJson);
                _Clients.Add(pClientViewModel);
            }
        }
        /// <summary>
        /// CLient to json
        /// </summary>
        /// <returns>List of clients in json</returns>
        public string ToJson()
        {
           return JsonConvert.SerializeObject(_Clients, Formatting.Indented);
        }
    }
}
