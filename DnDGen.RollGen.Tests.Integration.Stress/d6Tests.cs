using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d6Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 6; }
        }

        protected override PartialRoll GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d6();
        }

        [Test]
        public void StressD6AsSum()
        {
            stressor.Stress(AssertRollAsSum);
        }

        [Test]
        public void StressD10AsIndividualRolls()
        {
            stressor.Stress(AssertRollAsIndividualRolls);
        }

        [Test]
        public void StressD10AsMinimum()
        {
            stressor.Stress(AssertRollAsMinimum);
        }

        [Test]
        public void StressD10AsMaximum()
        {
            stressor.Stress(AssertRollAsMaximum);
        }

        [Test]
        public void StressD10AsAverage()
        {
            stressor.Stress(AssertRollAsAverage);
        }
    }
}