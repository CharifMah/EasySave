namespace Stockage.Load
{
    /// <summary>
    /// Interface ICharge
    /// </summary>
    /// <remarks>Mahmoud Charif - 31/12/2022- Création</remarks>
    public interface ICharge
    {
        /// <summary>
        /// Charger un fichier
        /// </summary>
        /// <typeparam name="T">Type du fichier à charger</typeparam>
        /// <param name="pFileName">Nom du fichier</param>
        /// <param name="pIsFullPath">vrai si le premier parametre est un chemin complet et non le nom du fichier</param>
        /// <returns>Data Cast in Generic Type</returns>
        /// <remarks>Mahmoud Charif - 31/12/2022 - Creation</remarks>
        /// <remarks>Mahmoud Charif - 13/02/2024 - Ajout d'un parametre</remarks>
        T Charger<T>(string pPath, bool pIsFullPath = false);
    }
}
