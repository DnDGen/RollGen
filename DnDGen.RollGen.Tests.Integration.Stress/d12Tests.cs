using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d12Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 12; }
        }

        protected override PartialRoll GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d12();
        }

        [Test]
        public void StressD12AsSum()
        {
            stressor.Stress(AssertRollAsSum);
        }

        [Test]
        public void StressD12AsIndividualRolls()
        {
            stressor.Stress(AssertRollAsIndividualRolls);
        }

        [Test]
        public void StressD12AsMinimum()
        {
            stressor.Stress(AssertRollAsMinimum);
        }

        [Test]
        public void StressD12AsMaximum()
        {
            stressor.Stress(AssertRollAsMaximum);
        }

        [Test]
        public void StressD12AsAverage()
        {
            stressor.Stress(AssertRollAsAverage);
        }

        [Test]
        public void StressD12AsTrueOrFalse()
        {
            stressor.Stress(AssertRollAsTrueOrFalse);
        }
    }
}