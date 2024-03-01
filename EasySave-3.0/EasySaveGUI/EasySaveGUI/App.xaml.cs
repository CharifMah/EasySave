using System.Windows;
using System.Threading;
using System;

namespace EasySaveGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string mutexId = "EasySaveID";
        private Mutex mutex;


        protected override void OnStartup(StartupEventArgs e)
        {
            bool estPremiereInstance;
            // On créé un Mutex global basé sur un nom unique.
            mutex = new Mutex(true, mutexId, out estPremiereInstance);

            if (!estPremiereInstance)
            {
                // On arrête complètement la deuxième instance de l'application pour éviter son exécution en arrière plan.
                Environment.Exit(0);
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //On libère le Mutex lorsque l'on a terminé.
            mutex.ReleaseMutex();
            base.OnExit(e);
        }
    }
}
