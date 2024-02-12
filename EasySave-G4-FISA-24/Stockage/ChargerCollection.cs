using Newtonsoft.Json;
namespace Stockage
{
    public class ChargerCollection : ICharge
    {
        private static readonly JsonSerializerSettings _options = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, NullValueHandling = NullValueHandling.Ignore };

        private string _Path;

        /// <summary>
        /// Charger un object 
        /// </summary>
        /// <param name="pPath">Chemin du fichier</param>
        /// <Author>Charif</Author>
        public ChargerCollection(string pPath)
        {
            this._Path = pPath;
        }

        /// <summary>
        /// Charger un fichier
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="pFileName">chemin du fichier</param>
        /// <returns>Data Cast in Generic Type</returns>
        /// <remarks>Mahmoud Charif - 31/12/2022 - Creation</remarks>
        public T Charger<T>(string pFileName)
        {
            T d2 = default;
            string lPath;
            if (_Path == "")
                lPath = pFileName;
            else
                lPath = Path.Combine(_Path, $"{pFileName}.json");

            try
            {
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