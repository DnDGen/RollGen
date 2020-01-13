using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d12Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 12; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d12().AsSum();
        }

        [Test]
        public void StressD12()
        {
            stressor.Stress(AssertRoll);
        }
    }
}