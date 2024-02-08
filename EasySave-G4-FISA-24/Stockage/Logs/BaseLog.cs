using System.Collections.ObjectModel;

namespace Stockage.Logs
{
    public abstract class BaseLogger<T> : ILogger<T>
    {
        public abstract ObservableCollection<T> Datas { get; }
        public abstract void Log(T pData);
    }
}
