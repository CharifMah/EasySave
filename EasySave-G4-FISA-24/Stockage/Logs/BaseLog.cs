using System.Collections.ObjectModel;

namespace Stockage.Logs
{
    public abstract class BaseLogger<T> : ILogger<T>
    {
        private ObservableCollection<T> _Datas;

        public ObservableCollection<T> Datas => _Datas;

        protected BaseLogger()
        {
            _Datas = new ObservableCollection<T>();
        }

        public virtual void Log(T pData, bool pSerialize = true, bool pAppend = true, string pFileName = "Logs")
        {
            if (pSerialize)
            {
                string lFolderName = "Logs";
                string lPath = Path.Combine(Environment.CurrentDirectory, lFolderName);
                ISauve lSave = new SauveCollection(lPath);
                lSave.Sauver(pData, pFileName, pAppend);
            }

            _Datas.Add(pData);
        }

        public virtual void Clear()
        {
            _Datas.Clear();
        }
    }
}
