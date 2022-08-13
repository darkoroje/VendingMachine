using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VendingMachine.IProductRepository;

namespace VendingMachine
{
    public class SampleProductRepository : IProductRepository
    {
        // hardcoded as per requirements
        // in real life an implementation would read the list from DB and interface to the 
        // hardware to see what is actually available

        private static List<Product> Products = new List<Product>
        {
            new Product {Name = "Cola", Price = 100 },
            new Product {Name = "Crisps", Price = 50},
            new Product {Name = "Chocolate", Price = 65 }
        };

        public Product GetProduct(string name)
        {
            var product = Products.Find(product => product.Name == name);
            if (product == null)
                throw new InvalidOperationException("Product not found");
            return product;
        }

        public IReadOnlyList<Product> GetProducts()
        {
            return Products.AsReadOnly();
        }
    }
}
