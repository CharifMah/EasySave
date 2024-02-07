using System.Runtime.Serialization;

namespace Models.Logs
{
    [DataContract]
    public class CLogState : CLogBase
    {

        [DataMember]
        private double _RemainingSize;
        [DataMember]
        private int _EligibleFileCount;

        public double RemainingSize { get => _RemainingSize; set => _RemainingSize = value; }
        public int EligibleFileCount { get => _EligibleFileCount; set => _EligibleFileCount = value; }
    }
}
