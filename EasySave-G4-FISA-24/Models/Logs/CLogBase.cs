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

        protected DateTime TimeStamp { get => _TimeStamp; set => _TimeStamp = value; }
        protected string Name { get => _Name; set => _Name = value; }
        protected double TotalSize { get => _TotalSize; set => _TotalSize = value; }


    }
}
