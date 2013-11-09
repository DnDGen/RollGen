using NUnit.Framework;
using System;

namespace D20Dice.Test.Integration
{
    [TestFixture]
    public class d6Tests
    {
        private const Int32 TESTRUNS = 1000000;
        private const Int32 MIN = 1;
        private const Int32 MAX = 6;

        private IDice dice;

        [SetUp]
        public void Setup()
        {
            dice = DiceFactory.Create();
        }

        [Test]
        public void InRange()
        {
            for (var i = 0; i < TESTRUNS; i++)
            {
                var result = dice.d6();
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
                var result = dice.d6();

                hitMin |= result == MIN;
                hitMax |= result == MAX;
            }

            Assert.That(hitMin, Is.True, "Did not hit minimum");
            Assert.That(hitMax, Is.True, "Did not hit maximum");
        }
    }
}