using LogsModels;
using Newtonsoft.Json;
using Stockage.Logs;
using System.Xml.Serialization;

namespace Stockage.Save
{
    /// <summary>
    /// Classe abstraite de base pour la sauvegarde d'un ficher ou le déplacement de Repertoire
    /// </summary>
    public abstract class BaseSave : ISauve
    {
        private string _path;

        private readonly JsonSerializerSettings _options = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, NullValueHandling = NullValueHandling.Ignore };

        public JsonSerializerSettings Options => _options;

        /// <summary>
        /// Sauvegarde
        /// </summary>
        /// <param name="pPath">Directory Path</param>
        public BaseSave(string pPath)
        {
            if (!string.IsNullOrEmpty(pPath) && !Directory.Exists(pPath) && !File.Exists(pPath) && !String.IsNullOrEmpty(Path.GetFileName(pPath)))
                Directory.CreateDirectory(pPath);
            _path = pPath;
        }
        /// <summary>
        /// Crée un fichier Json par default avec les Settings
        /// </summary>
        /// <param name="pData">Data a sauvegarde</param>
        /// <param name="pFileName">Name of the file</param>
        /// <param name="pExtention">Extension of the file can be null</param>
        /// <param name="pIsFullPath">vrai si le premier parametre est un chemin complet et non le nom du fichier</param>
        /// <Author>Mahmoud Charif - 31/12/2022 - Création</Author>
        public virtual void Sauver<T>(T pData, string pFileName, bool pAppend = false, string pExtention = "json", bool pIsFullPath = false)
        {
            try
            {
                string lPath;

                if (string.IsNullOrEmpty(_path))
                    throw new ArgumentNullException("Path is null");

                if (pIsFullPath)
                    _path = pFileName;

                // cm - Check if the directory exist
                if (Directory.Exists(_path) || pIsFullPath)
                {
                    string dataString = "";

                    if (pExtention == "xml")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        StringWriter stringWriter = new StringWriter();
                        serializer.Serialize(stringWriter, pData);
                        dataString = stringWriter.ToString();
                        stringWriter.Close();
                    }
                    else
                    {
                        // cm - Serialize data to json
                        dataString = JsonConvert.SerializeObject(pData, Formatting.Indented, Options);
                    }

                    if (!pIsFullPath)
                        lPath = Path.Combine(_path, $"{pFileName}.{pExtention}");
                    else
                        lPath = _path;

                    // cm - delete the file if exist
                    if (!pAppend)
                    {
                        // cm - Write json or xml data into the file
                        File.WriteAllText(lPath, dataString);
                    }
                    if (pAppend)
                        File.AppendAllText(lPath, dataString);
                }
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }
        public virtual void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, bool pForce = false)
        {
            throw new NotImplementedException();
        }

        public virtual void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, ref CLogState pLogState, bool pDiffertielle = false)
        {
            throw new NotImplementedException();
        }
        public virtual async Task CopyDirectoryAsync(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, bool pDiffertielle = false)
        {
            throw new NotImplementedException();
        }
    }
}
