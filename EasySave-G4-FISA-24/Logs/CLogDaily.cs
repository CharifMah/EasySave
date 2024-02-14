using System.Runtime.Serialization;
namespace LogsModels
{
    /// <summary>
    /// Classe de log journalier
    /// </summary>
    public class CLogDaily : CLogBase
    {
        [DataMember]
        private double _TransfertTimeSecond;
        /// <summary>
        /// Temps de transfert en seconde
        /// </summary>
        public double TransfertTimeSecond { get => _TransfertTimeSecond; set => _TransfertTimeSecond = value; }
    }
}
