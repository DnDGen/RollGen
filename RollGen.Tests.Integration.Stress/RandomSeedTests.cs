using Ninject;
using NUnit.Framework;
using RollGen.Domain.IoC;
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
            Stress(() => PopulateRolls(Dice1, Dice2));

            var different = false;
            for (var i = 0; i < dice1Rolls.Count; i++)
                different |= dice1Rolls[i] != dice2Rolls[i];

            Assert.That(different, Is.True);
        }

        private void PopulateRolls(Dice dice1, Dice dice2)
        {
            var firstRoll = dice1.Roll().Percentile().AsSum();
            dice1Rolls.Add(firstRoll);

            var secondRoll = dice2.Roll().Percentile().AsSum();
            dice2Rolls.Add(secondRoll);
        }

        [Test]
        public void RollsAreDifferentBetweenRolls()
        {
            Stress(() => PopulateRolls(Dice1, Dice2));

            var distinctRolls = dice1Rolls.Distinct();
            Assert.That(distinctRolls.Count(), Is.EqualTo(100));

            distinctRolls = dice2Rolls.Distinct();
            Assert.That(distinctRolls.Count(), Is.EqualTo(100));
        }

        [Test]
        public void RollsAreDifferentBetweenDiceFromFactory()
        {
            var dice1 = DiceFactory.Create();
            var dice2 = DiceFactory.Create();

            Stress(() => PopulateRolls(dice1, dice2));

            var different = false;
            for (var i = 0; i < dice1Rolls.Count; i++)
                different |= dice1Rolls[i] != dice2Rolls[i];

            Assert.That(different, Is.True);
        }

        [Test]
        public void RollsAreDifferentBetweenRollsForDiceFromFactory()
        {
            var dice1 = DiceFactory.Create();
            var dice2 = DiceFactory.Create();

            Stress(() => PopulateRolls(dice1, dice2));

            var distinctRolls = dice1Rolls.Distinct();
            Assert.That(distinctRolls.Count(), Is.EqualTo(100));

            distinctRolls = dice2Rolls.Distinct();
            Assert.That(distinctRolls.Count(), Is.EqualTo(100));
        }
    }
}