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
    public interface IConcurrentQueueRepository<T>
    {
        bool GetNext(out T result);
        void Add(T crawlingPage);
    }
}
