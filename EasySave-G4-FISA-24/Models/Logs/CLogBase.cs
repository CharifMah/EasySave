using Models.Backup;
using System.Runtime.Serialization;

namespace Models.Logs
{
    [DataContract]
    public class CLogBase
    {
        [DataMember]
        private DateTime _TimeStamp;
        [DataMember]
        private string _Name;
        [DataMember]
        private double _TotalSize;
        [DataMember]
        private CJob _Job;


        public DateTime TimeStamp { get => _TimeStamp; set => _TimeStamp = value; }
        public string Name { get => _Name; set => _Name = value; }
        public double TotalSize { get => _TotalSize; set => _TotalSize = value; }
        public CJob Job { get => _Job; set => _Job = value; }
    }
}
