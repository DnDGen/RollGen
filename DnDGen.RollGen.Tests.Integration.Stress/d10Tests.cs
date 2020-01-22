using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d10Tests : ProvidedDiceTests
    {
        protected override int die => 10;
        protected override PartialRoll GetRoll(int quantity) => Dice.Roll(quantity).d10();

        [Test]
        public void StressD10()
        {
            stressor.Stress(AssertRoll);
        }
    }
}