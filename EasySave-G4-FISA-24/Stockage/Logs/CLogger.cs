using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockage.Logs
{
    public class CLogger<T>
    {
        private CGenericLogger<T> _GenericLogger;
        private CStringLogger _StringLogger;

        public CGenericLogger<T> GenericLogger { get => _GenericLogger; set => _GenericLogger = value; }
        public CStringLogger StringLogger { get => _StringLogger; set => _StringLogger = value; }

        public CLogger()
        {
            _GenericLogger = new CGenericLogger<T>();
            _StringLogger = new CStringLogger();
        }
    }
}
