using Stockage.Save;
using System.Collections.ObjectModel;

namespace Stockage.Logs
{
    /// <summary>
    /// Classe de base abstraite pour les loggers.
    /// </summary>
    /// <typeparam name="T">Type des objets loggés</typeparam>
    public abstract class BaseLogger<T> : ILogger<T>
    {
        private ObservableCollection<T> _Datas;
        private SauveCollection _Save;
        /// <summary>
        /// Collection de données observables
        /// </summary>
        public ObservableCollection<T> Datas => _Datas;
        protected BaseLogger()
        {
            _Datas = new ObservableCollection<T>();
            _Save = new SauveCollection("");
        }
        /// <summary>
        /// Méthode de logging des données
        /// </summary>
        /// <param name="pData">Données à logger</param>
        /// <param name="pSerialize">Indique si les données doivent être sérialisées avant d'être loggées</param>
        /// <param name="pAppend">Indique si on ajoute le logging au fichier existant ou si on recrée le fichier</param>
        /// <param name="pFileName">Nom du fichier où sont loggées les données</param>
        public virtual void Log(T pData, bool pSerialize = true, bool pAppend = true, string pFileName = "Logs", string pFolderName = "", string pExtension = "json")
        {
            if (pSerialize)
            {
                string lAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string lLogsFolder = Path.Combine(lAppDataFolder, "EasySave");

                _Save.FolderPath = string.IsNullOrEmpty(pFolderName) ? lLogsFolder : Path.Combine(lLogsFolder, pFolderName);
                _Save.Sauver(pData, pFileName, pAppend, pExtension);
            }
            if (!pAppend)
                _Datas = new ObservableCollection<T>();

            _Datas.Add(pData);
        }
        /// <summary>
        /// Vide la collection de données
        /// </summary>
        public virtual void Clear()
        {
            _Datas.Clear();
        }
    }
}
