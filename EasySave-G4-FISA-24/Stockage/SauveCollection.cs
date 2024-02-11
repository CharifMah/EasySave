﻿using LogsModels;
using Newtonsoft.Json;
using Stockage.Logs;
using System.Diagnostics;
using System.Xml.Linq;

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
                    string jsonString = JsonConvert.SerializeObject(pData, Formatting.Indented, _options);
                    string lPath = Path.Combine(_path, $"{pFileName}.{pExtention}");

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

        /// <summary>
        /// Copy files and directory from the soruce path to the destinationPath
        /// </summary>
        /// <param name="pSourceDir">Path of the directory you want tot copy</param>
        /// <param name="pTargetDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pForce">true if overwrite</param>
        /// <param name="pLogger">Logger</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, bool pForce = false)
        {
            string lName = "Logs - " + DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                // cm - Check if the source directory exists
                if (!pSourceDir.Exists)
                    throw new DirectoryNotFoundException($"Source directory not found: {pSourceDir.FullName}");

                Directory.CreateDirectory(pTargetDir.FullName);
                FileInfo[] lFiles = pSourceDir.GetFiles();

                CLogDaily lLogFilesDaily = new CLogDaily();
                lLogFilesDaily.IsSummary = false;
                // cm - Get files in the source directory and copy to the destination directory
                for (int i = 0; i < lFiles.Length; i++)
                {
                    Stopwatch lSw = Stopwatch.StartNew();
                    lSw.Start();
                    string lTargetFilePath = Path.Combine(pTargetDir.FullName, lFiles[i].Name);

                    lFiles[i].CopyTo(lTargetFilePath, pForce);
                    lSw.Stop();

                    lLogFilesDaily.Name = lFiles[i].Name;
                    lLogFilesDaily.SourceDirectory = lFiles[i].FullName;
                    lLogFilesDaily.TargetDirectory = lTargetFilePath;
                    lLogFilesDaily.Date = DateTime.Now;
                    lLogFilesDaily.TotalSize = lFiles[i].Length;
                    lLogFilesDaily.TransfertTimeSecond = lSw.Elapsed.TotalSeconds;
                    CLogger<CLogBase>.GenericLogger.Log(lLogFilesDaily, true, true, lName);
                }

                // cm - If recursive and copying subdirectories, recursively call this method
                if (pRecursive)
                {
                    foreach (DirectoryInfo lSubDir in pSourceDir.GetDirectories())
                    {
                        DirectoryInfo lNewDestinationDir = pTargetDir.CreateSubdirectory(lSubDir.Name);
                        CopyDirectory(lSubDir, lNewDestinationDir, true, pForce);
                    }
                }
            }
            catch (Exception ex)
            {
                CLogger<CLogBase>.StringLogger.Log(ex.Message, false, true, lName);
            }
        }

        public long GetDirSize(string pPath)
        {
            try
            {
                return Directory.EnumerateFiles(pPath).Sum(x => new FileInfo(x).Length)
                    +
                       Directory.EnumerateDirectories(pPath).Sum(x => GetDirSize(x));
            }
            catch
            {
                return 0L;
            }
        }
    }
}
