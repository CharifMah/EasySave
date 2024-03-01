using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [DataContract]
    public class CClient
    {
        [DataMember]
        private string _Name;
        [DataMember]
        private string _ConnectionId;
        [DataMember]
        private string _ClientId;

        public string ConnectionId { get => _ConnectionId; set => _ConnectionId = value; }
        public string ClientId { get => _ClientId; set => _ClientId = value; }
    }
}
