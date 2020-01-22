using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d12Tests : ProvidedDiceTests
    {
        protected override int die => 12;
        protected override PartialRoll GetRoll(int quantity) => Dice.Roll(quantity).d12();

        [Test]
        public void StressD12()
        {
            stressor.Stress(AssertRoll);
        }
    }
}