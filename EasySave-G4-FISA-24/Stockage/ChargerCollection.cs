
using Newtonsoft.Json;

namespace Stockage
{
    public class ChargerCollection : ICharge
    {
        private static readonly JsonSerializerSettings _options = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, NullValueHandling = NullValueHandling.Ignore };

        /// <summary>
        /// Charger un fichier
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="pPath">chemin du fichier</param>
        /// <returns>Data Cast in Generic Type</returns>
        /// <remarks>Mahmoud Charif - 31/12/2022 - Creation</remarks>
        public T Charger<T>(string pPath)
        {
            T d2 = default;

            try
            {
                if (File.Exists(pPath))
                    // read file into a string and deserialize JSON to a type
                    d2 = JsonConvert.DeserializeObject<T>(File.ReadAllText(pPath), _options);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return d2;
        }
    }
}