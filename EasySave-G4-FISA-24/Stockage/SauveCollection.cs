
using Newtonsoft.Json;

namespace Stockage
{
    public class SauveCollection : ISauve
    {
        private string _path;

        /// <summary>
        /// Sauvgarde
        /// </summary>
        /// <param name="pPath">Directory Path</param>
        public SauveCollection(string pPath)
        {
            _path = pPath;
        }

        private static readonly JsonSerializerSettings _options = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, NullValueHandling = NullValueHandling.Ignore };

        /// <summary>
        /// Crée un fichier Json par default avec les Settings
        /// </summary>
        /// <param name="pData">Data a sauvgarde</param>
        /// <param name="pFileName">Name of the file</param>
        /// <param name="pExtention">Extention of the file can be null</param>
        /// <Author>Mahmoud Charif - 31/12/2022 - Création</Author>
        public void Sauver<T>(T pData, string pFileName, string pExtention = "json")
        {
            try
            {
                // cm - Check if the directory exist
                if (Directory.Exists(_path))
                {
                    // cm - delete the file if exist
                    if (File.Exists(Path.Combine(_path, $"{pFileName}.{pExtention}")))
                        File.Delete(Path.Combine(_path, $"{pFileName}.{pExtention}"));
                    // cm - Serialize data to json
                    string jsonString = JsonConvert.SerializeObject(pData, _options);
                    // cm - Write json data into the file
                    File.WriteAllText(Path.Combine(_path, $"{pFileName}.{pExtention}"), jsonString);
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
        /// <param name="pDestinationDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pForce">true if overwrite</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void CopyDirectory(string pSourceDir, string pDestinationDir, bool pRecursive, bool pForce = false)
        {
            var lDir = new DirectoryInfo(pSourceDir);

            // cm - Check if the source directory exists
            if (!lDir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {lDir.FullName}");

            DirectoryInfo[] lDirs = lDir.GetDirectories();

            // cm - Get files in the source directory and copy to the destination directory
            foreach (FileInfo file in lDir.GetFiles())
            {
                string lTargetFilePath = Path.Combine(pDestinationDir, file.Name);

                if (File.Exists(lTargetFilePath))
                    if (pForce)
                        File.Delete(lTargetFilePath);

                file.CopyTo(lTargetFilePath);
            }

            // cm - If recursive and copying subdirectories, recursively call this method
            if (pRecursive)
            {
                foreach (DirectoryInfo lSubDir in lDirs)
                {
                    string lNewDestinationDir = Path.Combine(pDestinationDir, lSubDir.Name);
                    CopyDirectory(lSubDir.FullName, lNewDestinationDir, true, pForce);
                }
            }
        }
    }
}
