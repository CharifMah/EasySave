using System.Collections.ObjectModel;

namespace Stockage.Logs
{
    public abstract class BaseLogger<T> : ILogger<T>
    {
        private ObservableCollection<T> _Datas = new ObservableCollection<T>();

        public ObservableCollection<T> Datas => _Datas;

        public void Log(T pData,string pFileName = "Logs")
        {
            ISauve lSave = new SauveCollection(Environment.CurrentDirectory);
            lSave.Sauver(pData, pFileName, true);
            _Datas.Add(pData);
        }
    }
}
