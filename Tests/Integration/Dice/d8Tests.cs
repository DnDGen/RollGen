using System;
using System.Collections.Generic;
using Ninject;
using NUnit.Framework;

namespace D20Dice.Tests.Integration.Dice
{
    [TestFixture]
    public class d8Tests : ProvidedDiceTests
    {
        [Inject]
        public IDice Dice { get; set; }

        protected override Int32 maximum
        {
            get { return 8; }
        }

        protected override Int32 GetRoll()
        {
            return Dice.Roll().d8();
        }
    }
}