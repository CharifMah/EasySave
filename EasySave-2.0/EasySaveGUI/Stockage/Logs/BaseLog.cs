using LogsModels;
using Stockage.Save;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Stockage.Logs
{
    /// <summary>
    /// Classe de base abstraite pour les loggers.
    /// </summary>
    /// <typeparam name="T">Type des objets loggés</typeparam>
    public abstract class BaseLogger<T> : ILogger<T>, INotifyCollectionChanged
    {
        private ObservableCollection<T> _Datas;
        /// <summary>
        /// Collection de données observables
        /// </summary>
        public ObservableCollection<T> Datas => _Datas;
        protected BaseLogger()
        {
            _Datas = new ObservableCollection<T>();
        }
        /// <summary>
        /// Méthode de logging des données
        /// </summary>
        /// <param name="pData">Données à logger</param>
        /// <param name="pSerialize">Indique si les données doivent être sérialisées avant d'être loggées</param>
        /// <param name="pAppend">Indique si on ajoute le logging au fichier existant ou si on recrée le fichier</param>
        /// <param name="pFileName">Nom du fichier où sont loggées les données</param>
        public virtual void Log(T pData, bool pSerialize = true, bool pAppend = true, string pFileName = "Logs")
        {
            if (pSerialize)
            {
                string lFolderName = "Logs";
                string lPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), lFolderName);
                ISauve lSave = new SauveCollection(lPath);
                lSave.Sauver(pData, pFileName, pAppend);
            }
            Datas.Add(pData);
            NotifyCollectionChanged();
        }
        /// <summary>
        /// Vide la collection de données
        /// </summary>
        public virtual void Clear()
        {
            _Datas.Clear();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void NotifyCollectionChanged([CallerMemberName] string propertyName = "")
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            CollectionChanged?.Invoke(this, args);
        }
    }
}
