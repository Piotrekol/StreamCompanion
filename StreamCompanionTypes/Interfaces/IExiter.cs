using System;

namespace StreamCompanionTypes.Interfaces
{
    public interface IExiter
    {
        void SetExitHandle(Action<object> exiter);
    }
}