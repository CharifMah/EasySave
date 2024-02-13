namespace Stockage.Logs
{
    /// <summary>
    /// Classe Logger permettant de Logger des objet et des string dans un fichier
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class CLogger<T>
    {
        private static CGenericLogger<T> _GenericLogger = new CGenericLogger<T>();
        private static CStringLogger _StringLogger = new CStringLogger();
        /// <summary>
        /// Logger generic
        /// </summary>
        public static CGenericLogger<T> GenericLogger { get => _GenericLogger; }
        /// <summary>
        /// Logger de string
        /// </summary>
        public static CStringLogger StringLogger { get => _StringLogger; }

        /// <summary>
        /// Vide les Liste de logs
        /// </summary>
        public static void Clear()
        {
            _GenericLogger.Clear();
            _StringLogger.Clear();
        }
    }
}
