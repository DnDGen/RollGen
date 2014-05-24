using System;
using D20Dice.Tests.Integration.Common;
using Ninject;
using NUnit.Framework;

namespace D20Dice.Test.Integration
{
    [TestFixture]
    public class d10Tests : IntegrationTests
    {
        [Inject]
        public IDice Dice { get; set; }

        private const Int32 TESTRUNS = 1000000;
        private const Int32 MIN = 1;
        private const Int32 MAX = 10;

        [Test]
        public void InRange()
        {
            for (var i = 0; i < TESTRUNS; i++)
            {
                var result = Dice.d10();
                Assert.That(result, Is.InRange<Int32>(MIN, MAX));
            }
        }

        [Test]
        public void HitsMinAndMax()
        {
            var hitMin = false;
            var hitMax = false;

            for (var i = 0; i < TESTRUNS; i++)
            {
                var result = Dice.d10();

                hitMin |= result == MIN;
                hitMax |= result == MAX;
            }

            Assert.That(hitMin, Is.True, "Did not hit minimum");
            Assert.That(hitMax, Is.True, "Did not hit maximum");
        }
    }
}