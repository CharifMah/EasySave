namespace CryptoSoft
{
    /// <summary>
    /// Classe de base pour le chiffrement et le déchiffrement
    /// </summary>
    public abstract class BaseChiffrement
    {
        /// <summary>
        /// Chiffre les données
        /// </summary>
        /// <param name="data">donnée a chiffrer</param>
        /// <param name="key">clé pour le chiffrement</param>
        /// <returns>tableau d'octet</returns>
        public abstract byte[] Encrypt(byte[] data, byte[] key);
        /// <summary>
        /// Déchiffre les données
        /// </summary>
        /// <param name="data">donnée a déchiffrer</param>
        /// <param name="key">clé pour le déchiffrement</param>
        /// <returns>tableau d'octet</returns>
        public abstract byte[] Decrypt(byte[] data, byte[] key);
    }
}
