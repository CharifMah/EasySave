using View = EasySave.Views.View;
namespace EasySave // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            View pView = new View();
            pView.Run();
        }
    }
}