using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine
{
    public interface IVendingMachine
    {
        // a coin has been inserted
        void AddCoin(int coinValue);
        void CheckDisplay();
        void VendProduct(string name);
    }
}
