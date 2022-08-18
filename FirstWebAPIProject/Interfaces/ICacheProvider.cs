using FirstWebAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FirstWebAPIProject.Interfaces
{
    public interface ICacheProvider
    {
        public Task<IEnumerable<Products>> GetCachedResponse();

        public Task<IEnumerable<Products>> GetCachedResponse(string cacheKey, SemaphoreSlim semaphore);
    }
}
