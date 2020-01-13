using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d3Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 3; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d3().AsSum();
        }

        [Test]
        public void StressD3()
        {
            stressor.Stress(AssertRoll);
        }
    }
}