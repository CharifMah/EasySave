using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CClient
    {

        private string _Name;
        private string _ConnectionId;

        public string ConnectionId { get => _ConnectionId; set => _ConnectionId = value; }
    }
}
