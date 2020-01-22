using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d2Tests : ProvidedDiceTests
    {
        protected override int die => 2;
        protected override PartialRoll GetRoll(int quantity) => Dice.Roll(quantity).d2();

        [Test]
        public void StressD2()
        {
            stressor.Stress(AssertRoll);
        }
    }
}