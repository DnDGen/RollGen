using DnDGen.RollGen.IoC;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class RandomSeedTests : StressTests
    {
        private Dice dice1;
        private Dice dice2;
        private List<int> dice1Rolls;
        private List<int> dice2Rolls;

        [SetUp]
        public void Setup()
        {
            dice1 = GetNewInstanceOf<Dice>();
            dice2 = GetNewInstanceOf<Dice>();

            dice1Rolls = new List<int>();
            dice2Rolls = new List<int>();
        }

        [Test]
        public void RollsAreDifferentBetweenDice()
        {
            stressor.Stress(() => PopulateRolls(dice1, dice2));

            Assert.That(dice1Rolls, Is.Not.EqualTo(dice2Rolls));
            Assert.That(dice1Rolls.Distinct().Count(), Is.InRange(1000, Limits.Die));
            Assert.That(dice2Rolls.Distinct().Count(), Is.InRange(1000, Limits.Die));
        }

        private void PopulateRolls(Dice dice1, Dice dice2)
        {
            var firstRoll = dice1.Roll().d(Limits.Die).AsSum();
            dice1Rolls.Add(firstRoll);

            var secondRoll = dice2.Roll().d(Limits.Die).AsSum();
            dice2Rolls.Add(secondRoll);
        }

        [Test]
        public void RollsAreDifferentBetweenDiceFromFactory()
        {
            var dice1 = DiceFactory.Create();
            var dice2 = DiceFactory.Create();

            stressor.Stress(() => PopulateRolls(dice1, dice2));

            Assert.That(dice1Rolls, Is.Not.EqualTo(dice2Rolls));
            Assert.That(dice1Rolls.Distinct().Count(), Is.InRange(1000, Limits.Die));
            Assert.That(dice2Rolls.Distinct().Count(), Is.InRange(1000, Limits.Die));
        }
    }
}