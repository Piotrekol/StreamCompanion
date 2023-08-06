using System;

namespace StreamCompanion.Common.Helpers.Tokens
{
    public class BulkTokenUpdateContext : IDisposable
    {
        private readonly BulkTokenUpdateState _bulkTokenUpdateState;
        
        internal BulkTokenUpdateContext(BulkTokenUpdateState bulkTokenUpdateState)
        {
            _bulkTokenUpdateState = bulkTokenUpdateState;
            bulkTokenUpdateState.InProgress = true;
        }

        public void Dispose()
        {
            _bulkTokenUpdateState.InProgress = false;
        }
    }
}
