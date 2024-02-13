namespace LogsModels
{
    /// <summary>
    /// Interface IPath
    /// </summary>
    public interface IPath
    {
        /// <summary>
        /// Répertoire source
        /// </summary>
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Répertoire cible
        /// </summary>
        public string TargetDirectory { get; set; }
    }
}
