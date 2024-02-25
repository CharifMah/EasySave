using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Collections.ObjectModel;

namespace ViewModels
{
    public class BusinessSoftwareViewModel : BaseViewModel
    {
        private ObservableCollection<CBusinessSoftware> _businessSoftwares;
        private readonly string[] _acceptableExtensions = new string[] { ".exe", ".app", ".bat", ".sh", ".jar" }; // Extensions de fichiers acceptables

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
        /// Ajoute un nouveau logiciel métier à la collection après vérification de son extension.
        /// </summary>
        /// <param name="software">Le logiciel métier à ajouter.</param>
        public bool AddBusinessSoftware(CBusinessSoftware software)
        {
            // Vérifie l'extension du logiciel
            string extension = Path.GetExtension(software.Name);
            if (!_acceptableExtensions.Contains(extension.ToLower()))
            {
                // Extension non acceptable, retourne false
                return false;
            }

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
