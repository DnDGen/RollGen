using System;
using D20Dice.RandomWrappers;
using NUnit.Framework;

namespace D20Dice.Test.Integration.RandomWrappers
{
    [TestFixture]
    public class RandomWrapperTests
    {
        private const Int32 POSMAX = 10;
        private const Int32 TESTRUNS = 1000000;

        private IRandomWrapper wrapper;

        [SetUp]
        public void Setup()
        {
            wrapper = new RandomWrapper(new Random());
        }

        [Test]
        public void InRange()
        {
            for (var i = 0; i < TESTRUNS; i++)
            {
                var result = wrapper.Next(POSMAX);
                Assert.That(result, Is.InRange<Int32>(0, POSMAX));
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
                var result = wrapper.Next(POSMAX);

                if (result == 0)
                    hitMin = true;
                else if (result == POSMAX - 1)
                    hitMax = true;
            }

            Assert.That(hitMin, Is.True, "Did not hit minimum");
            Assert.That(hitMax, Is.True, "Did not hit maximum");
        }
    }
}