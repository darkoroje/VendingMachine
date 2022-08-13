using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine
{
    public class VendingMachine : IVendingMachine
    {
        private readonly IHardwareInterface hardware;
        private readonly IProductRepository products;
        private readonly ICoinRepository repository;

        public VendingMachine(IHardwareInterface hardware, IProductRepository products, ICoinRepository repository)
        {
            this.hardware = hardware;
            this.products = products;
            this.repository = repository;
            CheckDisplay();
        }

        public void AddCoin(int coinValue)
        {
            if (repository.AddCoin(coinValue))
                hardware.RejectCoin();
            else
                CheckDisplay();
        }

        public void CheckDisplay()
        {
            var amountAvailable = repository.GetAvailableAmount();
            this.hardware.SetDisplay(amountAvailable > 0 ? $"{amountAvailable}" : "INSERT COIN");
        }

        public void VendProduct(string name)
        {
            try
            {
                var product = products.GetProduct(name);
                if (repository.GetAvailableAmount() >= product.Price)
                {
                    repository.ConsumeAmount(product.Price);
                    hardware.DispenseProduct(name);
                    var customerChange = repository.MakeChange();
                    hardware.ReturnCoins(customerChange);
                    hardware.SetDisplay("THANK YOU");
                    repository.Empty();
                }
                else
                {
                    hardware.SetDisplay($"PRICE {product.Price}");
                }
            }
            catch (Exception ex)
            {
                // here we would log the error or take some action, but for now just
                // display it
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
