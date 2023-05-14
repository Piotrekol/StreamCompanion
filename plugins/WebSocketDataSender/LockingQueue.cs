using System.Collections.Generic;

namespace WebSocketDataSender
{
    internal class LockingQueue<T>
    {
        protected Queue<T> queue = new Queue<T>();
        private readonly object syncRoot = new();
        
        public int Count { get { lock (syncRoot) return queue.Count; } }

        public bool TryPeek(out T value)
        {
            lock (syncRoot)
                return queue.TryPeek(out value);
        }

        public bool TryDequeue(out T value)
        {
            lock (syncRoot)
                return queue.TryDequeue(out value);
        }

        public void Enqueue(T value)
        {
            lock (syncRoot)
                queue.Enqueue(value);
        }
    }
}