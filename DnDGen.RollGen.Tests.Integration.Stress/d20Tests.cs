using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d20Tests : ProvidedDiceTests
    {
        protected override int die => 20;
        protected override PartialRoll GetRoll(int quantity) => Dice.Roll(quantity).d20();

        [Test]
        public void StressD20()
        {
            stressor.Stress(AssertRoll);
        }
    }
}