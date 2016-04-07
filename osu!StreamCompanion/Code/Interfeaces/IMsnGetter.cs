using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Interfeaces
{
     public interface IMsnGetter
     {
        void SetNewMsnString(Dictionary<string, string> osuStatus);
    }
}
