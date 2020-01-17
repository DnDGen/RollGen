using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class PercentileTests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 100; }
        }

        protected override PartialRoll GetRoll(int quantity)
        {
            return Dice.Roll(quantity).Percentile();
        }

        [Test]
        public void StressPercentile()
        {
            stressor.Stress(AssertRollAsSum);
        }

        [Test]
        public void StressPercentileAsIndividualRolls()
        {
            stressor.Stress(AssertRollAsIndividualRolls);
        }

        [Test]
        public void StressPercentileAsMinimum()
        {
            stressor.Stress(AssertRollAsMinimum);
        }

        [Test]
        public void StressPercentileAsMaximum()
        {
            stressor.Stress(AssertRollAsMaximum);
        }

        [Test]
        public void StressPercentileAsAverage()
        {
            stressor.Stress(AssertRollAsAverage);
        }

        [Test]
        public void StressPercentileAsTrueOrFalse()
        {
            stressor.Stress(AssertRollAsTrueOrFalse);
        }
    }
}