using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
namespace LogsModels
{
    /// <summary>
    /// Log de base
    /// </summary>
    [DataContract]
    public abstract class CLogBase : IPath, INotifyPropertyChanged
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
        public virtual string Name
        {
            get => _Name; set { _Name = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// Date of the log
        /// </summary>
        public virtual DateTime Date
        {
            get => _Date; set { _Date = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// Total transfer file size
        /// </summary>
        public virtual double TotalSize
        {
            get => _TotalSize; set { _TotalSize = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// Source directory
        /// </summary>
        public virtual string SourceDirectory
        {
            get => _SourceDirectory;
            set { _SourceDirectory = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// Target directory
        /// </summary>
        public virtual string TargetDirectory
        {
            get => _TargetDirectory; set { _TargetDirectory = value; NotifyPropertyChanged(); }
        }

        /// <summary> Événement de modification d'une property </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary> Méthode à appeler pour avertir d'une modification </summary>
        /// <param name="propertyName">Nom de la property modifiée (automatiquement déterminé si appelé directement dans le setter une property) </param>
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
