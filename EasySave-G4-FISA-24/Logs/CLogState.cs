using System.Runtime.Serialization;

namespace LogsModels
{
    [DataContract]
    public class CLogState : CLogBase
    {

        [DataMember]
        private double _RemainingSize;
        [DataMember]
        private int _EligibleFileCount;
        [DataMember]
        private long _ElapsedMilisecond;

        public double RemainingSize { get => _RemainingSize; set => _RemainingSize = value; }
        public int EligibleFileCount { get => _EligibleFileCount; set => _EligibleFileCount = value; }
        public long ElapsedMilisecond { get => _ElapsedMilisecond; set => _ElapsedMilisecond = value; }
        public override string Name { get => base.Name; set => base.Name = "Summary - " + value; }

        public CLogState()
        {
            Name = "Summary - " + Name ;
            IsSummary = true;
        }
    }
}
