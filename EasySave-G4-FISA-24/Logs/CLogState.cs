using System.Runtime.Serialization;

namespace LogsModels
{
    [DataContract]
    public class CLogState : CLogBase
    {
        [DataMember]
        private int _RemainingFiles;
        [DataMember]
        private int _EligibleFileCount;
        [DataMember]
        private long _ElapsedMilisecond;
        [DataMember]
        private bool _IsActive;

        public override string Name
        {
            get => base.Name;
            set
            {
                if (IsSummary)
                    base.Name = "Summary - " + value;
                else
                    base.Name = value;
            }
        }
        public int RemainingFiles { get => _RemainingFiles; set => _RemainingFiles = value; }
        public int EligibleFileCount { get => _EligibleFileCount; set => _EligibleFileCount = value; }
        public long ElapsedMilisecond { get => _ElapsedMilisecond; set => _ElapsedMilisecond = value; }
        public bool IsActive { get => _IsActive; set => _IsActive = value; }

        public CLogState()
        {
            Name = "Summary - " + Name;
            IsSummary = true;
        }
    }
}
