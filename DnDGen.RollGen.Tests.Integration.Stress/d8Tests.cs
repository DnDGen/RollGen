using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d8Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 8; }
        }

        protected override PartialRoll GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d8();
        }

        [Test]
        public void StressD8()
        {
            stressor.Stress(AssertRollAsSum);
        }

        [Test]
        public void StressD8AsIndividualRolls()
        {
            stressor.Stress(AssertRollAsIndividualRolls);
        }

        [Test]
        public void StressD8AsMinimum()
        {
            stressor.Stress(AssertRollAsMinimum);
        }

        [Test]
        public void StressD8AsMaximum()
        {
            stressor.Stress(AssertRollAsMaximum);
        }

        [Test]
        public void StressD8AsAverage()
        {
            stressor.Stress(AssertRollAsAverage);
        }

        [Test]
        public void StressD8AsTrueOrFalse()
        {
            stressor.Stress(AssertRollAsTrueOrFalse);
        }
    }
}