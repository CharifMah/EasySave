using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockage
{
    public abstract class BaseLogger
    {
        public abstract void Log<T>(T pData);
    }
}
