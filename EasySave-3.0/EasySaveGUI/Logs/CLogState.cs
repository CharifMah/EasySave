﻿using System.Runtime.Serialization;
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
        private TimeSpan _Elapsed;
        [DataMember]
        private bool _IsActive;
        [DataMember]
        private double _BytesCopied;
        [DataMember]
        private int _TotalTransfered;
        [DataMember]
        private bool _IsPaused;
        [DataMember]
        private bool _IsStopped;
        [DataMember]
        private bool _IsStarted;

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
        public TimeSpan Elapsed
        {
            get => _Elapsed; set { _Elapsed = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Indique si le job est actif ou non
        /// </summary>
        public bool IsActive
        {
            get => _IsActive; set { _IsActive = value; NotifyPropertyChanged(); }
        }

        public double BytesCopied { get => _BytesCopied; set { _BytesCopied = value; NotifyPropertyChanged(); } }
        /// <summary>
        /// Le nombre de fichier transférer
        /// </summary>
        public int TotalTransferedFile { get => _TotalTransfered; set { _TotalTransfered = value; NotifyPropertyChanged(); } }

        public bool IsPaused
        {
            get => _IsPaused; set { _IsPaused = value; NotifyPropertyChanged(); }
        }
        public bool IsStopped { get => _IsStopped; set { _IsStopped = value; NotifyPropertyChanged(); } }
        public bool IsStarted { get => _IsStarted; set { _IsStarted = value; NotifyPropertyChanged(); } }



        /// <summary>
        /// Constructeur de CLogState
        /// </summary>
        public CLogState()
        {
            Name = "EasySaveLogState - " + Name;
        }

        /// <summary>
        /// Reprend les jobs selectionnée en cours
        /// </summary>
        public void Resume()
        {
            _IsStarted = false;
            if (_IsPaused)
            {
                _IsStarted = true;
            }
        }
        /// <summary>
        /// Met en pause les jobs
        /// </summary>
        public void Pause()
        {
            _IsPaused = true;
        }

        /// <summary>
        /// Arrete definitivement les jobs
        /// </summary>
        public void Stop()
        {
            _IsStopped = true;
            _IsPaused = false;
            _IsStarted = false;
        }
    }
}
