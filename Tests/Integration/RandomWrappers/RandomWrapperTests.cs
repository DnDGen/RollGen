using System;
using D20Dice.RandomWrappers;
using NUnit.Framework;

namespace D20Dice.Test.Integration.RandomWrappers
{
    [TestFixture]
    public class RandomWrapperTests
    {
        private const Int32 MAX = 10;
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
                var result = wrapper.Next(MAX);
                Assert.That(result, Is.InRange<Int32>(0, MAX));
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
                var result = wrapper.Next(MAX);

                hitMin |= result == 0;
                hitMax |= result == MAX - 1;
            }

            Assert.That(hitMin, Is.True, "Did not hit minimum");
            Assert.That(hitMax, Is.True, "Did not hit maximum");
        }
    }
}