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

        /// <summary>
        /// Name of the Log
        /// </summary>
        public virtual string Name { get => _Name; set => _Name = value; }
        /// <summary>
        /// Date of the log
        /// </summary>
        public virtual DateTime Date { get => _Date; set => _Date = value; }
        /// <summary>
        /// Total transfer file size
        /// </summary>
        public virtual double TotalSize { get => _TotalSize; set => _TotalSize = value; }
        /// <summary>
        /// Source directory
        /// </summary>
        public virtual string SourceDirectory { get => _SourceDirectory; set => _SourceDirectory = value; }
        /// <summary>
        /// Target directory
        /// </summary>
        public virtual string TargetDirectory { get => _TargetDirectory; set => _TargetDirectory = value; }
    }
}
