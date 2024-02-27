using System.Runtime.Serialization;
namespace LogsModels
{
    /// <summary>
    /// Classe de log journalier
    /// </summary>
    public class CLogDaily : CLogBase
    {
        [DataMember]
        private double _TransfertTime;

        /// <summary>
        /// Temps de transfert en milliseconde
        /// </summary>
        public double TransfertTime { get => _TransfertTime; set => _TransfertTime = value; }
    }
}
