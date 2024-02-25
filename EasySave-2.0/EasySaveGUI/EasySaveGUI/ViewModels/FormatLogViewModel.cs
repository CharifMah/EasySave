using Models;

namespace EasySaveGUI.ViewModels
{
    public class FormatLogViewModel : BaseViewModel
    {
        private CFormatLog _FormatLog;

        /// <summary>
        /// Classe model FormatLog
        /// </summary>
        public CFormatLog FormatLog
        {
            get => _FormatLog;
            set { _FormatLog = value; NotifyPropertyChanged(); }
        }

        public FormatLogViewModel()
        {
            _FormatLog = CSettings.Instance.FormatLog;
            SetFormatLog(_FormatLog.SelectedFormatLog.Value);
            NotifyPropertyChanged("FormatLog");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFormatLogInfo"></param>
        /// <returns></returns>
        public bool SetFormatLog(string pFormatLogInfo)
        {
            bool result = _FormatLog.SetFormatLog(pFormatLogInfo);
            CSettings.Instance.SaveSettings();
            NotifyPropertyChanged("FormatLog");
            return result;
        }
    }
}