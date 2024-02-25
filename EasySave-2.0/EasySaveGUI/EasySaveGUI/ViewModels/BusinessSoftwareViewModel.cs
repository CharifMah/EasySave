using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasySaveGUI.ViewModels
{
    public class BusinessSoftwareViewModel : BaseViewModel
    {
        public ObservableCollection<CBusinessSoftware> BusinessSoftwares { get; private set; }

        public BusinessSoftwareViewModel()
        {
            BusinessSoftwares = new ObservableCollection<CBusinessSoftware>(CSettings.Instance.BusinessSoftwares);
        }

        public bool AddBusinessSoftware(string softwareName)
        {
            if (!string.IsNullOrWhiteSpace(softwareName) &&
                !BusinessSoftwares.Any(software => software.Name.Equals(softwareName)))
            {
                BusinessSoftwares.Add(new CBusinessSoftware(softwareName));
                NotifyPropertyChanged(nameof(BusinessSoftwares));
                SaveSettings();
                return true;
            }
            return false;
        }

        public void RemoveBusinessSoftwares(IEnumerable<CBusinessSoftware> softwaresToRemove)
        {
            List<CBusinessSoftware> softwaresToRemoveList = softwaresToRemove.ToList();

            foreach (CBusinessSoftware software in softwaresToRemoveList)
            {
                if (BusinessSoftwares.Contains(software))
                {
                    BusinessSoftwares.Remove(software);
                }
            }
            NotifyPropertyChanged(nameof(BusinessSoftwares));
            SaveSettings();
        }

        private void SaveSettings()
        {
            try
            {
                CSettings.Instance.BusinessSoftwares = BusinessSoftwares.ToList();
                CSettings.Instance.SaveSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
    }
}