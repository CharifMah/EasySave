using System.Collections.ObjectModel;

namespace Stockage.Logs
{
    /// <summary>
    /// BaseLogger
    /// </summary>
    /// <typeparam name="T">Type du Logger</typeparam>
    public abstract class BaseLogger<T> : ILogger<T>
    {
        private ObservableCollection<T> _Datas;

        public ObservableCollection<T> Datas => _Datas;

        protected BaseLogger()
        {
            _Datas = new ObservableCollection<T>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pSerialize"></param>
        /// <param name="pAppend"></param>
        /// <param name="pFileName"></param>
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
        /// <summary>
        /// 
        /// </summary>
        public virtual void Clear()
        {
            _Datas.Clear();
        }
    }
}
