using System.Runtime.Serialization;

namespace Models.Settings.Theme
{
    /// <summary>
    /// Classe model du theme utilisateur
    /// </summary>
    [DataContract]
    public class CTheme
    {
        [DataMember]
        private ETheme _CurrentTheme;
        [DataMember]
        private string _CurrentLayout;
        [DataMember]
        private Dictionary<string, ETheme> _LayoutsTheme;
        /// <summary>
        /// Les theme lié aux layouts
        /// </summary>
        public Dictionary<string, ETheme> LayoutsTheme { get => _LayoutsTheme; set => _LayoutsTheme = value; }
        /// <summary>
        /// Layout actuel
        /// </summary>
        public string CurrentLayout { get => _CurrentLayout; set => _CurrentLayout = value; }
        /// <summary>
        /// Theme actuel
        /// </summary>
        public ETheme CurrentTheme
        {
            get => _CurrentTheme;
            set
            {
                _CurrentTheme = value;
                _LayoutsTheme[_CurrentLayout] = _CurrentTheme;
            }
        }
        /// <summary>
        /// Contructeur du theme initialise le dictionnaire de layout
        /// </summary>
        public CTheme()
        {
            _LayoutsTheme = new Dictionary<string, ETheme>();
        }
    }
}
