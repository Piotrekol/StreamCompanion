using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Interfeaces
{
    public interface IModParserGetter
    {
        void SetModParserHandle(List<IModParser> modParser);
    }
}