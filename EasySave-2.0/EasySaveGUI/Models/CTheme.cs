using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class CTheme
    {
        [DataMember]
        private ETheme _CurrentTheme;
        [DataMember]
        private string _CurrentLayout;
        [DataMember]
        private Dictionary<string, ETheme> _LayoutsTheme;

        public Dictionary<string, ETheme> LayoutsTheme { get => _LayoutsTheme; set => _LayoutsTheme = value; }
        public string CurrentLayout { get => _CurrentLayout; set => _CurrentLayout = value; }
        public ETheme CurrentTheme
        {
            get => _CurrentTheme;
            set
            {
                _CurrentTheme = value;
                _LayoutsTheme[_CurrentLayout] = _CurrentTheme;
            }
        }

        public CTheme()
        {
            _LayoutsTheme = new Dictionary<string, ETheme>();
        }
    }
}
