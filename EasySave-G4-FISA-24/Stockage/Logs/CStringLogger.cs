using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockage.Logs
{
    public class CStringLogger : BaseLogger<string>
    {
        private ObservableCollection<string> _Datas;
        public override ObservableCollection<string> Datas => _Datas;

        public CStringLogger()
        {
            _Datas = new ObservableCollection<string>();
        }

        public override void Log(string pData)
        {
            ISauve lSave = new SauveCollection(Environment.CurrentDirectory);
            lSave.Sauver(pData, "Logs", true);
            _Datas.Add(pData);
        }
    }
}
