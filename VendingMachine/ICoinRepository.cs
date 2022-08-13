using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine
{
    public interface ICoinRepository
    {
        public record struct OneCoin(int coinDenomination, int count);

        bool AddCoin(int coinValueInPence);
        int GetAvailableAmount();
        void ConsumeAmount(int amount);
        public void Empty();
        // returns a list of coins that are needed to make up the current amount
        List<OneCoin> MakeChange();
    }
}
