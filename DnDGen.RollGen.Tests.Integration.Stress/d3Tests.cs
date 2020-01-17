using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d3Tests : ProvidedDiceTests
    {
        protected override int die => 3;
        protected override PartialRoll GetRoll(int quantity) => Dice.Roll(quantity).d3();

        [Test]
        public void StressD3()
        {
            stressor.Stress(AssertRoll);
        }
    }
}