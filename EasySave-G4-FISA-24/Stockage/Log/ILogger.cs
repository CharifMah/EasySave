namespace Stockage.Log
{
    public interface ILogger<T>
    {
        List<T> Datas { get; }

        void Log(T pData);
    }
}