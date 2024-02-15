namespace EasySave.Views
{
    /// <summary>
    /// Vue de l'application
    /// </summary>
    public abstract class BaseView
    {
        public abstract string Title { get; }
        /// <summary>
        /// Lance le déroulement de la vue dans l'interface de manière procedural
        /// </summary>
        public abstract void Run();
    }
}
