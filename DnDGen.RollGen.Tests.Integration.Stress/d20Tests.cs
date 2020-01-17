using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d20Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 20; }
        }

        protected override PartialRoll GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d20();
        }

        [Test]
        public void StressD20AsSum()
        {
            stressor.Stress(AssertRollAsSum);
        }

        [Test]
        public void StressD20AsIndividualRolls()
        {
            stressor.Stress(AssertRollAsIndividualRolls);
        }

        [Test]
        public void StressD20AsMinimum()
        {
            stressor.Stress(AssertRollAsMinimum);
        }

        [Test]
        public void StressD20AsMaximum()
        {
            stressor.Stress(AssertRollAsMaximum);
        }

        [Test]
        public void StressD20AsAverage()
        {
            stressor.Stress(AssertRollAsAverage);
        }

        [Test]
        public void StressD20AsTrueOrFalse()
        {
            stressor.Stress(AssertRollAsTrueOrFalse);
        }
    }
}