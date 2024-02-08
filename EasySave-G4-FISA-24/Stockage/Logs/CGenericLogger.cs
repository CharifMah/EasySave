using System.Collections.ObjectModel;

namespace Stockage.Logs
{
    public class CGenericLogger<T> : BaseLogger<T>
    {
        private ObservableCollection<T> _Datas;
        public override ObservableCollection<T> Datas
        {
            get
            {
                return _Datas;
            }
        }

        public CGenericLogger()
        {
            _Datas = new ObservableCollection<T>();
        }

        public override void Log(T pData)
        {
            ISauve lSave = new SauveCollection(Environment.CurrentDirectory);
            lSave.Sauver(pData, "Logs", true);
            _Datas.Add(pData);
        }
    }
}
