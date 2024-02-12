namespace EasySave.ViewModels
{
    public class MainViewModel
    {
        private LangueViewModel _LangueVm;
        private JobViewModel _JobVm;
        public LangueViewModel LangueVm { get => _LangueVm; set => _LangueVm = value; }
        public JobViewModel JobVm { get => _JobVm; set => _JobVm = value; }
        public MainViewModel()
        {
            Models.Settings.Instance.LoadSettings();
            _LangueVm = new LangueViewModel();
            _JobVm = new JobViewModel();
        }
    }
}
