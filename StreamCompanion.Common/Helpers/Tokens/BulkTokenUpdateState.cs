using System.Threading;

namespace StreamCompanion.Common.Helpers.Tokens
{
    public class BulkTokenUpdateState
    {
        internal BulkTokenUpdateState() { }
        public readonly ManualResetEventSlim UpdateFinished = new();
        private bool _inProgress = false;
        public bool InProgress
        {
            get => _inProgress;
            internal set
            {
                _inProgress = value;
                if (value)
                    UpdateFinished.Reset();
                else
                    UpdateFinished.Set();
            }
        }
    }
}
