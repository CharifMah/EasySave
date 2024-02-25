using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// Business Software class
    /// </summary>
    [DataContract]
    public class CBusinessSoftware
    {
        [DataMember]
        public string Name { get; set; }

        public CBusinessSoftware(string pName)
        {
            Name = pName;
        }
    }
}
