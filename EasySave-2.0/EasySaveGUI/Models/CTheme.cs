using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
        private Dictionary<string, string> _LayoutsTheme;

        public Dictionary<string, string> LayoutsTheme { get => _LayoutsTheme; set => _LayoutsTheme = value; }
        public string CurrentLayout { get => _CurrentLayout; set => _CurrentLayout = value; }
        public ETheme CurrentTheme { get => _CurrentTheme; 
            set 
            { 
                _CurrentTheme = value;
                _LayoutsTheme[_CurrentLayout] = _CurrentTheme.ToString();
            } 
        }

        public CTheme()
        {
            _LayoutsTheme = new Dictionary<string, string>();
        }


    }
}
