using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachine;
using static Tests.CoinRepositoryTests;
using static VendingMachine.ICoinRepository;

namespace Tests
{
    public class VendingMachineTests
    {
        public class ProductForVending
        {
            public List<OneCoin> InsertedCoins { get; set; } = new List<OneCoin>();
            public string ProductName { get; set; } = string.Empty;
            public List<OneCoin> ExpectedReturnedCoins { get; set; } = new List<OneCoin>();
        }

        public static IEnumerable<object[]> TestProducts =>
            new List<object[]>
            {
                new object[]
                {
                    new ProductForVending {
                        InsertedCoins = new List<OneCoin> {new OneCoin(50, 2)},
                        ProductName = "Cola",
                        ExpectedReturnedCoins = new List<OneCoin>(),
                    },
                },
                new object[]
                {
                    new ProductForVending {
                        InsertedCoins = new List<OneCoin> {new OneCoin(50, 2)},
                        ProductName = "Chocolate",
                        ExpectedReturnedCoins = new List<OneCoin> {
                            new OneCoin(20, 1), new OneCoin(10, 1), new OneCoin(5, 1) 
                        },
                    },
                },
                new object[]
                {
                    new ProductForVending {
                        InsertedCoins = new List<OneCoin> {new OneCoin(100, 1), new OneCoin(50, 1)},
                        ProductName = "Crisps",
                        ExpectedReturnedCoins = new List<OneCoin> {
                            new OneCoin(100, 1)
                        },
                    },
                },
                new object[]
                {
                    new ProductForVending {
                        InsertedCoins = new List<OneCoin> {
                            new OneCoin(100, 1), new OneCoin(50, 1), new OneCoin(20, 1), new OneCoin(10, 1)
                        },
                        ProductName = "Chocolate",
                        ExpectedReturnedCoins = new List<OneCoin> {new OneCoin(100, 1)},
                    },
                },
            };

        IProductRepository products;
        Mock<IHardwareInterface> hardware;
        IVendingMachine machine;
        Mock<ICoinRepository> repository;
        string displayMessage = string.Empty;

        public VendingMachineTests()
        {
            products = new SampleProductRepository();
            hardware = new Mock<IHardwareInterface> ();
            repository = new Mock<ICoinRepository>();
            hardware.Setup(p => p.SetDisplay(It.IsAny<string>()))
                .Callback((string message) => { displayMessage = message; });
            machine = new VendingMachine.VendingMachine(hardware.Object, products, repository.Object);
        }

        [Fact]
        public void InitialDisplayIsInsertCoin()
        {
            hardware.Verify(h => h.SetDisplay("INSERT COIN"), Times.Once);
        }

        [Fact]
        public void NotEnoughMoneyProducesPriceMessage()
        {
            machine.VendProduct("Cola");
            hardware.Verify(h => h.SetDisplay("INSERT COIN"), Times.Once);
            hardware.Verify(h => h.SetDisplay("PRICE 100"), Times.Once);
        }

        private int InsertCoins(List<OneCoin> coinsToInsert)
        {
            var totalAmount = coinsToInsert.Sum(coin => coin.coinDenomination * coin.count);
            repository.Setup(m => m.GetAvailableAmount()).Returns(totalAmount);
            foreach (var coin in coinsToInsert)
            {
                for (int i=1; i<=coin.count; i++)
                    machine.AddCoin(coin.coinDenomination);
            }
            return totalAmount;
        }

        [Theory]
        [MemberData(nameof(TestProducts))]
        public void InsertedAmountIsDisplayedCorrectly(ProductForVending test)
        {
            var totalAmount = InsertCoins(test.InsertedCoins);
            displayMessage.Should().Be($"{totalAmount}", "Inserted amount is not correct");
        }

        [Theory]
        [MemberData(nameof(TestProducts))]
        public void CoinsAreInsertedCorrectly(ProductForVending test)
        {
            InsertCoins(test.InsertedCoins);
            foreach (var coin in test.InsertedCoins)
                repository.Verify(m => m.AddCoin(coin.coinDenomination), Times.Exactly(coin.count));
        }


        [Theory]
        [MemberData(nameof(TestProducts))]
        public void ProductIsVendedCorrectly(ProductForVending test)
        {
            List<OneCoin> returnedCoins = new List<OneCoin>();

            hardware.Setup(m => m.ReturnCoins(It.IsAny<List<OneCoin>>()))
                .Callback((List<OneCoin> coinList) => { returnedCoins = coinList; });
            repository.Setup(m => m.MakeChange()).Returns(test.ExpectedReturnedCoins);

            InsertCoins(test.InsertedCoins);
            machine.VendProduct(test.ProductName);

            hardware.Verify(m => m.DispenseProduct(test.ProductName));
            hardware.Verify(m => m.ReturnCoins(It.IsAny<List<OneCoin>>()), Times.Once);
            repository.Verify(m => m.MakeChange(), Times.Once);
            repository.Verify(m => m.Empty(), Times.Once);
            if (test.ExpectedReturnedCoins.Count == 0)
                returnedCoins.Should().BeEmpty();
            else
                returnedCoins.Should().NotBeEmpty()
                    .And.ContainInOrder(test.ExpectedReturnedCoins);
            displayMessage.Should().Be("THANK YOU", "Incorrect vending message");
        }

    }
}
