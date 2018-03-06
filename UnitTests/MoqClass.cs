using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    class MoqClass
    {
        public class BankAccount3
        {
            public int Balance { get; set; }
            private readonly ILog log;

            public BankAccount3(ILog log)
            {
                this.log = log;
            }

            public void Deposit(int amount)
            {
                log.Write($"User has withdraw {amount}");
                Balance += amount;
            }
        }
    }

    public delegate void AlientAbductionAventHandler(int galaxy, bool returned);

    public interface IAnimal
    {
        event AlientAbductionAventHandler AbductedByAliens;
        event EventHandler FallsIll;
        void Stumble();
    }

    public class Doctor
    {
        public Doctor(IAnimal animal)
        {
            animal.FallsIll += Curying;
            animal.AbductedByAliens += Animal_AbductedByAliens;
        }

        private void Animal_AbductedByAliens(int galaxy, bool returned)
        {
            ++AbductionObserved;
        }

        private void Curying(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("I will cure you!");
            TimesCured++;
        }

        public int TimesCured { get; set; }
        public int AbductionObserved { get; set; }
    }

    [TestFixture]
    public class BankAccount3Tests
    {
        private MoqClass.BankAccount3 ba;

        [Test]
        public void DepositIntegrationTest()
        {
            var log = new Mock<ILog>();
            ba = new MoqClass.BankAccount3(log.Object) {Balance = 100};

            ba.Deposit(100);

            Assert.That(ba.Balance, Is.EqualTo(200));
        }

        [Test]
        public void DoctorTest()
        {
            var mock = new Mock<IAnimal>();
            var doctor = new Doctor(mock.Object);

            mock.Raise(
                a=>a.FallsIll +=null,
                new EventArgs()
                );

            Assert.That(doctor.TimesCured, Is.EqualTo(1));

            mock.Setup(a => a.Stumble()).Raises(
                a => a.FallsIll += null,
                new EventArgs());

            mock.Object.Stumble();

            Assert.That(doctor.TimesCured, Is.EqualTo(2));

            mock.Raise(a=>a.AbductedByAliens += null,42, true);

            Assert.That(doctor.AbductionObserved, Is.EqualTo(1));

        }
    }
}
