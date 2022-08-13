using FluentAssertions;
using VendingMachine;
using static VendingMachine.ICoinRepository;

namespace Tests
{
    public class CoinRepositoryTests
    {
        ICoinRepository repository;
        public class CoinsForTest
        {
            public int [] InsertedCoins { get; set; } = new int [0];
            public int ExpectedTotal { get; set; }
            public int AmountToConsume = 0;
            public OneCoin [] ExpectedReturnedCoins { get; set; } = new OneCoin[0];
        }

        public static IEnumerable<object[]> TestCoins =>
            new List<object[]> 
            {
                new object[]
                {
                    new CoinsForTest {
                        InsertedCoins = new int [] { 5, 5, 10, 50 }, ExpectedTotal = 70,
                        AmountToConsume = 20,
                        ExpectedReturnedCoins = new OneCoin [] { new OneCoin(50, 1) },
                    },
                },
                new object[]
                {
                    new CoinsForTest {
                        InsertedCoins = new int [] { 100, 5, 20, 20 }, ExpectedTotal = 145,
                        AmountToConsume = 100,
                        ExpectedReturnedCoins = new OneCoin [] { new OneCoin(20, 2), new OneCoin(5, 1) },
                    },
                },
                new object[]
                {
                    new CoinsForTest {
                        InsertedCoins = new int [] { 100, 5, 20, 20 }, ExpectedTotal = 145,
                        AmountToConsume = 80,
                        ExpectedReturnedCoins = new OneCoin [] { new OneCoin(50, 1), new OneCoin(10, 1), new OneCoin(5, 1) },
                    },
                },
                new object[]
                {
                    new CoinsForTest {
                        InsertedCoins = new int [] { 50, 50 }, ExpectedTotal = 100,
                        AmountToConsume = 80,
                        ExpectedReturnedCoins = new OneCoin [] { new OneCoin(20, 1)  },
                    },
                }
            };

        public CoinRepositoryTests()
        {
            repository = new CoinRepository();
        }

        [Theory]
        [MemberData(nameof(TestCoins))]
        public void AddingCoinsGivesCorrectAmount(CoinsForTest testCoins)
        {
            Array.ForEach(testCoins.InsertedCoins, coin => repository.AddCoin(coin));
            repository.GetAvailableAmount().Should().Be(testCoins.ExpectedTotal, 
                $"There should be {testCoins.ExpectedTotal} pence available in repository");
        }

        [Fact]
        public void AddingInvalidCoinFails()
        {
            repository.AddCoin(6).Should().BeFalse("Repository should not accept invalid denominations");
        }

        [Fact]
        public void ShouldNotBeAbleToTakeMoreMoneyThanInserted()
        {
            Assert.Throws<InvalidOperationException>(() => repository.ConsumeAmount(10));
        }

        [Theory]
        [MemberData(nameof(TestCoins))]
        public void ReturnedChangeIsCorrect(CoinsForTest testCoins)
        {
            Array.ForEach(testCoins.InsertedCoins, coin => repository.AddCoin(coin));
            repository.ConsumeAmount(testCoins.AmountToConsume);
            var remainingCoins = repository.MakeChange();
            if (testCoins.ExpectedReturnedCoins.Length == 0)
                remainingCoins.Should().BeEmpty();
            else
                remainingCoins.Should().NotBeEmpty()
                    .And.ContainInOrder(testCoins.ExpectedReturnedCoins);
        }
    }
}