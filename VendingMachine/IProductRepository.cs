using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine
{
    public interface IProductRepository
    {
        class Product
        {
            public string Name { get; set; } = String.Empty;
            public int Price { get; set; } = 0;
        }
        IReadOnlyList<Product> GetProducts();

        Product GetProduct(string name);
    }
}
