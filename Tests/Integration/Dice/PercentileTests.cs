using Ninject;
using NUnit.Framework;
using System;

namespace RollGen.Tests.Integration.Dice
{
    [TestFixture]
    public class PercentileTests : ProvidedDiceTests
    {
        [Inject]
        public IDice Dice { get; set; }

        protected override Int32 maximum
        {
            get { return 100; }
        }

        protected override Int32 GetRoll()
        {
            return Dice.Roll().Percentile();
        }
    }
}