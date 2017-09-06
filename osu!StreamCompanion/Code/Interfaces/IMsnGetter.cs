using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Interfaces
{
     public interface IMsnGetter
     {
        void SetNewMsnString(Dictionary<string, string> osuStatus);
    }
}
