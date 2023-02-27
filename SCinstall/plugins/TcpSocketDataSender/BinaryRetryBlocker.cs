using System;

namespace TcpSocketDataSender
{
    public class BinaryRetryBlocker : RetryBlocker
    {
        private readonly int _slotTime;
        public int MaxBlockTimeInSeconds { get; private set; }

        public BinaryRetryBlocker(int maxBlockTimeInSeconds = 2 * 60, int slotTime = 1)
        {
            _slotTime = slotTime;
            MaxBlockTimeInSeconds = maxBlockTimeInSeconds;
        }

        protected override int BackOffAlgorithm(int failedAttemptsInARow)
        {
            var slots = Math.Pow(2, failedAttemptsInARow) - 1;

            var blockTime = (int)(slots * _slotTime);

            return blockTime > MaxBlockTimeInSeconds
                ? MaxBlockTimeInSeconds
                : blockTime;
        }
    }
}