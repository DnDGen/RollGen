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
        public void PrototypeDescription()
        {
            prototype.Quantity = 9266;
            prototype.Die = 90210;

            Assert.That(prototype.ToString(), Is.EqualTo("9266d90210"));
        }
    }
}
