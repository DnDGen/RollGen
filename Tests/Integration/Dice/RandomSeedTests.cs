using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Tests.Integration.Rolls
{
    [TestFixture]
    public class RandomSeedTests : DiceTests
    {
        [Inject]
        public Dice Dice1 { get; set; }
        [Inject]
        public Dice Dice2 { get; set; }

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
                dice1Rolls.Add(Dice1.Roll().Percentile());
                dice2Rolls.Add(Dice2.Roll().Percentile());
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
                dice1Rolls.Add(Dice1.Roll().Percentile());

            var distinctRolls = dice1Rolls.Distinct();
            Assert.That(distinctRolls.Count(), Is.EqualTo(100));
        }
    }
}