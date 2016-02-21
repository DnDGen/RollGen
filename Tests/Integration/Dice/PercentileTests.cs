using Ninject;
using NUnit.Framework;

namespace RollGen.Tests.Integration.Rolls
{
    [TestFixture]
    public class PercentileTests : ProvidedDiceTests
    {
        [Inject]
        public Dice Dice { get; set; }

        protected override int maximum
        {
            get { return 100; }
        }

        protected override int GetRoll()
        {
            return Dice.Roll().Percentile();
        }
    }
}