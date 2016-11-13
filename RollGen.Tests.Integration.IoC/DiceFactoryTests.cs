using NUnit.Framework;
using RollGen.Domain;
using RollGen.Domain.IoC;

namespace RollGen.Tests.Integration.IoC
{
    [TestFixture]
    public class DiceFactoryTests
    {
        [Test]
        public void DiceFactoryReturnsDice()
        {
            var dice = DiceFactory.Create();
            Assert.That(dice, Is.Not.Null);
            Assert.That(dice, Is.InstanceOf<DomainDice>());
        }

        [Test]
        public void DiceFactoryGeneratesMultipleDiceFromSameKernel()
        {
            var dice1 = DiceFactory.Create();
            var dice2 = DiceFactory.Create();

            var roll1 = dice1.Roll().d(Limits.Die).AsSum();
            var roll2 = dice2.Roll().d(Limits.Die).AsSum();

            Assert.That(roll1, Is.Not.EqualTo(roll2));
        }
    }
}
