namespace Stockage.Log
{
    public class CLogger<T> : BaseLogger<T>
    {
        private List<T> _Datas;
        public override List<T> Datas
        {
            get
            {
                return _Datas;
            }
        }

        public CLogger()
        {
            _Datas = new List<T>();
        }

        public override void Log(T pData)
        {
            ISauve lSave = new SauveCollection(Environment.CurrentDirectory);
            lSave.Sauver(pData, "Logs", true);
            _Datas.Add(pData);
        }
    }
}
