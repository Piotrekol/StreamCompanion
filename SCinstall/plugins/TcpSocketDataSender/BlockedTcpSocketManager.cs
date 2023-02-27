namespace TcpSocketDataSender
{
    public class BlockedTcpSocketManager : TcpSocketManager
    {
        BinaryRetryBlocker _blocker = new BinaryRetryBlocker();


        public override bool Connect()
        {
            return _blocker.Execute(base.Connect);
        }
    }
}