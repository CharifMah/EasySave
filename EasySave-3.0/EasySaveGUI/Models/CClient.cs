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

        public string ConnectionId { get => _ConnectionId; set => _ConnectionId = value; }
    }
}
