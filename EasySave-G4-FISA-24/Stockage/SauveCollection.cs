﻿
using Logs;
using Newtonsoft.Json;
using Stockage.Logs;

namespace Stockage
{
    public class SauveCollection : ISauve
    {
        private string _path;

        private static readonly JsonSerializerSettings _options = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, NullValueHandling = NullValueHandling.Ignore };

        /// <summary>
        /// Sauvgarde
        /// </summary>
        /// <param name="pPath">Directory Path</param>
        public SauveCollection(string pPath)
        {
            if (!Directory.Exists(pPath))
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
        public void Sauver<T>(T pData, string pFileName, bool pAppend = false, string pExtention = "json")
        {
            try
            {
                // cm - Check if the directory exist
                if (Directory.Exists(_path))
                {
                    // cm - Serialize data to json
                    string jsonString = JsonConvert.SerializeObject(pData, _options);
                    string lPath = Path.Combine(_path, $"{pFileName}.{pExtention}");

                    // cm - delete the file if exist
                    if (!pAppend && File.Exists(lPath))
                    {
                        File.Delete(Path.Combine(_path, $"{pFileName}.{pExtention}"));

                        // cm - Write json data into the file
                        File.WriteAllText(lPath, jsonString);
                    }
                    else
                        File.AppendAllText(lPath, jsonString);                        
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Copy files and directory from the soruce path to the destinationPath
        /// </summary>
        /// <param name="pSourceDir">Path of the directory you want tot copy</param>
        /// <param name="pTargetDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pForce">true if overwrite</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void CopyDirectory(string pSourceDir, string pTargetDir, bool pRecursive, bool pForce = false, ILogger<CLogBase> pLogger = null)
        {
            var lDir = new DirectoryInfo(pSourceDir);

            // cm - Check if the source directory exists
            if (!lDir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {lDir.FullName}");

            DirectoryInfo[] lDirs = lDir.GetDirectories();
            FileInfo[] lFiles = lDir.GetFiles();

            CLogState lLogState = new CLogState();

            // cm - Get files in the source directory and copy to the destination directory
            foreach (FileInfo file in lFiles)
            {
                string lTargetFilePath = Path.Combine(pTargetDir, file.Name);

                if (File.Exists(lTargetFilePath))
                    if (pForce)
                        File.Delete(lTargetFilePath);

                file.CopyTo(lTargetFilePath);
                lLogState.Name = file.Name;
                lLogState.SourceDirectory = file.FullName;
                lLogState.TargetDirectory = lTargetFilePath;
                lLogState.EligibleFileCount = lFiles.Length;
                lLogState.TimeStamp = DateTime.Now;
                lLogState.TotalSize = 0;
                pLogger.Log(lLogState);
            }

            // cm - If recursive and copying subdirectories, recursively call this method
            if (pRecursive)
            {
                foreach (DirectoryInfo lSubDir in lDirs)
                {
                    string lNewDestinationDir = Path.Combine(pTargetDir, lSubDir.Name);
                    CopyDirectory(lSubDir.FullName, lNewDestinationDir, true, pForce, pLogger);
                }
            }
        }
    }
}
