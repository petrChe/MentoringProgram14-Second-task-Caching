using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthwindLibrary;

namespace CachingSolutionsSamples.Interfaces
{
    public interface IProductCache
    {
        IEnumerable<Product> Get(string forUser);
        void Set(string forUser, IEnumerable<Product> products);
    }
}
