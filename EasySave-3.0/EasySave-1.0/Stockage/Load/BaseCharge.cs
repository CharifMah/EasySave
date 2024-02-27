using Newtonsoft.Json;

namespace Stockage.Load
{
    /// <summary>
    /// Classe abstraite de base pour le chargement d'un object
    /// </summary>
    public abstract class BaseCharge : ICharge
    {
        private static readonly JsonSerializerSettings _options = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, NullValueHandling = NullValueHandling.Ignore };

        private string _Path;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="pPath">Chemin du dossier</param>
        /// <remarks>Mahmoud Charif - 13/02/2024 - Création</remarks>
        public BaseCharge(string pPath)
        {
            _Path = pPath;
        }

        /// <summary>
        /// Charger un fichier
        /// </summary>
        /// <typeparam name="T">Type du fichier à charger</typeparam>
        /// <param name="pFileName">Nom du fichier</param>
        /// <param name="pIsFullPath">vrai si le premier parametre est un chemin complet et non le nom du fichier</param>
        /// <returns>Data Cast in Generic Type</returns>
        /// <remarks>Mahmoud Charif - 31/12/2022 - Creation</remarks>
        public virtual T Charger<T>(string pFileName, bool pIsFullPath = false)
        {
            T d2 = default;

            try
            {
                string lPath = String.Empty;

                if (pIsFullPath)
                    lPath = pFileName;
                else
                    lPath = Path.Combine(_Path, $"{pFileName}.json");


                if (File.Exists(lPath))
                    // read file into a string and deserialize JSON to a type
                    d2 = JsonConvert.DeserializeObject<T>(File.ReadAllText(lPath), _options);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return d2;
        }
    }
}
