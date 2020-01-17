using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d3Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 3; }
        }

        protected override PartialRoll GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d3();
        }

        [Test]
        public void StressD3AsSum()
        {
            stressor.Stress(AssertRollAsSum);
        }

        [Test]
        public void StressD3AsIndividualRolls()
        {
            stressor.Stress(AssertRollAsIndividualRolls);
        }

        [Test]
        public void StressD3AsMinimum()
        {
            stressor.Stress(AssertRollAsMinimum);
        }

        [Test]
        public void StressD3AsMaximum()
        {
            stressor.Stress(AssertRollAsMaximum);
        }

        [Test]
        public void StressD3AsAverage()
        {
            stressor.Stress(AssertRollAsAverage);
        }

        [Test]
        public void StressD3AsTrueOrFalse()
        {
            stressor.Stress(AssertRollAsTrueOrFalse);
        }
    }
}