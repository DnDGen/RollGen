using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d8Tests : ProvidedDiceTests
    {
        protected override int die => 8;
        protected override PartialRoll GetRoll(int quantity) => Dice.Roll(quantity).d8();

        [Test]
        public void StressD8()
        {
            stressor.Stress(AssertRoll);
        }
    }
}