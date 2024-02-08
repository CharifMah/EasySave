using System.Collections.ObjectModel;

namespace Stockage.Logs
{
    public interface ILogger<T>
    {
        ObservableCollection<T> Datas { get; }

        void Log(T pData);
    }
}