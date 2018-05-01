using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CachingSolutionsSamples.Interfaces;
using NorthwindLibrary;
using StackExchange.Redis;

namespace CachingSolutionsSamples.Caches
{
    public class ProductsRedisCache : IProductCache
    {
        private ConnectionMultiplexer redisConnection;
        string prefix = "Product_Cache";
        DataContractSerializer serializer = new DataContractSerializer(typeof(IEnumerable<Product>));

        public ProductsRedisCache(string hostName)
        {
            redisConnection = ConnectionMultiplexer.Connect(hostName);
        }

        public IEnumerable<Product> Get(string forUser)
        {
            var db = redisConnection.GetDatabase();
            byte[] s = db.StringGet(prefix + forUser);

            if (s == null)
                return null;

            return (IEnumerable<Product>)serializer.ReadObject(new MemoryStream(s));
            
        }

        public void Set(string forUser, IEnumerable<Product> products)
        {
            var db = redisConnection.GetDatabase();
            var key = prefix + forUser;

            if (products == null)
                db.StringSet(key, RedisValue.Null);
            else
            {
                var stream = new MemoryStream();
                serializer.WriteObject(stream, products);
                db.StringSet(key, stream.ToArray());            
            }
        }
    }
}
