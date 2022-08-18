using FirstWebAPIProject.Interfaces;
using FirstWebAPIProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FirstWebAPIProject
{
    public class CacheProvider : ICacheProvider
    {
        private static readonly SemaphoreSlim GetUsersSemaphore = new SemaphoreSlim(1, 1);
        private readonly IMemoryCache _cache;
        private readonly ShopContext context;

        public CacheProvider(IMemoryCache memoryCache, ShopContext context)
        {
            _cache = memoryCache;
            this.context = context;
        }
        public async Task<IEnumerable<Products>> GetCachedResponse()
        {
            try
            {
                return await GetCachedResponse(CacheKeys.Products, GetUsersSemaphore);
            }
            catch
            {
                throw;
            }
        }
        public async Task<IEnumerable<Products>> GetCachedResponse(string cacheKey, SemaphoreSlim semaphore)
        {
            bool isAvaiable = _cache.TryGetValue(cacheKey, out List<Products> Productss);
            if (isAvaiable) return Productss;
            try
            {
                await semaphore.WaitAsync();
                isAvaiable = _cache.TryGetValue(cacheKey, out Productss);
                if (isAvaiable) return Productss;
                Productss = await context.Products.ToListAsync(); //ProductsService.GetProductssDeatilsFromDB();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2),
                    Size = 1024,
                };
                _cache.Set(cacheKey, Productss, cacheEntryOptions);
            }
            catch
            {
                throw;
            }
            finally
            {
                semaphore.Release();
            }
            return Productss;
        }
    }
}
