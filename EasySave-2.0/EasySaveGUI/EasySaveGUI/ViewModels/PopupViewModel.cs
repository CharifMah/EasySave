namespace EasySaveGUI.ViewModels
{
    public class PopupViewModel : BaseViewModel
    {
        private string _Message;
        public string Message { get => _Message; set { _Message = value; NotifyPropertyChanged(); } }

        public PopupViewModel()
        {
        }
    }
}
