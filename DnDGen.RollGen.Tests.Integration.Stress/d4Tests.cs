using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d4Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 4; }
        }

        protected override PartialRoll GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d4();
        }

        [Test]
        public void StressD4AsSum()
        {
            stressor.Stress(AssertRollAsSum);
        }

        [Test]
        public void StressD4AsIndividualRolls()
        {
            stressor.Stress(AssertRollAsIndividualRolls);
        }

        [Test]
        public void StressD4AsMinimum()
        {
            stressor.Stress(AssertRollAsMinimum);
        }

        [Test]
        public void StressD4AsMaximum()
        {
            stressor.Stress(AssertRollAsMaximum);
        }

        [Test]
        public void StressD4AsAverage()
        {
            stressor.Stress(AssertRollAsAverage);
        }

        [Test]
        public void StressD4AsTrueOrFalse()
        {
            stressor.Stress(AssertRollAsTrueOrFalse);
        }
    }
}