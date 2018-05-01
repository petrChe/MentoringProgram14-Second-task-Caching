using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using CachingSolutionsSamples.Interfaces;
using NorthwindLibrary;

namespace CachingSolutionsSamples.Caches
{
    public class ProductsMemoryCache : IProductCache
    {
        ObjectCache objectCache = MemoryCache.Default;
        string prefix = "Products_Cache";

        public IEnumerable<Product> Get(string forUser)
        {
            return (IEnumerable<Product>)objectCache.Get(prefix + forUser);
        }

        public void Set(string forUser, IEnumerable<Product> products)
        {
            objectCache.Set(prefix + forUser, products, ObjectCache.InfiniteAbsoluteExpiration);
        }
    }
}
