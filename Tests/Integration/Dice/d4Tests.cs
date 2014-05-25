using System;
using Ninject;
using NUnit.Framework;

namespace D20Dice.Tests.Integration.Dice
{
    [TestFixture]
    public class d4Tests : ProvidedDiceTests
    {
        [Inject]
        public IDice Dice { get; set; }

        protected override Int32 maximum
        {
            get { return 4; }
        }

        protected override Int32 GetRoll()
        {
            return Dice.d4();
        }
    }
}