using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockage.Log
{
    public abstract class BaseLogger<T> : ILogger<T>
    {
        public abstract List<T> Datas { get; }
        public abstract void Log(T pData);
    }
}
