using Models;
using Pango;
using Stockage.Load;
using Stockage.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveGUI.ViewModels
{
    public class LayoutViewModel : BaseViewModel
    {
        private HashSet<string> _LayoutsName;
        private Layout _DockLayout;
        private ISauve _SauveCollection;
        private ICharge _ChargeCollection;
        public Layout DockLayout { get => _DockLayout; set => _DockLayout = value; }

        public LayoutViewModel()
        {
            _SauveCollection = new SauveCollection(CSettings.Instance.LayoutDefaultFolderPath);
            _ChargeCollection = new ChargerCollection(CSettings.Instance.LayoutDefaultFolderPath);
            _LayoutsName = new HashSet<string>();
        }

        public void SaveLayout(string pLayoutName)
        {
            _LayoutsName.Add(pLayoutName);
            _SauveCollection.Sauver(_DockLayout, pLayoutName);
        }   
        
        public Layout LoadLayout(string pLayoutName)
        {
            _DockLayout = _ChargeCollection.Charger<Layout>(pLayoutName);
            NotifyPropertyChanged("DockLayout");

            return _DockLayout;
        }


    }
}
