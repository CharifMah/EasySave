using Models;

namespace ViewModels
{
    public class FormatLogViewModel : BaseViewModel
    {
        private CFormatLog _FormatLog;

        /// <summary>
        /// Classe model FormatLog
        /// </summary>
        public CFormatLog FormatLog { get => _FormatLog; set => _FormatLog = value; }

        public FormatLogViewModel()
        {
            _FormatLog = CSettings.Instance.FormatLog;

        }

        public bool SetFormatLog(string pFormatLogInfo)
        {
            bool result = _FormatLog.SetLogFormat(pFormatLogInfo);
            Models.CSettings.Instance.SaveSettings();
            return result;
        }
    }
}