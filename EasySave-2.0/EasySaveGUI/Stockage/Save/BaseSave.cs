﻿using LogsModels;
using Newtonsoft.Json;

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
        /// <Author>Mahmoud Charif - 31/12/2022 - Création</Author>
        public virtual void Sauver<T>(T pData, string pFileName, bool pAppend = false, string pExtention = "json", bool IsFullPath = false)
        {
            try
            {
                string lPath;

                if (string.IsNullOrEmpty(_path))
                    throw new ArgumentNullException("Path is null");

                if (IsFullPath)
                    _path = pFileName;

                // cm - Check if the directory exist
                if (Directory.Exists(_path) || IsFullPath)
                {

                    // cm - Serialize data to json
                    string jsonString = JsonConvert.SerializeObject(pData, Formatting.Indented, Options);

                    if (!IsFullPath)
                        lPath = Path.Combine(_path, $"{pFileName}.{pExtention}");
                    else
                        lPath = _path;

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