using System.Runtime.Serialization;

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
