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
    }
}
