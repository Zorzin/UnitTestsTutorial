using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UnitTests
{
    public class BankAccount
    {
        public int Balance { get; private set; }

        public BankAccount(int startingBalance)
        {
            Balance = startingBalance;
        }

        public void Deposit(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposite amount must be positive", nameof(amount));
            }

            Balance += amount;
        }

        public bool Withdraw(int amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                return true;
            }

            return false;
        }
    }

    [TestFixture]
    public class BankAccountTests
    {
        private BankAccount ba;

        [SetUp]
        public void SetUp()
        {
            ba = new BankAccount(100);
        }

        [Test]
        public void BankAccountShouldIncreasePositiveDeposit()
        {

            ba.Deposit(100);

            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void BankAccountShouldThrowOnNonPositiveAmount()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => ba.Deposit(-1)
            );
            Console.WriteLine(ex.Message);
            StringAssert.StartsWith("Deposite amount must be positive",ex.Message);

        }
    }

    [TestFixture]
    class DataDrivenTests
    {
        private BankAccount ba;

        [SetUp]
        public void SetUp()
        {
            ba = new BankAccount(100);
        }

        [Test]
        [TestCase(50,true,50)]
        [TestCase(100, true, 0)]
        [TestCase(1000, false, 100)]
        public void TestMultipleWithdrawScenarios(int amountToWithdraw, bool shouldSucceed, int expectedBalance)
        {
            var result = ba.Withdraw(amountToWithdraw);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(shouldSucceed));
                Assert.That(expectedBalance, Is.EqualTo(ba.Balance));
            });
        }

    }
}
