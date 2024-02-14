using System.Runtime.Serialization;
namespace LogsModels
{
    /// <summary>
    /// Log de base
    /// </summary>
    [DataContract]
    public abstract class CLogBase : IPath
    {
        [DataMember]
        private string _Name;
        [DataMember]
        private DateTime _Date;
        [DataMember]
        private double _TotalSize;
        [DataMember]
        private string _SourceDirectory;
        [DataMember]
        private string _TargetDirectory;

        public virtual string Name { get => _Name; set => _Name = value; }
        public virtual DateTime Date { get => _Date; set => _Date = value; }
        public virtual double TotalSize { get => _TotalSize; set => _TotalSize = value; }
        public virtual string SourceDirectory { get => _SourceDirectory; set => _SourceDirectory = value; }
        public virtual string TargetDirectory { get => _TargetDirectory; set => _TargetDirectory = value; }
    }
}
