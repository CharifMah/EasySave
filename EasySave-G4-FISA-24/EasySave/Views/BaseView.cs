﻿namespace EasySave.Views
{
    public abstract class BaseView
    {
        public abstract string Title { get; }
        /// <summary>
        /// Lance le deroulement de la vue dans l'interface de maniere procedurale
        /// </summary>
        public abstract void Run();
    }
}
