using Ninject;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class RandomSeedTests : StressTests
    {
        [Inject]
        public Dice Dice1 { get; set; }
        [Inject]
        public Dice Dice2 { get; set; }

        private List<int> dice1Rolls;
        private List<int> dice2Rolls;

        [SetUp]
        public void Setup()
        {
            dice1Rolls = new List<int>();
            dice2Rolls = new List<int>();
        }

        [Test]
        public void RollsAreDifferentBetweenDice()
        {
            Stress(PopulateDieRolls);

            var different = false;
            for (var i = 0; i < dice1Rolls.Count; i++)
                different |= dice1Rolls[i] != dice2Rolls[i];

            Assert.That(different, Is.True);
        }

        private void PopulateDieRolls()
        {
            var firstRoll = Dice1.Roll().Percentile();
            dice1Rolls.Add(firstRoll);

            var secondRoll = Dice2.Roll().Percentile();
            dice2Rolls.Add(secondRoll);
        }

        [Test]
        public void RollsAreDifferentBetweenRolls()
        {
            Stress(PopulateDieRolls);

            var distinctRolls = dice1Rolls.Distinct();
            Assert.That(distinctRolls.Count(), Is.EqualTo(100));

            distinctRolls = dice2Rolls.Distinct();
            Assert.That(distinctRolls.Count(), Is.EqualTo(100));
        }
    }
}