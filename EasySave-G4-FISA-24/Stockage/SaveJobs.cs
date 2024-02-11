using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockage
{
    public class SaveJobs : ISauve
    {
        public void CopyDirectory(DirectoryInfo pSourceDir, DirectoryInfo pTargetDir, bool pRecursive, bool pForce = false)
        {
            throw new NotImplementedException();
        }

        public void Sauver<T>(T pData, string pFileName, bool pAppend = false, string pExtention = "json")
        {
            throw new NotImplementedException();
        }
    }
}
