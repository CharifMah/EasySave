using System.Collections.ObjectModel;
namespace Stockage.Logs
{
    /// <summary>
    /// Interface ILogger
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogger<T>
    {
        /// <summary>
        /// Collection de données observables
        /// </summary>
        ObservableCollection<T> Datas { get; }

        /// <summary>
        /// Méthode de logging des données
        /// </summary>
        /// <param name="pData">Données à logger</param>
        /// <param name="pSerialize">Indique si les données doivent être sérialisées avant d'être loggées</param>
        /// <param name="pAppend">Indique si on ajoute le logging au fichier existant ou si on recrée le fichier</param>
        /// <param name="pFileName">Nom du fichier où sont loggées les données</param>
        /// <remarks>Mahmoud Charif - 10/02/2024 - Création</remarks>
        void Log(T pData, bool pSerialize, bool pAppend = true, string pFileName = "Logs");
    }
}