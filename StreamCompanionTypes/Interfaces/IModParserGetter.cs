using System.Collections.Generic;

namespace StreamCompanionTypes.Interfaces
{
    public interface IModParserGetter
    {
        void SetModParserHandle(List<IModParser> modParser);
    }
}