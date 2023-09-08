using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StreamCompanion.Common.Helpers.Tokens
{
    public static class TokensBulkUpdate
    {
        private static readonly Dictionary<BulkTokenUpdateType, BulkTokenUpdateState> _BulkUpdateStates;
        public static readonly ReadOnlyDictionary<BulkTokenUpdateType, BulkTokenUpdateState> States;

        static TokensBulkUpdate()
        {
            _BulkUpdateStates = new Dictionary<BulkTokenUpdateType, BulkTokenUpdateState>();
            foreach (BulkTokenUpdateType type in Enum.GetValues(typeof(BulkTokenUpdateType)))
                _BulkUpdateStates[type] = new();

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
