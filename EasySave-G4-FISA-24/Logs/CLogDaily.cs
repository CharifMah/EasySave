using System.Runtime.Serialization;

namespace LogsModels
{
    public class CLogDaily : CLogBase
    {
        [DataMember]
        private double _TransfertTimeSecond;

        public double TransfertTimeSecond { get => _TransfertTimeSecond; set => _TransfertTimeSecond = value; }

    }

}
