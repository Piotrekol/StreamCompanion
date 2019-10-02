namespace StreamCompanionTypes.Interfaces
{
    public interface IContextAwareLogger : ILogger
    {
        void SetContextData(string key, string value);
    }
}