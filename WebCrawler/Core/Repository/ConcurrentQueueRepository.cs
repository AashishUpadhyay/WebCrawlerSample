using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ConcurrentQueueRepository<T> : IConcurrentQueueRepository<T> where T : class
    {
        private ConcurrentQueue<T> queuedItems = new ConcurrentQueue<T>();
        public ConcurrentQueueRepository(ConcurrentQueue<T> queuedItems)
        {
            this.queuedItems = queuedItems;
        }
        public bool GetNext(out T result)
        {
            return this.queuedItems.TryDequeue(out result);
        }

        public void Add(T item)
        {
            if (!this.queuedItems.Contains(item))
            {
                this.queuedItems.Enqueue(item);
            }
        }
    }
}
