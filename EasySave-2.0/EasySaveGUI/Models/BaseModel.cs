﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Models
{
    public abstract class BaseModel : INotifyPropertyChanged
    {
        /// <summary> Événement de modification d'une property </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary> Méthode à appeler pour avertir d'une modification </summary>
        /// <param name="propertyName">Nom de la property modifiée (automatiquement déterminé si appelé directement dans le setter une property) </param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
