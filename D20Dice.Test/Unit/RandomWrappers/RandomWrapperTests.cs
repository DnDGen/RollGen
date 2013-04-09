using System;
using D20Dice.RandomWrappers;
using NUnit.Framework;

namespace D20Dice.Test.Unit.RandomWrappers
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
        public void WrapperNext()
        {
            for (var i = 0; i < TESTRUNS; i++)
            {
                var result = wrapper.Next(POSMAX);
                Assert.That(result, Is.LessThan(POSMAX));
                Assert.That(result, Is.GreaterThanOrEqualTo(0));
            }
        }
    }
}