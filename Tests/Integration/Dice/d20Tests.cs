using Ninject;
using NUnit.Framework;

namespace RollGen.Tests.Integration.Rolls
{
    [TestFixture]
    public class d20Tests : ProvidedDiceTests
    {
        [Inject]
        public Dice Dice { get; set; }

        protected override int maximum
        {
            get { return 20; }
        }

        protected override int GetRoll()
        {
            return Dice.Roll().d20();
        }
    }
}