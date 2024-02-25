using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Collections.ObjectModel;

namespace EasySaveGUI.ViewModels
{
    public class BusinessSoftwareViewModel : BaseViewModel
    {
        private ObservableCollection<CBusinessSoftware> _businessSoftwares;
     
        public ObservableCollection<CBusinessSoftware> BusinessSoftwares
        {
            get => _businessSoftwares;
            set
            {
                if (_businessSoftwares != value)
                {
                    _businessSoftwares = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BusinessSoftwareViewModel()
        {
            _businessSoftwares = new ObservableCollection<CBusinessSoftware>(CSettings.Instance.BusinessSoftwares.Values);
        }

        /// <summary>
        /// Ajoute un nouveau logiciel métier à la collection.
        /// </summary>
        /// <param name="software">Nom du logiciel métier</param>
        public bool AddBusinessSoftware(CBusinessSoftware software)
        {
            if (!CSettings.Instance.BusinessSoftwares.ContainsKey(software.Name))
            {
                _businessSoftwares.Add(software);
                CSettings.Instance.BusinessSoftwares.Add(software.Name, software);
                CSettings.Instance.SaveSettings();
                NotifyPropertyChanged(nameof(BusinessSoftwares));
                return true; // Ajout réussi
            }

            return false; // Logiciel déjà présent
        }

        public void RemoveBusinessSoftware(CBusinessSoftware software)
        {
            if (CSettings.Instance.BusinessSoftwares.ContainsKey(software.Name))
            {
                _businessSoftwares.Remove(software);
                CSettings.Instance.BusinessSoftwares.Remove(software.Name);
                CSettings.Instance.SaveSettings();
                NotifyPropertyChanged(nameof(BusinessSoftwares));
            }
        }
    }
}
