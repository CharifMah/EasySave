using System.Runtime.Serialization;

namespace Models.Settings
{
    /// <summary>
    /// Log format Class
    /// </summary>
    [DataContract]
    public class CFormatLog
    {
        private Dictionary<int, string> _FormatsLogs;
        [DataMember]
        private KeyValuePair<int, string> _SelectedFormatLog;

        /// <summary>
        /// Dictionary of available log formats in the application
        /// </summary>
        public Dictionary<int, string> FormatsLogs { get => _FormatsLogs; set => _FormatsLogs = value; }
        public KeyValuePair<int, string> SelectedFormatLog { get => _SelectedFormatLog; set => _SelectedFormatLog = value; }

        /// <summary>
        /// Initializes the log format with the default format
        /// </summary>
        public CFormatLog()
        {
            _FormatsLogs = new Dictionary<int, string>()
            {
              {0, "json"},
              {1, "xml"}
            };
            _SelectedFormatLog = _FormatsLogs.First();
        }

        /// <summary>
        /// Sets the current log format
        /// </summary>
        /// <param name="pFormatLogInfo">Format log identifier</param>
        /// <returns>true if the logs format was changed</returns>
        public bool SetFormatLog(string pFormatLogInfo)
        {
            if (_FormatsLogs.Any(f => f.Value.Contains(pFormatLogInfo)))
            {
                _SelectedFormatLog = _FormatsLogs.First(f => f.Value.Contains(pFormatLogInfo));
                return true;
            }
            return false;
        }
    }
}
