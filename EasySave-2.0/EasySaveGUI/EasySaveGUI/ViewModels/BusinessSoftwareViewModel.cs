using Models.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasySaveGUI.ViewModels
{
    public class BusinessSoftwareViewModel : BaseViewModel
    {
        public ObservableCollection<string> BusinessSoftware { get; private set; }

        public BusinessSoftwareViewModel()
        {
            BusinessSoftware = new ObservableCollection<string>(CSettings.Instance.BusinessSoftware);
        }

        public void AddBusinessSoftware(string pSoftware)
        {
            if (!string.IsNullOrWhiteSpace(pSoftware) && !BusinessSoftware.Contains(pSoftware))
            {
                BusinessSoftware.Add(pSoftware);
                CSettings.Instance.BusinessSoftware.Add(pSoftware);
                CSettings.Instance.SaveSettings();
                NotifyPropertyChanged(nameof(BusinessSoftware));
            }
        }


        public void RemoveBusinessSoftwares(List<string> pSoftwareList)
        {
            List<string> softwaresList = pSoftwareList.ToList();

            foreach (string software in softwaresList)
            {
                if (BusinessSoftware.Contains(software))
                {
                    BusinessSoftware.Remove(software);
                    CSettings.Instance.BusinessSoftware.Remove(software);
                }
            }
            CSettings.Instance.SaveSettings();
            NotifyPropertyChanged(nameof(BusinessSoftware));
        }
    }
}