using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d4Tests : ProvidedDiceTests
    {
        protected override int die => 4;
        protected override PartialRoll GetRoll(int quantity) => Dice.Roll(quantity).d4();

        [Test]
        public void StressD4()
        {
            stressor.Stress(AssertRoll);
        }
    }
}