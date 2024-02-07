namespace Stockage
{
    /// <summary>
    /// Interface sur un chargeur de dictionnaire
    /// </summary>
    /// <remarks>Mahmoud Charif - 31/12/2022- Création</remarks>
    public interface ICharge
    {
        /// <summary>
        /// Charge le dictionnaire
        /// </summary>
        /// <param name="pPath">Complete path of the file with extention</param>
        /// <returns>Loaded file</returns>
        /// <remarks>Mahmoud Charif - 31/12/2022 - Creation </remarks>
        T Charger<T>(string pPath);
    }
}
