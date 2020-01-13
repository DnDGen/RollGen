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

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d8().AsSum();
        }

        [Test]
        public void StressD8()
        {
            stressor.Stress(AssertRoll);
        }
    }
}