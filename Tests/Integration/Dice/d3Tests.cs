using System;
using D20Dice.Dice;
using NUnit.Framework;

namespace D20Dice.Test.Integration.Dice
{
    [TestFixture]
    public class d3Tests
    {
        private const Int32 TESTRUNS = 1000000;
        private const Int32 MIN = 1;
        private const Int32 MAX = 3;

        private IDice dice;

        [SetUp]
        public void Setup()
        {
            dice = DiceFactory.Create(new Random());
        }

        [Test]
        public void InRange()
        {
            for (var i = 0; i < TESTRUNS; i++)
            {
                var result = dice.d3();
                Assert.That(result, Is.InRange<Int32>(MIN, MAX));
            }
        }

        [Test]
        public void HitsMinAndMax()
        {
            var hitMin = false;
            var hitMax = false;
            var count = TESTRUNS;

            while (!(hitMin && hitMax) && count-- > 0)
            {
                var result = dice.d3();

                if (result == MIN)
                    hitMin = true;
                else if (result == MAX)
                    hitMax = true;
            }

            Assert.That(hitMin, Is.True, "Did not hit minimum");
            Assert.That(hitMax, Is.True, "Did not hit maximum");
        }
    }
}