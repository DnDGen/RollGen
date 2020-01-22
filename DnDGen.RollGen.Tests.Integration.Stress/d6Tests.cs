using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d6Tests : ProvidedDiceTests
    {
        protected override int die => 6;
        protected override PartialRoll GetRoll(int quantity) => Dice.Roll(quantity).d6();

        [Test]
        public void StressD6()
        {
            stressor.Stress(AssertRoll);
        }
    }
}