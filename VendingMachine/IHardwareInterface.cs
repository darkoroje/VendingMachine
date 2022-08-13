using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VendingMachine.ICoinRepository;

namespace VendingMachine
{
    // vending machine controls the hardware through this interface
    public interface IHardwareInterface
    {
        // sets the display
        void SetDisplay(string display);
        // returns remaining coins to the user
        void ReturnCoins(List<OneCoin> coins);
        // user has bought something, coins are to be taken
        // dispenses given product to the user
        void DispenseProduct(string name);
        // rejects the last inserted coin
        void RejectCoin();
    }
}
