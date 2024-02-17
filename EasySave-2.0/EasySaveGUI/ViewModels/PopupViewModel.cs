using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PopupViewModel : BaseViewModel
    {
        private string _Message;
        public string Message { get => _Message; set { _Message = value; NotifyPropertyChanged();  } }

        public PopupViewModel()
        {
        }
    }
}
