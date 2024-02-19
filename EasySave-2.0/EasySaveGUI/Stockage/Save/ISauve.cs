using LogsModels;

namespace Stockage.Save
{
    /// <summary>
    /// Interface ISauve
    /// </summary>
    /// <remarks>Mahmoud Charif - 31/12/2022 - Création</remarks>
    public interface ISauve
    {
        /// <summary>
        /// Sauvegarde les données dans un fichier
        /// </summary>
        /// <param name="pData">Data to serialize</param>
        /// <param name="pFileName">File name</param>
        /// <param name="pAppend">True si on veux append sur le fichier</param>
        /// <param name="pExtention">Extension</param>
        /// <param name="IsFullPath">vrai si pFileName est un chemin complet</param>
        /// <remarks>Mahmoud Charif - 31/12/2022 - Création</remarks>
        void Sauver<T>(T pData, string pFileName, bool pAppend = false, string pExtention = "json", bool IsFullPath = false);
        /// <summary>
        /// Copy files and directory from the source path to the destinationPath
        /// </summary>
        /// <param name="pSourceDir">Path of the directory you want tot copy</param>
        /// <param name="pTargetDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pDiffertielle">true if differential</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, ref CLogState pLogState, bool pDiffertielle = false);
    }
}
