using System;
using System.Collections.Generic;

namespace StreamCompanionTypes.Interfaces
{
    public interface IHighFrequencyDataSender
    {
        void SetHighFrequencyDataHandlers(List<IHighFrequencyDataHandler> handlers);
    }
}