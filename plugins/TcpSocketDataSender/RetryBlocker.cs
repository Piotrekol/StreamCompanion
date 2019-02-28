using System;

namespace TcpSocketDataSender
{
    public abstract class RetryBlocker
    {
        public int FailedAttempts { get; private set; } = 0;
        private DateTime NextAllowedAttempt { get; set; } = DateTime.UtcNow;
        
        public bool CanRetry => DateTime.UtcNow >= NextAllowedAttempt;

        public void AddFailedAttempt()
        {
            if (!CanRetry)
                return;

            var blockTime = BackOffAlgorithm(FailedAttempts++);

            NextAllowedAttempt = DateTime.UtcNow.AddSeconds(blockTime);
        }

        public void CompletedSuccessfully()
        {
            FailedAttempts = 0;
            NextAllowedAttempt = DateTime.UtcNow;
        }

        public bool Execute(Func<bool> func)
        {
            if (!CanRetry)
                return false;

            var result = func();

            if (result)
            {
                CompletedSuccessfully();
            }
            else
            {
                AddFailedAttempt();
            }

            return result;
        }
        protected abstract int BackOffAlgorithm(int failedAttemptsInARow);
    }
}