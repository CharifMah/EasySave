namespace Stockage
{
    public class CLogger : BaseLogger
    {
        public override void Log<T>(T pData)
        {
            ISauve lSave = new SauveCollection(Environment.CurrentDirectory);
            lSave.Sauver(pData, "Logs", true);
        }
    }
}
