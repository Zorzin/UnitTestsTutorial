using System;
using System.Collections.Generic;
using System.Dynamic;
using ImpromptuInterface;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace UnitTests
{
    public interface ILog
    {
        bool Write(string msg);
    }

    public class ConsoleLog : ILog
    {
        public bool Write(string msg)
        {
            Console.WriteLine(msg);
            return true;
        }
    }

    public class BankAccount2
    {
        public int Balance { get; set; }
        private readonly ILog log;

        public BankAccount2(ILog log)
        {
            this.log = log;
        }

        public void Deposit(int amount)
        {
            if (log.Write($"Depositing {amount}"))
            {
                Balance += amount;
            }
        }
    }

    class NullLog : ILog
    {
        public bool Write(string msg)
        {
            return true;
        }
    }

    class NullLogWithResult : ILog
    {
        private bool expectedResult;

        public NullLogWithResult(bool expectedResult)
        {
            this.expectedResult = expectedResult;
        }

        public bool Write(string msg)
        {
            return expectedResult;
        }
    }

    public class Null<T> : DynamicObject where T : class
    {
        public static T Instance => new Null<T>().ActLike<T>();

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Activator.CreateInstance(typeof(T).GetMethod(binder.Name).ReturnType);
            return true;
        }
    }

    public class LogMock : ILog
    {
        private bool expectedRasult;
        public Dictionary<string, int> MethodCallCount;

        public LogMock(bool expectedRasult)
        {
            this.expectedRasult = expectedRasult;
            MethodCallCount = new Dictionary<string,int>();
        }

        private void AddOrIncrement(string methodName)
        {
            if (MethodCallCount.ContainsKey(methodName))
            {
                MethodCallCount[methodName]++;
            }
            else
            {
                MethodCallCount.Add(methodName,1);
            }
        }

        public bool Write(string msg)
        {
            AddOrIncrement(nameof(Write));
            return expectedRasult;
        }
    }


    [TestFixture]
    public class BankAccount2Tests
    {
        private BankAccount2 ba;

        [Test]
        public void DepositIntegrationTest()
        {
            ba = new BankAccount2(new ConsoleLog()) {Balance = 100};
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void DepositUnitTestWithFake()
        {
            var log = new NullLog();

            ba = new BankAccount2(log) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        //[Test]
        //public void DepositUnitTestWithDynamicFake()
        //{
        //    var log = Null<ILog>.Instance;

        //    ba = new BankAccount2(log) { Balance = 100 };
        //    ba.Deposit(100);
        //    Assert.That(ba.Balance, Is.EqualTo(200));
        //}

        [Test]
        public void DepositUnitTestWithStub()
        {
            var log = new NullLogWithResult(true);

            ba = new BankAccount2(log) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void DepositUnitTestWithMock()
        {
            var log = new LogMock(true);

            ba = new BankAccount2(log) { Balance = 100 };
            ba.Deposit(100);
            Assert.Multiple(()=>
            {
                Assert.That(ba.Balance, Is.EqualTo(200));
                Assert.That(log.MethodCallCount[nameof(LogMock.Write)], Is.EqualTo(1));
            });
        }
    }
}