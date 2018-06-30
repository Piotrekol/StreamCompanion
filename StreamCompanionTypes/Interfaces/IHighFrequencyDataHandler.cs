namespace StreamCompanionTypes.Interfaces
{
    public interface IHighFrequencyDataHandler
    {
        void Handle(string content);
        void Handle(string name, string content);
    }
}