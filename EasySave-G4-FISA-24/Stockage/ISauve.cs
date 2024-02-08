using Logs;
using Stockage.Logs;

namespace Stockage
{
    /// <summary>
    /// Interface ISauve
    /// </summary>
    /// <remarks>Mahmoud Charif - 31/12/2022 - Création</remarks>
    public interface ISauve
    {
        /// <summary>
        /// Sauvagarde le dictionnaire
        /// </summary>
        /// <param name="pData">Data to serialize</param>
        /// <param name="pFileName">File name</param>
        /// <param name="pExtention">Extention</param>
        /// <remarks>Mahmoud Charif - 31/12/2022 - Création</remarks>
        void Sauver<T>(T pData, string pFileName, bool pAppend = false, string pExtention = "json");

        /// <summary>
        /// Copy files and directory from the soruce path to the destinationPath
        /// </summary>
        /// <param name="pSourceDir">Path of the directory you want tot copy</param>
        /// <param name="pTargetDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pForce">true if overwrite</param>
        /// <param name="pLogger">Class de logger</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        void CopyDirectory(string pSourceDir, string pTargetDir, bool pRecursive, bool pForce = false, CLogger<CLogBase> pLogger = null);
    }
}
