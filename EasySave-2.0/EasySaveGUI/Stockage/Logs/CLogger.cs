namespace Stockage.Logs
{
    /// <summary>
    /// Classe Logger permettant de Logger des objet et des string dans un fichier
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CLogger<T>
    {
        private CGenericLogger<T> _GenericLogger;
        private CStringLogger _StringLogger;

        /// <summary>
        /// Logger generic
        /// </summary>
        public CGenericLogger<T> GenericLogger { get => _GenericLogger; }
        /// <summary>
        /// Logger de string
        /// </summary>
        public CStringLogger StringLogger { get => _StringLogger; }


        #region CTOR
        private static CLogger<T> _Instance;
        public static CLogger<T> Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (typeof(CLogger<T>))
                    {
                        if (_Instance == null)
                        {
                            _Instance = new CLogger<T>();
                        }
                    }
                }
                return _Instance;
            }
        }
        private CLogger()
        {
            _GenericLogger = new CGenericLogger<T>();
            _StringLogger = new CStringLogger();
        }
        #endregion
        /// <summary>
        /// Vide les Liste de logs
        /// </summary>
        public void Clear()
        {
            _GenericLogger.Clear();
            _StringLogger.Clear();
        }
    }
}
