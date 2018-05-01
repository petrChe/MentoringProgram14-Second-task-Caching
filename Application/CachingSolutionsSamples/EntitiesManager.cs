using NorthwindLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CachingSolutionsSamples.Interfaces;
using System.Configuration;
using System.Runtime.Caching;
using System.Data.SqlClient;

namespace CachingSolutionsSamples
{
	public class EntitiesManager
	{
		private ICategoriesCache cache;
        private IProductCache prCache;
        private const int rowCount = 77;

		public EntitiesManager(ICategoriesCache cache)
		{
			this.cache = cache;
		}

        public EntitiesManager(IProductCache cache)
        {
            prCache = cache;
        }

		public IEnumerable<Category> GetCategories()
		{
			Console.WriteLine("Get Categories");

			var user = Thread.CurrentPrincipal.Identity.Name;
			var categories = cache.Get(user);

			if (categories == null)
			{
				Console.WriteLine("From DB");

				using (var dbContext = new Northwind())
				{
					dbContext.Configuration.LazyLoadingEnabled = false;
					dbContext.Configuration.ProxyCreationEnabled = false;
					categories = dbContext.Categories.ToList();
					cache.Set(user, categories);
				}
			}

			return categories;
		}

        public IEnumerable<Product> GetProducts()
        {
            //Checking ivalidation
            if (!IsCacheValidForProducts())
            {
                return null;
            }

            Console.WriteLine("Get products");

            var user = Thread.CurrentPrincipal.Identity.Name;
            var products = prCache.Get(user);

            if (products == null)
            {
                Console.WriteLine("From Database");

                using (var db = new Northwind())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.ProxyCreationEnabled = false;
                    products = db.Products.ToList();
                    prCache.Set(user, products);
                }
            }

            return products;
        }


        public bool IsCacheValidForProducts()
        {
            bool isValid;

            if (MemoryCache.Default["OwnMonitoring"] == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;
                CacheItemPolicy policy = new CacheItemPolicy();
                SqlDependency.Start(connectionString);

                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("select count(*) from [Northwind].[dbo].[Products]", connection))
                {
                    command.Notification = null;
                    SqlDependency dependency = new SqlDependency();
                    dependency.AddCommandDependency(command);

                    connection.Open();

                    isValid = ((int)command.ExecuteScalar() ==  rowCount);

                    SqlChangeMonitor monitor = new SqlChangeMonitor(dependency);
                    policy.ChangeMonitors.Add(monitor);

                    connection.Close();
                }

                MemoryCache.Default.Add("OwnMonitoring", isValid, policy);
            }
            else
                isValid = (bool)MemoryCache.Default.Get("OwnMonitoring");

            return isValid;
        }
	}
}
