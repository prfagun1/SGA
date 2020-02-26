namespace SGA.Interfaces
{
    public interface ILogCustomRepository
    {
        void SaveLogApplicationMessage(string description, string message);
        void SaveLogApplicationError(string description, string message);

    }
}
