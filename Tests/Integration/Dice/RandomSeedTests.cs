using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using NUnit.Framework;

namespace D20Dice.Tests.Integration.Dice
{
    [TestFixture]
    public class RandomSeedTests : DiceTests
    {
        [Inject]
        public IDice Dice1 { get; set; }
        [Inject]
        public IDice Dice2 { get; set; }

        private List<Int32> dice1Rolls;
        private List<Int32> dice2Rolls;

        [SetUp]
        public void Setup()
        {
            dice1Rolls = new List<Int32>();
            dice2Rolls = new List<Int32>();
        }

        [Test]
        public void RollsAreDifferentBetweenDice()
        {
            for (var i = 0; i < ConfidentIterations; i++)
            {
                dice1Rolls.Add(Dice1.Percentile());
                dice2Rolls.Add(Dice2.Percentile());
            }

            var different = false;
            for (var i = 0; i < ConfidentIterations; i++)
                different |= dice1Rolls[i] != dice2Rolls[i];

            Assert.That(different, Is.True);
        }

        [Test]
        public void RollsAreDifferentBetweenRolls()
        {
            for (var i = 0; i < ConfidentIterations; i++)
                dice1Rolls.Add(Dice1.Percentile());

            var distinctRolls = dice1Rolls.Distinct();
            Assert.That(distinctRolls.Count(), Is.EqualTo(100));
        }
    }
}