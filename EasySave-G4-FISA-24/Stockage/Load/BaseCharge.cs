using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockage.Load
{
    public abstract class BaseCharge : ICharge
    {
        private static readonly JsonSerializerSettings _options = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, NullValueHandling = NullValueHandling.Ignore };

        private string _Path;

        /// <summary>
        /// Charger un object 
        /// </summary>
        /// <param name="pPath">Chemin du fichier</param>
        /// <Author>Charif</Author>
        public BaseCharge(string pPath)
        {
            _Path = pPath;
        }

        /// <summary>
        /// Charger un fichier
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="pFileName">chemin du fichier or full path if the attribute _Path is not null</param>
        /// <param name="pIsFullPath">allow to put full path in the first param</param>
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
