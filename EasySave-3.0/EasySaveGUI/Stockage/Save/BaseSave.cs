using LogsModels;
using Newtonsoft.Json;
using Stockage.Logs;
using System.Threading;
using System.Xml.Serialization;
using static Stockage.Logs.ILogger<uint>;

namespace Stockage.Save
{
    /// <summary>
    /// Classe abstraite de base pour la sauvegarde d'un ficher ou le déplacement de Repertoire
    /// </summary>
    public abstract class BaseSave : ISauve
    {
        #region Attributes
        private string _path;

        private readonly JsonSerializerSettings _options;
        #endregion

        #region Property

        public JsonSerializerSettings Options => _options;

        public string FolderPath { get => _path; set => _path = value; }
        #endregion

        #region CTOR
        /// <summary>
        /// Sauvegarde
        /// </summary>
        /// <param name="pPath">Directory Path</param>
        public BaseSave(string pPath)
        {
            if (!string.IsNullOrEmpty(pPath) && !Directory.Exists(pPath) && !File.Exists(pPath) && !String.IsNullOrEmpty(Path.GetFileName(pPath)))
                Directory.CreateDirectory(pPath);
            _path = pPath;
            _options = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, NullValueHandling = NullValueHandling.Ignore };
        }

        #endregion

        #region Methods
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
                string lPath = String.Empty;
                string lDataString = "";

                if (string.IsNullOrEmpty(_path))
                    throw new ArgumentNullException("Path is null");
                if (!Directory.Exists(_path))
                    Directory.CreateDirectory(_path);
                if (pIsFullPath)
                    _path = pFileName;

                if (pExtention == "xml")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    StringWriter stringWriter = new StringWriter();
                    serializer.Serialize(stringWriter, pData);
                    lDataString = stringWriter.ToString();
                    stringWriter.Close();
                }
                else
                {
                    // cm - Serialize data to json
                    lDataString = JsonConvert.SerializeObject(pData, Formatting.Indented, Options);
                }

                if (!pIsFullPath)
                    lPath = Path.Combine(_path, $"{pFileName}.{pExtention}");
                else
                    lPath = _path;

                // cm - delete the file if exist
                if (!pAppend)
                {
                    // cm - Write json or xml data into the file
                    File.WriteAllText(lPath, lDataString);
                }
                if (pAppend)
                    File.AppendAllText(lPath, lDataString);

            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.Instance.StringLogger.Log(ex.Message, false);
            }
        }
        public virtual void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, ref CLogState pLogState, bool pDiffertielle = false)
        {
            throw new NotImplementedException();
        }
        public virtual void CopyDirectoryAsync(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, UpdateLogDelegate pUpdateLog, bool pRecursive, bool pDiffertielle = false, List<string>? pPriorityFileExtensions = null)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
