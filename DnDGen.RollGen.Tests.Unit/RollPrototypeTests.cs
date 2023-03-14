using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Unit
{
    [TestFixture]
    public class RollPrototypeTests
    {
        private RollPrototype prototype;

        [SetUp]
        public void Setup()
        {
            prototype = new RollPrototype();
        }

        [Test]
        public void RollPrototypeInitialized()
        {
            Assert.That(prototype.Die, Is.Zero);
            Assert.That(prototype.Quantity, Is.Zero);
            Assert.That(prototype.Multiplier, Is.EqualTo(1));
        }

        [Test]
        public void BuildRoll()
        {
            prototype.Quantity = 9266;
            prototype.Die = 90210;

            var roll = prototype.Build();
            Assert.That(roll, Is.EqualTo("9266d90210"));
        }

        [Test]
        public void BuildRoll_WithMultiplier()
        {
            prototype.Quantity = 9266;
            prototype.Die = 90210;
            prototype.Multiplier = 42;

            var roll = prototype.Build();
            Assert.That(roll, Is.EqualTo("(9266d90210-9266)*42"));
        }

        [Test]
        public void PrototypeDescription()
        {
            prototype.Quantity = 9266;
            prototype.Die = 90210;

            Assert.That(prototype.ToString(), Is.EqualTo("9266d90210"));
        }

        [Test]
        public void PrototypeDescription_WithMultiplier()
        {
            prototype.Quantity = 9266;
            prototype.Die = 90210;
            prototype.Multiplier = 42;

            Assert.That(prototype.ToString(), Is.EqualTo("(9266d90210-9266)*42"));
        }

        [Test]
        public void IsValid()
        {
            prototype.Quantity = 9266;
            prototype.Die = 42;

            Assert.That(prototype.IsValid, Is.True);
        }

        [Test]
        public void IsNotValid_QuantityTooLow()
        {
            prototype.Quantity = 0;
            prototype.Die = 42;

            Assert.That(prototype.IsValid, Is.False);
        }

        [Test]
        public void IsNotValid_QuantityTooHigh()
        {
            prototype.Quantity = Limits.Quantity + 1;
            prototype.Die = 42;

            Assert.That(prototype.IsValid, Is.False);
        }

        [Test]
        public void IsNotValid_DieTooLow()
        {
            prototype.Quantity = 9266;
            prototype.Die = 0;

            Assert.That(prototype.IsValid, Is.False);
        }

        [Test]
        public void IsNotValid_DieTooHigh()
        {
            prototype.Quantity = 9266;
            prototype.Die = Limits.Die + 1;

            Assert.That(prototype.IsValid, Is.False);
        }
    }
}
