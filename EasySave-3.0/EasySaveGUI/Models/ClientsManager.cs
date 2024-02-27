namespace Models
{
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

        public HashSet<string> Clients { get => _Clients; set => _Clients = value; }

        private ClientsManager()
        {
            _Clients = new HashSet<string>();
        }
    }
}
