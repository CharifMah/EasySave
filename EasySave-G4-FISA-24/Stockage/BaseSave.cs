using LogsModels;
using Newtonsoft.Json;
namespace Stockage
{
    public abstract class BaseSave : ISauve
    {
        private string _path;
        private readonly JsonSerializerSettings _options = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, NullValueHandling = NullValueHandling.Ignore };
        public string FullPath { get => _path; set => _path = value; }
        public JsonSerializerSettings Options => _options;
        /// <summary>
        /// Sauvgarde
        /// </summary>
        /// <param name="pPath">Directory Path</param>
        public BaseSave(string pPath)
        {
            if (!String.IsNullOrEmpty(pPath) && !Directory.Exists(pPath))
                Directory.CreateDirectory(pPath);
            _path = pPath;
        }
        /// <summary>
        /// Crée un fichier Json par default avec les Settings
        /// </summary>
        /// <param name="pData">Data a sauvgarde</param>
        /// <param name="pFileName">Name of the file</param>
        /// <param name="pExtention">Extention of the file can be null</param>
        /// <Author>Mahmoud Charif - 31/12/2022 - Création</Author>
        public virtual void Sauver<T>(T pData, string pFileName, bool pAppend = false, string pExtention = "json")
        {
            try
            {
                // cm - Check if the directory exist
                if (Directory.Exists(FullPath))
                {
                    // cm - Serialize data to json
                    string jsonString = JsonConvert.SerializeObject(pData, Formatting.Indented, Options);
                    string lPath = Path.Combine(FullPath, $"{pFileName}.{pExtention}");
                    // cm - delete the file if exist
                    if (!pAppend)
                    {
                        // cm - Write json data into the file
                        File.WriteAllText(lPath, jsonString);
                    }
                    if (pAppend)
                        File.AppendAllText(lPath, jsonString);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public virtual void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, bool pForce = false)
        {
            throw new NotImplementedException();
        }
        public virtual void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, ref CLogState pLogState, bool pForce = false)
        {
            throw new NotImplementedException();
        }
    }
}
