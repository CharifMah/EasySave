using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logs
{
    public interface IPath
    {
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
    }
}
