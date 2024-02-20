using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// Log format Class
    /// </summary>
    [DataContract]
    public class CFormatLog
    {
        private Dictionary<int, string> _FormatsLogs;
        [DataMember]
        private string _SelectedLogFormat;

        /// <summary>
        /// Dictionnaire de formats de logs dans l'application
        /// </summary>
        public Dictionary<int, string> FormatsLogs { get => _FormatsLogs; set => _FormatsLogs = value; }
        public string SelectedLogFormat { get => _SelectedLogFormat; set => _SelectedLogFormat = value; }

        /// <summary>
        /// Initialize the format logs
        /// </summary>
        public CFormatLog()
        {
            _FormatsLogs = new Dictionary<int, string>()
            {
              {1, "json"},
              {2, "xml"}
            };
            _SelectedLogFormat = "json";
        }

        /// <summary>
        /// Set the current logs format
        /// </summary>
        /// <param name="pFormatLogInfo">give a number</param>
        /// <returns>true if the logs format was changed</returns>
        public bool SetLogFormat(string pFormatLogInfo)
        {

            SelectedLogFormat = pFormatLogInfo;
            return true;
        }
    }
}
