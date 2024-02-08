using System.Runtime.Serialization;

namespace Logs
{
    [DataContract]
    public class CLogBase : IPath
    {
        [DataMember]
        private DateTime _TimeStamp;
        [DataMember]
        private string _Name;
        [DataMember]
        private double _TotalSize;
        [DataMember]
        private string _SourceDirectory;
        [DataMember]
        private string _TargetDirectory;


        public DateTime TimeStamp { get => _TimeStamp; set => _TimeStamp = value; }
        public string Name { get => _Name; set => _Name = value; }
        public double TotalSize { get => _TotalSize; set => _TotalSize = value; }
        public string SourceDirectory { get => _SourceDirectory; set => _SourceDirectory = value; }
        public string TargetDirectory { get => _TargetDirectory; set => _TargetDirectory = value; }

        public override string? ToString()
        {
            return $" - {_TimeStamp.Date} - {_Name} - {_TotalSize}";
        }
    }
}
