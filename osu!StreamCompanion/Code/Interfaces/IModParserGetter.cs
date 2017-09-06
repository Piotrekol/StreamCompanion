using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Interfaces
{
    public interface IModParserGetter
    {
        void SetModParserHandle(List<IModParser> modParser);
    }
}