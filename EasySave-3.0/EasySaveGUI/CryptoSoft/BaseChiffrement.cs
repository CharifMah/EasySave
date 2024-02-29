namespace CryptoSoft
{
    /// <summary>
    /// Classe de base pour le chiffrement et le déchiffrement
    /// </summary>
    public abstract class BaseChiffrement
    {
        private TimeSpan _EncryptTime;
        private TimeSpan _DecryptTime;

        public virtual TimeSpan EncryptTime { get => _EncryptTime; set => _EncryptTime = value; }
        public virtual TimeSpan DecryptTime { get => _DecryptTime; set => _DecryptTime = value; }

        /// <summary>
        /// Chiffre les données
        /// </summary>
        /// <param name="pData">donnée a chiffrer</param>
        /// <param name="pKey">clé pour le chiffrement</param>
        /// <returns>tableau d'octet</returns>
        public abstract byte[] Encrypt(byte[] pData, byte[] pKey);
        /// <summary>
        /// Déchiffre les données
        /// </summary>
        /// <param name="pData">donnée a déchiffrer</param>
        /// <param name="pKey">clé pour le déchiffrement</param>
        /// <returns>tableau d'octet</returns>
        public abstract byte[] Decrypt(byte[] pData, byte[] pKey);
    }
}
