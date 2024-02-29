using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
namespace LogsModels
{
    /// <summary>
    /// Log de base
    /// </summary>
    [DataContract]
    public abstract class CLogBase : IPath, INotifyPropertyChanged, IDisposable
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
        [DataMember]
        private TimeSpan _EncryptTime;
        [DataMember]
        private double _Progress;

        private string _FormatLog;

        #region Property

        public double Progress
        {
            get
            {
                return _Progress;
            }
            set
            {
                _Progress = value;
                NotifyPropertyChanged();
            }
        }
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
        /// <summary>
        /// Format de log
        /// </summary>
        public string FormatLog { get => _FormatLog; set => _FormatLog = value; }
        /// <summary>
        /// Temps de chiffrement
        /// </summary>
        public TimeSpan EncryptTime
        {
            get => _EncryptTime; set { _EncryptTime = value; NotifyPropertyChanged(); }
        }


        #endregion

        ~CLogBase()
        {
            Dispose();
        }
        public override bool Equals(object? obj)
        {
            return obj is CLogBase @base &&
                   _Name == @base._Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_Name);
        }

        /// <summary> Événement de modification d'une property </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary> Méthode à appeler pour avertir d'une modification </summary>
        /// <param name="propertyName">Nom de la property modifiée (automatiquement déterminé si appelé directement dans le setter une property) </param>
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
    }
}
