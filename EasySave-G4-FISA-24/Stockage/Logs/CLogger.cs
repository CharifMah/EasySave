namespace Stockage.Logs
{
    public static class CLogger<T>
    {
        private static CGenericLogger<T> _GenericLogger = new CGenericLogger<T>();
        private static CStringLogger _StringLogger = new CStringLogger();

        public static CGenericLogger<T> GenericLogger { get => _GenericLogger; }
        public static CStringLogger StringLogger { get => _StringLogger; }
    }
}
