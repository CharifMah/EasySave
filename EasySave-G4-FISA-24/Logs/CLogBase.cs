using System.Runtime.Serialization;

namespace LogsModels
{
    [DataContract]
    public class CLogBase : IPath
    {
        [DataMember]
        private DateTime _Date;
        [DataMember]
        private string _Name;
        [DataMember]
        private double _TotalSize;
        [DataMember]
        private string _SourceDirectory;
        [DataMember]
        private string _TargetDirectory;

        private bool _IsSummary;


        public DateTime Date { get => _Date; set => _Date = value; }
        public string Name { get => _Name; set => _Name = value; }
        public double TotalSize { get => _TotalSize; set => _TotalSize = value; }
        public string SourceDirectory { get => _SourceDirectory; set => _SourceDirectory = value; }
        public string TargetDirectory { get => _TargetDirectory; set => _TargetDirectory = value; }
        public bool IsSummary { get => _IsSummary; set => _IsSummary = value; }
    }
}
