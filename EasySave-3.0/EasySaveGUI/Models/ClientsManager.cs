using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ClientsManager
    {
        private HashSet<CClient> _Clients;
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

        public HashSet<CClient> Clients { get => _Clients; set => _Clients = value; }

        private ClientsManager()
        {
            _Clients = new HashSet<CClient>();
        }
    }
}
