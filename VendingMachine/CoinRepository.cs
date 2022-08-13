using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VendingMachine.ICoinRepository;

namespace VendingMachine
{
    public class CoinRepository : ICoinRepository
    {
        // sorted in descending order so that we always return highest denomination first
        static SortedSet<int> coinDenominations = new SortedSet<int>(new DescendingOrderInts()) { 5, 10, 20, 50, 100, 200 };

        int depositedAmount = 0;

        private class DescendingOrderInts : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return y.CompareTo(x);
            }
        }

        public bool AddCoin(int coinValueInPence)
        {
            if (coinDenominations.Contains(coinValueInPence))
            {
                depositedAmount += coinValueInPence;
                return true;
            }
            return false;
        }

        public int GetAvailableAmount()
        {
            return depositedAmount;
        }

        public void Empty()
        {
            depositedAmount = 0;
        }

        void ICoinRepository.ConsumeAmount(int amount)
        {
            if (depositedAmount >= amount)
                depositedAmount -= amount;
            else
                throw new InvalidOperationException("Not enough money");
        }

        // this assumes we have prefilled the machine with enough coins
        public List<OneCoin> MakeChange()
        {
            var result = new List<OneCoin>();
            var outstandingAmount = depositedAmount;
            foreach (var coinDenomination in coinDenominations)
            {
                int maxNumOfCoins = outstandingAmount / coinDenomination;
                if (maxNumOfCoins > 0)
                {
                    result.Add(new OneCoin(coinDenomination, maxNumOfCoins));
                    outstandingAmount -= maxNumOfCoins * coinDenomination;
                }
                if (outstandingAmount <= 0)
                    break;
            }
            return result;
        }
    }
}
