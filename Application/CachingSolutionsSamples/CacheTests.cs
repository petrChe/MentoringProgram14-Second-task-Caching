using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindLibrary;
using System.Linq;
using System.Threading;
using CachingSolutionsSamples.Caches;

namespace CachingSolutionsSamples
{
	[TestClass]
	public class CacheTests
	{
		[TestMethod]
		public void MemoryCache()
		{
			var categoryManager = new EntitiesManager(new CategoriesMemoryCache());
            var productManager = new EntitiesManager(new ProductsMemoryCache());

            var productsCount = productManager.GetProducts();
            if (productsCount == null)
                return;
            else
            {
                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine(categoryManager.GetCategories().Count());
                    Console.WriteLine(productsCount.Count());
                    Thread.Sleep(100);
                }
            }
        }

		[TestMethod]
		public void RedisCache()
		{
			var categoryManager = new EntitiesManager(new CategoriesRedisCache("localhost"));
            var productManager = new EntitiesManager(new ProductsRedisCache("localhost"));

			for (var i = 0; i < 10; i++)
			{
				Console.WriteLine(categoryManager.GetCategories().Count());
                Console.WriteLine(productManager.GetProducts().Count());
				Thread.Sleep(100);
			}
		}
	}
}
