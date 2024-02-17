using System.Runtime.Serialization;
namespace LogsModels
{
    /// <summary>
    /// Classe de journal d'état représentant l'état de transfert d'une liste de fichiers
    /// </summary>
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

        private double _BytesCopied;

        private double _Progress;

        public override string Name
        {
            get => base.Name;
            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Nombre de fichier restant
        /// </summary>
        public int RemainingFiles
        {
            get => _RemainingFiles; set { _RemainingFiles = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Nombre de fichier eligible au déplacement (Nombre de fichier Total)
        /// </summary>
        public int EligibleFileCount
        {
            get => _EligibleFileCount; set { _EligibleFileCount = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Nombre de millisecondes écoulées
        /// </summary>
        public long ElapsedMilisecond
        {
            get => _ElapsedMilisecond; set { _ElapsedMilisecond = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Indique si le job est actif ou non
        /// </summary>
        public bool IsActive
        {
            get => _IsActive; set { _IsActive = value; NotifyPropertyChanged(); }
        }

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

        public double BytesCopied { get => _BytesCopied; set { _BytesCopied = value; NotifyPropertyChanged(); } }

        /// <summary>
        /// Constructeur de CLogState
        /// </summary>
        public CLogState()
        {
            Name = "EasySaveLogState - " + Name;
        }
    }
}
