using View = EasySave.Views.View;
namespace EasySave // Note: actual namespace depends on the project name.
{
    /// <summary>
    /// Application entry point
    /// </summary>
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            View pView = new View();
            //Lance le program principale
            pView.Run();
        }
    }
}