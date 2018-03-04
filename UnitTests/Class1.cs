using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
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

        public void Withdraw(int amount)
        {

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
}
