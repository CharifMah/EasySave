using LogsModels;
using Newtonsoft.Json;
using Stockage.Logs;
using System.Diagnostics;
using System.Xml.Linq;

namespace Stockage
{
    public class SauveCollection : BaseSave
    {
        public SauveCollection(string pPath) : base(pPath)
        {
        }
    }
}
