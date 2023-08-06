using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StreamCompanion.Common.Helpers.Tokens
{
    public static class TokensBulkUpdate
    {
        private static Dictionary<BulkTokenUpdateType, BulkTokenUpdateState> _BulkUpdateStates;
        public static ReadOnlyDictionary<BulkTokenUpdateType, BulkTokenUpdateState> States { get; private set; }

        static TokensBulkUpdate()
        {
            _BulkUpdateStates = new Dictionary<BulkTokenUpdateType, BulkTokenUpdateState>();
            foreach (BulkTokenUpdateType state in Enum.GetValues(typeof(BulkTokenUpdateType)))
                _BulkUpdateStates[state] = new();

            States = new(_BulkUpdateStates);
        }

        public static BulkTokenUpdateContext StartBulkUpdate(BulkTokenUpdateType bulkTokenUpdateType)
        {
            if (_BulkUpdateStates[bulkTokenUpdateType].InProgress)
                throw new InvalidOperationException("Token bulk update is already in progress.");

            return new BulkTokenUpdateContext(_BulkUpdateStates[bulkTokenUpdateType]);
        }
    }
}
