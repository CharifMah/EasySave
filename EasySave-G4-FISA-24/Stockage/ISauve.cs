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
        void Sauver<T>(T pData, string pFileName, string pExtention = "json");

        /// <summary>
        /// Copy files and directory from the soruce path to the destinationPath
        /// </summary>
        /// <param name="pSourceDir">Path of the directory you want tot copy</param>
        /// <param name="pDestinationDir">Path of the target directory</param>
        /// <param name="pRecursive">True if recursive</param>
        /// <param name="pForce">true if overwrite</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        void CopyDirectory(string pSourceDir, string pDestinationDir, bool pRecursive, bool pForce = false);
    }
}
