using DnDGen.RollGen.PartialRolls;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class RollTests
    {
        private Roll roll;
        private const string DEFAULT_ROLL = "1d2";
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            roll = new Roll(DEFAULT_ROLL);

            var count = 0;
            mockRandom.Setup(r => r.Next(It.IsAny<int>())).Returns((int max) => count++ % max);
        }

        [TestCase("1d2", 1, 2, 0, false)]
        [TestCase("1d2!", 1, 2, 0, true)]
        [TestCase("1d2!!", 1, 2, 0, true)]
        [TestCase(" 1 d 2 ", 1, 2, 0, false)]
        [TestCase(" 1 d 2 ! ", 1, 2, 0, true)]
        [TestCase(" 1 d 2 ! ! ", 1, 2, 0, true)]
        [TestCase("1d2k3", 1, 2, 3, false)]
        [TestCase("1d2!k3", 1, 2, 3, true)]
        [TestCase("1d2k3!", 1, 2, 3, true)]
        [TestCase("1d2!k3!", 1, 2, 3, true)]
        [TestCase(" 1 d 2 k 3 ", 1, 2, 3, false)]
        [TestCase(" 1 d 2 ! k 3 ", 1, 2, 3, true)]
        [TestCase(" 1 d 2 k 3 ! ", 1, 2, 3, true)]
        [TestCase(" 1 d 2 ! k 3 ! ", 1, 2, 3, true)]
        [TestCase("d2", 1, 2, 0, false)]
        [TestCase("d2!", 1, 2, 0, true)]
        [TestCase("d2!!", 1, 2, 0, true)]
        [TestCase(" d 2 ", 1, 2, 0, false)]
        [TestCase(" d 2 ! ", 1, 2, 0, true)]
        [TestCase(" d 2 ! ! ", 1, 2, 0, true)]
        [TestCase("1230d456k789", 1230, 456, 789, false)]
        [TestCase("1230d456!k789", 1230, 456, 789, true)]
        [TestCase("1230d456k789!", 1230, 456, 789, true)]
        [TestCase("1230d456!k789!", 1230, 456, 789, true)]
        [TestCase(" 1230 d 456 k 789 ", 1230, 456, 789, false)]
        [TestCase(" 1230 d 456 ! k 789 ", 1230, 456, 789, true)]
        [TestCase(" 1230 d 456 k 789 ! ", 1230, 456, 789, true)]
        [TestCase(" 1230 d 456 ! k 789 ! ", 1230, 456, 789, true)]
        [TestCase("92d66k42", 92, 66, 42, false)]
        [TestCase("92 d 66 k 42 ", 92, 66, 42, false)]
        [TestCase("92d66!k42", 92, 66, 42, true)]
        [TestCase("92d66k42!", 92, 66, 42, true)]
        [TestCase("92d66!k42!", 92, 66, 42, true)]
        [TestCase("92 d 66 ! k 42 ", 92, 66, 42, true)]
        [TestCase("92 d 66 k 42 ! ", 92, 66, 42, true)]
        [TestCase("92 d 66 ! k 42 ! ", 92, 66, 42, true)]
        public void ParseExpression(string expression, int quantity, int die, int toKeep, bool explode)
        {
            roll = new Roll(expression);
            Assert.That(roll.Quantity, Is.EqualTo(quantity));
            Assert.That(roll.Die, Is.EqualTo(die));
            Assert.That(roll.Explode, Is.EqualTo(explode));
            Assert.That(roll.AmountToKeep, Is.EqualTo(toKeep));
            Assert.That(roll.IsValid, Is.True);
        }

        [TestCase("", false)]
        [TestCase("!", false)]
        [TestCase("1", false)]
        [TestCase("1!", false)]
        [TestCase("1d2", true)]
        [TestCase("1d2!", true)]
        [TestCase("1d", false)]
        [TestCase("1d!", false)]
        [TestCase(" 1 d 2 ", true)]
        [TestCase(" 1 d 2 ! ", true)]
        [TestCase("1d2k3", true)]
        [TestCase("1d2!k3", true)]
        [TestCase("1d2k3!", true)]
        [TestCase(" 1 d 2 k 3 ", true)]
        [TestCase(" 1 d 2 ! k 3 ", true)]
        [TestCase(" 1 d 2 k 3 ! ", true)]
        [TestCase("d2", true)]
        [TestCase("d2!", true)]
        [TestCase(" d 2 ", true)]
        [TestCase(" d 2 ! ", true)]
        [TestCase("Not parsable", false)]
        [TestCase("1d2+3", false)]
        [TestCase("1d2!+3", false)]
        [TestCase("1+2-3", false)]
        [TestCase("1230d456k789", true)]
        [TestCase("1230d456!k789", true)]
        [TestCase("1230d456k789!", true)]
        [TestCase(" 1230 d 456 k 789 ", true)]
        [TestCase(" 1230 d 456 ! k 789 ", true)]
        [TestCase(" 1230 d 456 k 789 ! ", true)]
        [TestCase(" 12 30 d 4 56 k 7 8 9 ", false)]
        [TestCase(" 12 30 d 4 56 ! k 7 8 9 ", false)]
        [TestCase(" 12 30 d 4 56 k 7 8 9 ! ", false)]
        public void CanParse(string expression, bool canParse)
        {
            Assert.That(Roll.CanParse(expression), Is.EqualTo(canParse));
        }

        [TestCase(-1, -1, -2, false, false)]
        [TestCase(-1, -1, -2, true, false)]
        [TestCase(-1, -1, -1, false, false)]
        [TestCase(-1, -1, -1, true, false)]
        [TestCase(-1, -1, 0, false, false)]
        [TestCase(-1, -1, 0, true, false)]
        [TestCase(-1, -1, 1, false, false)]
        [TestCase(-1, -1, 1, true, false)]
        [TestCase(-1, -1, 2, false, false)]
        [TestCase(-1, -1, 2, true, false)]
        [TestCase(-1, -1, Limits.Quantity - 1, false, false)]
        [TestCase(-1, -1, Limits.Quantity - 1, true, false)]
        [TestCase(-1, -1, Limits.Quantity, false, false)]
        [TestCase(-1, -1, Limits.Quantity, true, false)]
        [TestCase(-1, -1, Limits.Quantity + 1, false, false)]
        [TestCase(-1, -1, Limits.Quantity + 1, true, false)]
        [TestCase(-1, 0, -2, false, false)]
        [TestCase(-1, 0, -2, true, false)]
        [TestCase(-1, 0, -1, false, false)]
        [TestCase(-1, 0, -1, true, false)]
        [TestCase(-1, 0, 0, false, false)]
        [TestCase(-1, 0, 0, true, false)]
        [TestCase(-1, 0, 1, false, false)]
        [TestCase(-1, 0, 1, true, false)]
        [TestCase(-1, 0, 2, false, false)]
        [TestCase(-1, 0, 2, true, false)]
        [TestCase(-1, 0, Limits.Quantity - 1, false, false)]
        [TestCase(-1, 0, Limits.Quantity - 1, true, false)]
        [TestCase(-1, 0, Limits.Quantity, false, false)]
        [TestCase(-1, 0, Limits.Quantity, true, false)]
        [TestCase(-1, 0, Limits.Quantity + 1, false, false)]
        [TestCase(-1, 0, Limits.Quantity + 1, true, false)]
        [TestCase(-1, 1, -2, false, false)]
        [TestCase(-1, 1, -2, true, false)]
        [TestCase(-1, 1, -1, false, false)]
        [TestCase(-1, 1, -1, true, false)]
        [TestCase(-1, 1, 0, false, false)]
        [TestCase(-1, 1, 0, true, false)]
        [TestCase(-1, 1, 1, false, false)]
        [TestCase(-1, 1, 1, true, false)]
        [TestCase(-1, 1, 2, false, false)]
        [TestCase(-1, 1, 2, true, false)]
        [TestCase(-1, 1, Limits.Quantity - 1, false, false)]
        [TestCase(-1, 1, Limits.Quantity - 1, true, false)]
        [TestCase(-1, 1, Limits.Quantity, false, false)]
        [TestCase(-1, 1, Limits.Quantity, true, false)]
        [TestCase(-1, 1, Limits.Quantity + 1, false, false)]
        [TestCase(-1, 1, Limits.Quantity + 1, true, false)]
        [TestCase(-1, 2, -2, false, false)]
        [TestCase(-1, 2, -2, true, false)]
        [TestCase(-1, 2, -1, false, false)]
        [TestCase(-1, 2, -1, true, false)]
        [TestCase(-1, 2, 0, false, false)]
        [TestCase(-1, 2, 0, true, false)]
        [TestCase(-1, 2, 1, false, false)]
        [TestCase(-1, 2, 1, true, false)]
        [TestCase(-1, 2, 2, false, false)]
        [TestCase(-1, 2, 2, true, false)]
        [TestCase(-1, 2, Limits.Quantity - 1, false, false)]
        [TestCase(-1, 2, Limits.Quantity - 1, true, false)]
        [TestCase(-1, 2, Limits.Quantity, false, false)]
        [TestCase(-1, 2, Limits.Quantity, true, false)]
        [TestCase(-1, 2, Limits.Quantity + 1, false, false)]
        [TestCase(-1, 2, Limits.Quantity + 1, true, false)]
        [TestCase(-1, Limits.Die - 1, -2, false, false)]
        [TestCase(-1, Limits.Die - 1, -2, true, false)]
        [TestCase(-1, Limits.Die - 1, -1, false, false)]
        [TestCase(-1, Limits.Die - 1, -1, true, false)]
        [TestCase(-1, Limits.Die - 1, 0, false, false)]
        [TestCase(-1, Limits.Die - 1, 0, true, false)]
        [TestCase(-1, Limits.Die - 1, 1, false, false)]
        [TestCase(-1, Limits.Die - 1, 1, true, false)]
        [TestCase(-1, Limits.Die - 1, 2, false, false)]
        [TestCase(-1, Limits.Die - 1, 2, true, false)]
        [TestCase(-1, Limits.Die - 1, Limits.Quantity - 1, false, false)]
        [TestCase(-1, Limits.Die - 1, Limits.Quantity - 1, true, false)]
        [TestCase(-1, Limits.Die - 1, Limits.Quantity, false, false)]
        [TestCase(-1, Limits.Die - 1, Limits.Quantity, true, false)]
        [TestCase(-1, Limits.Die - 1, Limits.Quantity + 1, false, false)]
        [TestCase(-1, Limits.Die - 1, Limits.Quantity + 1, true, false)]
        [TestCase(-1, Limits.Die, -2, false, false)]
        [TestCase(-1, Limits.Die, -2, true, false)]
        [TestCase(-1, Limits.Die, -1, false, false)]
        [TestCase(-1, Limits.Die, -1, true, false)]
        [TestCase(-1, Limits.Die, 0, false, false)]
        [TestCase(-1, Limits.Die, 0, true, false)]
        [TestCase(-1, Limits.Die, 1, false, false)]
        [TestCase(-1, Limits.Die, 1, true, false)]
        [TestCase(-1, Limits.Die, 2, false, false)]
        [TestCase(-1, Limits.Die, 2, true, false)]
        [TestCase(-1, Limits.Die, Limits.Quantity - 1, false, false)]
        [TestCase(-1, Limits.Die, Limits.Quantity - 1, true, false)]
        [TestCase(-1, Limits.Die, Limits.Quantity, false, false)]
        [TestCase(-1, Limits.Die, Limits.Quantity, true, false)]
        [TestCase(-1, Limits.Die, Limits.Quantity + 1, false, false)]
        [TestCase(-1, Limits.Die, Limits.Quantity + 1, true, false)]
        [TestCase(-1, Limits.Die + 1, -2, false, false)]
        [TestCase(-1, Limits.Die + 1, -2, true, false)]
        [TestCase(-1, Limits.Die + 1, -1, false, false)]
        [TestCase(-1, Limits.Die + 1, -1, true, false)]
        [TestCase(-1, Limits.Die + 1, 0, false, false)]
        [TestCase(-1, Limits.Die + 1, 0, true, false)]
        [TestCase(-1, Limits.Die + 1, 1, false, false)]
        [TestCase(-1, Limits.Die + 1, 1, true, false)]
        [TestCase(-1, Limits.Die + 1, 2, false, false)]
        [TestCase(-1, Limits.Die + 1, 2, true, false)]
        [TestCase(-1, Limits.Die + 1, Limits.Quantity - 1, false, false)]
        [TestCase(-1, Limits.Die + 1, Limits.Quantity - 1, true, false)]
        [TestCase(-1, Limits.Die + 1, Limits.Quantity, false, false)]
        [TestCase(-1, Limits.Die + 1, Limits.Quantity, true, false)]
        [TestCase(-1, Limits.Die + 1, Limits.Quantity + 1, false, false)]
        [TestCase(-1, Limits.Die + 1, Limits.Quantity + 1, true, false)]
        [TestCase(0, -1, -2, false, false)]
        [TestCase(0, -1, -2, true, false)]
        [TestCase(0, -1, -1, false, false)]
        [TestCase(0, -1, -1, true, false)]
        [TestCase(0, -1, 0, false, false)]
        [TestCase(0, -1, 0, true, false)]
        [TestCase(0, -1, 1, false, false)]
        [TestCase(0, -1, 1, true, false)]
        [TestCase(0, -1, 2, false, false)]
        [TestCase(0, -1, 2, true, false)]
        [TestCase(0, -1, Limits.Quantity - 1, false, false)]
        [TestCase(0, -1, Limits.Quantity - 1, true, false)]
        [TestCase(0, -1, Limits.Quantity, false, false)]
        [TestCase(0, -1, Limits.Quantity, true, false)]
        [TestCase(0, -1, Limits.Quantity + 1, false, false)]
        [TestCase(0, -1, Limits.Quantity + 1, true, false)]
        [TestCase(0, 0, -2, false, false)]
        [TestCase(0, 0, -2, true, false)]
        [TestCase(0, 0, -1, false, false)]
        [TestCase(0, 0, -1, true, false)]
        [TestCase(0, 0, 0, false, false)]
        [TestCase(0, 0, 0, true, false)]
        [TestCase(0, 0, 1, false, false)]
        [TestCase(0, 0, 1, true, false)]
        [TestCase(0, 0, 2, false, false)]
        [TestCase(0, 0, 2, true, false)]
        [TestCase(0, 0, Limits.Quantity - 1, false, false)]
        [TestCase(0, 0, Limits.Quantity - 1, true, false)]
        [TestCase(0, 0, Limits.Quantity, false, false)]
        [TestCase(0, 0, Limits.Quantity, true, false)]
        [TestCase(0, 0, Limits.Quantity + 1, false, false)]
        [TestCase(0, 0, Limits.Quantity + 1, true, false)]
        [TestCase(0, 1, -2, false, false)]
        [TestCase(0, 1, -2, true, false)]
        [TestCase(0, 1, -1, false, false)]
        [TestCase(0, 1, -1, true, false)]
        [TestCase(0, 1, 0, false, false)]
        [TestCase(0, 1, 0, true, false)]
        [TestCase(0, 1, 1, false, false)]
        [TestCase(0, 1, 1, true, false)]
        [TestCase(0, 1, 2, false, false)]
        [TestCase(0, 1, 2, true, false)]
        [TestCase(0, 1, Limits.Quantity - 1, false, false)]
        [TestCase(0, 1, Limits.Quantity - 1, true, false)]
        [TestCase(0, 1, Limits.Quantity, false, false)]
        [TestCase(0, 1, Limits.Quantity, true, false)]
        [TestCase(0, 1, Limits.Quantity + 1, false, false)]
        [TestCase(0, 1, Limits.Quantity + 1, true, false)]
        [TestCase(0, 2, -2, false, false)]
        [TestCase(0, 2, -2, true, false)]
        [TestCase(0, 2, -1, false, false)]
        [TestCase(0, 2, -1, true, false)]
        [TestCase(0, 2, 0, false, false)]
        [TestCase(0, 2, 0, true, false)]
        [TestCase(0, 2, 1, false, false)]
        [TestCase(0, 2, 1, true, false)]
        [TestCase(0, 2, 2, false, false)]
        [TestCase(0, 2, 2, true, false)]
        [TestCase(0, 2, Limits.Quantity - 1, false, false)]
        [TestCase(0, 2, Limits.Quantity - 1, true, false)]
        [TestCase(0, 2, Limits.Quantity, false, false)]
        [TestCase(0, 2, Limits.Quantity, true, false)]
        [TestCase(0, 2, Limits.Quantity + 1, false, false)]
        [TestCase(0, 2, Limits.Quantity + 1, true, false)]
        [TestCase(0, Limits.Die - 1, -2, false, false)]
        [TestCase(0, Limits.Die - 1, -2, true, false)]
        [TestCase(0, Limits.Die - 1, -1, false, false)]
        [TestCase(0, Limits.Die - 1, -1, true, false)]
        [TestCase(0, Limits.Die - 1, 0, false, false)]
        [TestCase(0, Limits.Die - 1, 0, true, false)]
        [TestCase(0, Limits.Die - 1, 1, false, false)]
        [TestCase(0, Limits.Die - 1, 1, true, false)]
        [TestCase(0, Limits.Die - 1, 2, false, false)]
        [TestCase(0, Limits.Die - 1, 2, true, false)]
        [TestCase(0, Limits.Die - 1, Limits.Quantity - 1, false, false)]
        [TestCase(0, Limits.Die - 1, Limits.Quantity - 1, true, false)]
        [TestCase(0, Limits.Die - 1, Limits.Quantity, false, false)]
        [TestCase(0, Limits.Die - 1, Limits.Quantity, true, false)]
        [TestCase(0, Limits.Die - 1, Limits.Quantity + 1, false, false)]
        [TestCase(0, Limits.Die - 1, Limits.Quantity + 1, true, false)]
        [TestCase(0, Limits.Die, -2, false, false)]
        [TestCase(0, Limits.Die, -2, true, false)]
        [TestCase(0, Limits.Die, -1, false, false)]
        [TestCase(0, Limits.Die, -1, true, false)]
        [TestCase(0, Limits.Die, 0, false, false)]
        [TestCase(0, Limits.Die, 0, true, false)]
        [TestCase(0, Limits.Die, 1, false, false)]
        [TestCase(0, Limits.Die, 1, true, false)]
        [TestCase(0, Limits.Die, 2, false, false)]
        [TestCase(0, Limits.Die, 2, true, false)]
        [TestCase(0, Limits.Die, Limits.Quantity - 1, false, false)]
        [TestCase(0, Limits.Die, Limits.Quantity - 1, true, false)]
        [TestCase(0, Limits.Die, Limits.Quantity, false, false)]
        [TestCase(0, Limits.Die, Limits.Quantity, true, false)]
        [TestCase(0, Limits.Die, Limits.Quantity + 1, false, false)]
        [TestCase(0, Limits.Die, Limits.Quantity + 1, true, false)]
        [TestCase(0, Limits.Die + 1, -2, false, false)]
        [TestCase(0, Limits.Die + 1, -2, true, false)]
        [TestCase(0, Limits.Die + 1, -1, false, false)]
        [TestCase(0, Limits.Die + 1, -1, true, false)]
        [TestCase(0, Limits.Die + 1, 0, false, false)]
        [TestCase(0, Limits.Die + 1, 0, true, false)]
        [TestCase(0, Limits.Die + 1, 1, false, false)]
        [TestCase(0, Limits.Die + 1, 1, true, false)]
        [TestCase(0, Limits.Die + 1, 2, false, false)]
        [TestCase(0, Limits.Die + 1, 2, true, false)]
        [TestCase(0, Limits.Die + 1, Limits.Quantity - 1, false, false)]
        [TestCase(0, Limits.Die + 1, Limits.Quantity - 1, true, false)]
        [TestCase(0, Limits.Die + 1, Limits.Quantity, false, false)]
        [TestCase(0, Limits.Die + 1, Limits.Quantity, true, false)]
        [TestCase(0, Limits.Die + 1, Limits.Quantity + 1, false, false)]
        [TestCase(0, Limits.Die + 1, Limits.Quantity + 1, true, false)]
        [TestCase(1, -1, -2, false, false)]
        [TestCase(1, -1, -2, true, false)]
        [TestCase(1, -1, -1, false, false)]
        [TestCase(1, -1, -1, true, false)]
        [TestCase(1, -1, 0, false, false)]
        [TestCase(1, -1, 0, true, false)]
        [TestCase(1, -1, 1, false, false)]
        [TestCase(1, -1, 1, true, false)]
        [TestCase(1, -1, 2, false, false)]
        [TestCase(1, -1, 2, true, false)]
        [TestCase(1, -1, Limits.Quantity - 1, false, false)]
        [TestCase(1, -1, Limits.Quantity - 1, true, false)]
        [TestCase(1, -1, Limits.Quantity, false, false)]
        [TestCase(1, -1, Limits.Quantity, true, false)]
        [TestCase(1, -1, Limits.Quantity + 1, false, false)]
        [TestCase(1, -1, Limits.Quantity + 1, true, false)]
        [TestCase(1, 0, -2, false, false)]
        [TestCase(1, 0, -2, true, false)]
        [TestCase(1, 0, -1, false, false)]
        [TestCase(1, 0, -1, true, false)]
        [TestCase(1, 0, 0, false, false)]
        [TestCase(1, 0, 0, true, false)]
        [TestCase(1, 0, 1, false, false)]
        [TestCase(1, 0, 1, true, false)]
        [TestCase(1, 0, 2, false, false)]
        [TestCase(1, 0, 2, true, false)]
        [TestCase(1, 0, Limits.Quantity - 1, false, false)]
        [TestCase(1, 0, Limits.Quantity - 1, true, false)]
        [TestCase(1, 0, Limits.Quantity, false, false)]
        [TestCase(1, 0, Limits.Quantity, true, false)]
        [TestCase(1, 0, Limits.Quantity + 1, false, false)]
        [TestCase(1, 0, Limits.Quantity + 1, true, false)]
        [TestCase(1, 1, -2, false, false)]
        [TestCase(1, 1, -2, true, false)]
        [TestCase(1, 1, -1, false, false)]
        [TestCase(1, 1, -1, true, false)]
        [TestCase(1, 1, 0, false, true)]
        [TestCase(1, 1, 0, true, false)]
        [TestCase(1, 1, 1, false, true)]
        [TestCase(1, 1, 1, true, false)]
        [TestCase(1, 1, 2, false, true)]
        [TestCase(1, 1, 2, true, false)]
        [TestCase(1, 1, Limits.Quantity - 1, false, true)]
        [TestCase(1, 1, Limits.Quantity - 1, true, false)]
        [TestCase(1, 1, Limits.Quantity, false, true)]
        [TestCase(1, 1, Limits.Quantity, true, false)]
        [TestCase(1, 1, Limits.Quantity + 1, false, false)]
        [TestCase(1, 1, Limits.Quantity + 1, true, false)]
        [TestCase(1, 2, -2, false, false)]
        [TestCase(1, 2, -2, true, false)]
        [TestCase(1, 2, -1, false, false)]
        [TestCase(1, 2, -1, true, false)]
        [TestCase(1, 2, 0, false, true)]
        [TestCase(1, 2, 0, true, true)]
        [TestCase(1, 2, 1, false, true)]
        [TestCase(1, 2, 1, true, true)]
        [TestCase(1, 2, 2, false, true)]
        [TestCase(1, 2, 2, true, true)]
        [TestCase(1, 2, Limits.Quantity - 1, false, true)]
        [TestCase(1, 2, Limits.Quantity - 1, true, true)]
        [TestCase(1, 2, Limits.Quantity, false, true)]
        [TestCase(1, 2, Limits.Quantity, true, true)]
        [TestCase(1, 2, Limits.Quantity + 1, false, false)]
        [TestCase(1, 2, Limits.Quantity + 1, true, false)]
        [TestCase(1, Limits.Die - 1, -2, false, false)]
        [TestCase(1, Limits.Die - 1, -2, true, false)]
        [TestCase(1, Limits.Die - 1, -1, false, false)]
        [TestCase(1, Limits.Die - 1, -1, true, false)]
        [TestCase(1, Limits.Die - 1, 0, false, true)]
        [TestCase(1, Limits.Die - 1, 0, true, true)]
        [TestCase(1, Limits.Die - 1, 1, false, true)]
        [TestCase(1, Limits.Die - 1, 1, true, true)]
        [TestCase(1, Limits.Die - 1, 2, false, true)]
        [TestCase(1, Limits.Die - 1, 2, true, true)]
        [TestCase(1, Limits.Die - 1, Limits.Quantity - 1, false, true)]
        [TestCase(1, Limits.Die - 1, Limits.Quantity - 1, true, true)]
        [TestCase(1, Limits.Die - 1, Limits.Quantity, false, true)]
        [TestCase(1, Limits.Die - 1, Limits.Quantity, true, true)]
        [TestCase(1, Limits.Die - 1, Limits.Quantity + 1, false, false)]
        [TestCase(1, Limits.Die - 1, Limits.Quantity + 1, true, false)]
        [TestCase(1, Limits.Die, -2, false, false)]
        [TestCase(1, Limits.Die, -2, true, false)]
        [TestCase(1, Limits.Die, -1, false, false)]
        [TestCase(1, Limits.Die, -1, true, false)]
        [TestCase(1, Limits.Die, 0, false, true)]
        [TestCase(1, Limits.Die, 0, true, true)]
        [TestCase(1, Limits.Die, 1, false, true)]
        [TestCase(1, Limits.Die, 1, true, true)]
        [TestCase(1, Limits.Die, 2, false, true)]
        [TestCase(1, Limits.Die, 2, true, true)]
        [TestCase(1, Limits.Die, Limits.Quantity - 1, false, true)]
        [TestCase(1, Limits.Die, Limits.Quantity - 1, true, true)]
        [TestCase(1, Limits.Die, Limits.Quantity, false, true)]
        [TestCase(1, Limits.Die, Limits.Quantity, true, true)]
        [TestCase(1, Limits.Die, Limits.Quantity + 1, false, false)]
        [TestCase(1, Limits.Die, Limits.Quantity + 1, true, false)]
        [TestCase(1, Limits.Die + 1, -2, false, false)]
        [TestCase(1, Limits.Die + 1, -2, true, false)]
        [TestCase(1, Limits.Die + 1, -1, false, false)]
        [TestCase(1, Limits.Die + 1, -1, true, false)]
        [TestCase(1, Limits.Die + 1, 0, false, false)]
        [TestCase(1, Limits.Die + 1, 0, true, false)]
        [TestCase(1, Limits.Die + 1, 1, false, false)]
        [TestCase(1, Limits.Die + 1, 1, true, false)]
        [TestCase(1, Limits.Die + 1, 2, false, false)]
        [TestCase(1, Limits.Die + 1, 2, true, false)]
        [TestCase(1, Limits.Die + 1, Limits.Quantity - 1, false, false)]
        [TestCase(1, Limits.Die + 1, Limits.Quantity - 1, true, false)]
        [TestCase(1, Limits.Die + 1, Limits.Quantity, false, false)]
        [TestCase(1, Limits.Die + 1, Limits.Quantity, true, false)]
        [TestCase(1, Limits.Die + 1, Limits.Quantity + 1, false, false)]
        [TestCase(1, Limits.Die + 1, Limits.Quantity + 1, true, false)]
        [TestCase(2, -1, -2, false, false)]
        [TestCase(2, -1, -2, true, false)]
        [TestCase(2, -1, -1, false, false)]
        [TestCase(2, -1, -1, true, false)]
        [TestCase(2, -1, 0, false, false)]
        [TestCase(2, -1, 0, true, false)]
        [TestCase(2, -1, 1, false, false)]
        [TestCase(2, -1, 1, true, false)]
        [TestCase(2, -1, 2, false, false)]
        [TestCase(2, -1, 2, true, false)]
        [TestCase(2, -1, Limits.Quantity - 1, false, false)]
        [TestCase(2, -1, Limits.Quantity - 1, true, false)]
        [TestCase(2, -1, Limits.Quantity, false, false)]
        [TestCase(2, -1, Limits.Quantity, true, false)]
        [TestCase(2, -1, Limits.Quantity + 1, false, false)]
        [TestCase(2, -1, Limits.Quantity + 1, true, false)]
        [TestCase(2, 0, -2, false, false)]
        [TestCase(2, 0, -2, true, false)]
        [TestCase(2, 0, -1, false, false)]
        [TestCase(2, 0, -1, true, false)]
        [TestCase(2, 0, 0, false, false)]
        [TestCase(2, 0, 0, true, false)]
        [TestCase(2, 0, 1, false, false)]
        [TestCase(2, 0, 1, true, false)]
        [TestCase(2, 0, 2, false, false)]
        [TestCase(2, 0, 2, true, false)]
        [TestCase(2, 0, Limits.Quantity - 1, false, false)]
        [TestCase(2, 0, Limits.Quantity - 1, true, false)]
        [TestCase(2, 0, Limits.Quantity, false, false)]
        [TestCase(2, 0, Limits.Quantity, true, false)]
        [TestCase(2, 0, Limits.Quantity + 1, false, false)]
        [TestCase(2, 0, Limits.Quantity + 1, true, false)]
        [TestCase(2, 1, -2, false, false)]
        [TestCase(2, 1, -2, true, false)]
        [TestCase(2, 1, -1, false, false)]
        [TestCase(2, 1, -1, true, false)]
        [TestCase(2, 1, 0, false, true)]
        [TestCase(2, 1, 0, true, false)]
        [TestCase(2, 1, 1, false, true)]
        [TestCase(2, 1, 1, true, false)]
        [TestCase(2, 1, 2, false, true)]
        [TestCase(2, 1, 2, true, false)]
        [TestCase(2, 1, Limits.Quantity - 1, false, true)]
        [TestCase(2, 1, Limits.Quantity - 1, true, false)]
        [TestCase(2, 1, Limits.Quantity, false, true)]
        [TestCase(2, 1, Limits.Quantity, true, false)]
        [TestCase(2, 1, Limits.Quantity + 1, false, false)]
        [TestCase(2, 1, Limits.Quantity + 1, true, false)]
        [TestCase(2, 2, -2, false, false)]
        [TestCase(2, 2, -2, true, false)]
        [TestCase(2, 2, -1, false, false)]
        [TestCase(2, 2, -1, true, false)]
        [TestCase(2, 2, 0, false, true)]
        [TestCase(2, 2, 0, true, true)]
        [TestCase(2, 2, 1, false, true)]
        [TestCase(2, 2, 1, true, true)]
        [TestCase(2, 2, 2, false, true)]
        [TestCase(2, 2, 2, true, true)]
        [TestCase(2, 2, Limits.Quantity - 1, false, true)]
        [TestCase(2, 2, Limits.Quantity - 1, true, true)]
        [TestCase(2, 2, Limits.Quantity, false, true)]
        [TestCase(2, 2, Limits.Quantity, true, true)]
        [TestCase(2, 2, Limits.Quantity + 1, false, false)]
        [TestCase(2, 2, Limits.Quantity + 1, true, false)]
        [TestCase(2, Limits.Die - 1, -2, false, false)]
        [TestCase(2, Limits.Die - 1, -2, true, false)]
        [TestCase(2, Limits.Die - 1, -1, false, false)]
        [TestCase(2, Limits.Die - 1, -1, true, false)]
        [TestCase(2, Limits.Die - 1, 0, false, true)]
        [TestCase(2, Limits.Die - 1, 0, true, true)]
        [TestCase(2, Limits.Die - 1, 1, false, true)]
        [TestCase(2, Limits.Die - 1, 1, true, true)]
        [TestCase(2, Limits.Die - 1, 2, false, true)]
        [TestCase(2, Limits.Die - 1, 2, true, true)]
        [TestCase(2, Limits.Die - 1, Limits.Quantity - 1, false, true)]
        [TestCase(2, Limits.Die - 1, Limits.Quantity - 1, true, true)]
        [TestCase(2, Limits.Die - 1, Limits.Quantity, false, true)]
        [TestCase(2, Limits.Die - 1, Limits.Quantity, true, true)]
        [TestCase(2, Limits.Die - 1, Limits.Quantity + 1, false, false)]
        [TestCase(2, Limits.Die - 1, Limits.Quantity + 1, true, false)]
        [TestCase(2, Limits.Die, -2, false, false)]
        [TestCase(2, Limits.Die, -2, true, false)]
        [TestCase(2, Limits.Die, -1, false, false)]
        [TestCase(2, Limits.Die, -1, true, false)]
        [TestCase(2, Limits.Die, 0, false, true)]
        [TestCase(2, Limits.Die, 0, true, true)]
        [TestCase(2, Limits.Die, 1, false, true)]
        [TestCase(2, Limits.Die, 1, true, true)]
        [TestCase(2, Limits.Die, 2, false, true)]
        [TestCase(2, Limits.Die, 2, true, true)]
        [TestCase(2, Limits.Die, Limits.Quantity - 1, false, true)]
        [TestCase(2, Limits.Die, Limits.Quantity - 1, true, true)]
        [TestCase(2, Limits.Die, Limits.Quantity, false, true)]
        [TestCase(2, Limits.Die, Limits.Quantity, true, true)]
        [TestCase(2, Limits.Die, Limits.Quantity + 1, false, false)]
        [TestCase(2, Limits.Die, Limits.Quantity + 1, true, false)]
        [TestCase(2, Limits.Die + 1, -2, false, false)]
        [TestCase(2, Limits.Die + 1, -2, true, false)]
        [TestCase(2, Limits.Die + 1, -1, false, false)]
        [TestCase(2, Limits.Die + 1, -1, true, false)]
        [TestCase(2, Limits.Die + 1, 0, false, false)]
        [TestCase(2, Limits.Die + 1, 0, true, false)]
        [TestCase(2, Limits.Die + 1, 1, false, false)]
        [TestCase(2, Limits.Die + 1, 1, true, false)]
        [TestCase(2, Limits.Die + 1, 2, false, false)]
        [TestCase(2, Limits.Die + 1, 2, true, false)]
        [TestCase(2, Limits.Die + 1, Limits.Quantity - 1, false, false)]
        [TestCase(2, Limits.Die + 1, Limits.Quantity - 1, true, false)]
        [TestCase(2, Limits.Die + 1, Limits.Quantity, false, false)]
        [TestCase(2, Limits.Die + 1, Limits.Quantity, true, false)]
        [TestCase(2, Limits.Die + 1, Limits.Quantity + 1, false, false)]
        [TestCase(2, Limits.Die + 1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity - 1, -1, -2, false, false)]
        [TestCase(Limits.Quantity - 1, -1, -2, true, false)]
        [TestCase(Limits.Quantity - 1, -1, -1, false, false)]
        [TestCase(Limits.Quantity - 1, -1, -1, true, false)]
        [TestCase(Limits.Quantity - 1, -1, 0, false, false)]
        [TestCase(Limits.Quantity - 1, -1, 0, true, false)]
        [TestCase(Limits.Quantity - 1, -1, 1, false, false)]
        [TestCase(Limits.Quantity - 1, -1, 1, true, false)]
        [TestCase(Limits.Quantity - 1, -1, 2, false, false)]
        [TestCase(Limits.Quantity - 1, -1, 2, true, false)]
        [TestCase(Limits.Quantity - 1, -1, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity - 1, -1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity - 1, -1, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity - 1, -1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity - 1, -1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity - 1, -1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity - 1, 0, -2, false, false)]
        [TestCase(Limits.Quantity - 1, 0, -2, true, false)]
        [TestCase(Limits.Quantity - 1, 0, -1, false, false)]
        [TestCase(Limits.Quantity - 1, 0, -1, true, false)]
        [TestCase(Limits.Quantity - 1, 0, 0, false, false)]
        [TestCase(Limits.Quantity - 1, 0, 0, true, false)]
        [TestCase(Limits.Quantity - 1, 0, 1, false, false)]
        [TestCase(Limits.Quantity - 1, 0, 1, true, false)]
        [TestCase(Limits.Quantity - 1, 0, 2, false, false)]
        [TestCase(Limits.Quantity - 1, 0, 2, true, false)]
        [TestCase(Limits.Quantity - 1, 0, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity - 1, 0, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity - 1, 0, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity - 1, 0, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity - 1, 0, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity - 1, 0, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity - 1, 1, -2, false, false)]
        [TestCase(Limits.Quantity - 1, 1, -2, true, false)]
        [TestCase(Limits.Quantity - 1, 1, -1, false, false)]
        [TestCase(Limits.Quantity - 1, 1, -1, true, false)]
        [TestCase(Limits.Quantity - 1, 1, 0, false, true)]
        [TestCase(Limits.Quantity - 1, 1, 0, true, false)]
        [TestCase(Limits.Quantity - 1, 1, 1, false, true)]
        [TestCase(Limits.Quantity - 1, 1, 1, true, false)]
        [TestCase(Limits.Quantity - 1, 1, 2, false, true)]
        [TestCase(Limits.Quantity - 1, 1, 2, true, false)]
        [TestCase(Limits.Quantity - 1, 1, Limits.Quantity - 1, false, true)]
        [TestCase(Limits.Quantity - 1, 1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity - 1, 1, Limits.Quantity, false, true)]
        [TestCase(Limits.Quantity - 1, 1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity - 1, 1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity - 1, 1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity - 1, 2, -2, false, false)]
        [TestCase(Limits.Quantity - 1, 2, -2, true, false)]
        [TestCase(Limits.Quantity - 1, 2, -1, false, false)]
        [TestCase(Limits.Quantity - 1, 2, -1, true, false)]
        [TestCase(Limits.Quantity - 1, 2, 0, false, true)]
        [TestCase(Limits.Quantity - 1, 2, 0, true, true)]
        [TestCase(Limits.Quantity - 1, 2, 1, false, true)]
        [TestCase(Limits.Quantity - 1, 2, 1, true, true)]
        [TestCase(Limits.Quantity - 1, 2, 2, false, true)]
        [TestCase(Limits.Quantity - 1, 2, 2, true, true)]
        [TestCase(Limits.Quantity - 1, 2, Limits.Quantity - 1, false, true)]
        [TestCase(Limits.Quantity - 1, 2, Limits.Quantity - 1, true, true)]
        [TestCase(Limits.Quantity - 1, 2, Limits.Quantity, false, true)]
        [TestCase(Limits.Quantity - 1, 2, Limits.Quantity, true, true)]
        [TestCase(Limits.Quantity - 1, 2, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity - 1, 2, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, -2, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, -2, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, -1, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, -1, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, 0, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, 0, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, 1, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, 1, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, 2, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, 2, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, Limits.Quantity - 1, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, Limits.Quantity - 1, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, Limits.Quantity, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, Limits.Quantity, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die, -2, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die, -2, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die, -1, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die, -1, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die, 0, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, 0, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, 1, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, 1, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, 2, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, 2, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, Limits.Quantity - 1, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, Limits.Quantity - 1, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, Limits.Quantity, false, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, Limits.Quantity, true, true)]
        [TestCase(Limits.Quantity - 1, Limits.Die, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, -2, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, -2, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, -1, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, -1, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, 0, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, 0, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, 1, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, 1, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, 2, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, 2, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity - 1, Limits.Die + 1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity, -1, -2, false, false)]
        [TestCase(Limits.Quantity, -1, -2, true, false)]
        [TestCase(Limits.Quantity, -1, -1, false, false)]
        [TestCase(Limits.Quantity, -1, -1, true, false)]
        [TestCase(Limits.Quantity, -1, 0, false, false)]
        [TestCase(Limits.Quantity, -1, 0, true, false)]
        [TestCase(Limits.Quantity, -1, 1, false, false)]
        [TestCase(Limits.Quantity, -1, 1, true, false)]
        [TestCase(Limits.Quantity, -1, 2, false, false)]
        [TestCase(Limits.Quantity, -1, 2, true, false)]
        [TestCase(Limits.Quantity, -1, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity, -1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity, -1, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity, -1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity, -1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity, -1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity, 0, -2, false, false)]
        [TestCase(Limits.Quantity, 0, -2, true, false)]
        [TestCase(Limits.Quantity, 0, -1, false, false)]
        [TestCase(Limits.Quantity, 0, -1, true, false)]
        [TestCase(Limits.Quantity, 0, 0, false, false)]
        [TestCase(Limits.Quantity, 0, 0, true, false)]
        [TestCase(Limits.Quantity, 0, 1, false, false)]
        [TestCase(Limits.Quantity, 0, 1, true, false)]
        [TestCase(Limits.Quantity, 0, 2, false, false)]
        [TestCase(Limits.Quantity, 0, 2, true, false)]
        [TestCase(Limits.Quantity, 0, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity, 0, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity, 0, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity, 0, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity, 0, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity, 0, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity, 1, -2, false, false)]
        [TestCase(Limits.Quantity, 1, -2, true, false)]
        [TestCase(Limits.Quantity, 1, -1, false, false)]
        [TestCase(Limits.Quantity, 1, -1, true, false)]
        [TestCase(Limits.Quantity, 1, 0, false, true)]
        [TestCase(Limits.Quantity, 1, 0, true, false)]
        [TestCase(Limits.Quantity, 1, 1, false, true)]
        [TestCase(Limits.Quantity, 1, 1, true, false)]
        [TestCase(Limits.Quantity, 1, 2, false, true)]
        [TestCase(Limits.Quantity, 1, 2, true, false)]
        [TestCase(Limits.Quantity, 1, Limits.Quantity - 1, false, true)]
        [TestCase(Limits.Quantity, 1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity, 1, Limits.Quantity, false, true)]
        [TestCase(Limits.Quantity, 1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity, 1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity, 1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity, 2, -2, false, false)]
        [TestCase(Limits.Quantity, 2, -2, true, false)]
        [TestCase(Limits.Quantity, 2, -1, false, false)]
        [TestCase(Limits.Quantity, 2, -1, true, false)]
        [TestCase(Limits.Quantity, 2, 0, false, true)]
        [TestCase(Limits.Quantity, 2, 0, true, true)]
        [TestCase(Limits.Quantity, 2, 1, false, true)]
        [TestCase(Limits.Quantity, 2, 1, true, true)]
        [TestCase(Limits.Quantity, 2, 2, false, true)]
        [TestCase(Limits.Quantity, 2, 2, true, true)]
        [TestCase(Limits.Quantity, 2, Limits.Quantity - 1, false, true)]
        [TestCase(Limits.Quantity, 2, Limits.Quantity - 1, true, true)]
        [TestCase(Limits.Quantity, 2, Limits.Quantity, false, true)]
        [TestCase(Limits.Quantity, 2, Limits.Quantity, true, true)]
        [TestCase(Limits.Quantity, 2, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity, 2, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity, Limits.Die - 1, -2, false, false)]
        [TestCase(Limits.Quantity, Limits.Die - 1, -2, true, false)]
        [TestCase(Limits.Quantity, Limits.Die - 1, -1, false, false)]
        [TestCase(Limits.Quantity, Limits.Die - 1, -1, true, false)]
        [TestCase(Limits.Quantity, Limits.Die - 1, 0, false, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, 0, true, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, 1, false, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, 1, true, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, 2, false, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, 2, true, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, Limits.Quantity - 1, false, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, Limits.Quantity - 1, true, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, Limits.Quantity, false, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, Limits.Quantity, true, true)]
        [TestCase(Limits.Quantity, Limits.Die - 1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity, Limits.Die - 1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity, Limits.Die, -2, false, false)]
        [TestCase(Limits.Quantity, Limits.Die, -2, true, false)]
        [TestCase(Limits.Quantity, Limits.Die, -1, false, false)]
        [TestCase(Limits.Quantity, Limits.Die, -1, true, false)]
        [TestCase(Limits.Quantity, Limits.Die, 0, false, true)]
        [TestCase(Limits.Quantity, Limits.Die, 0, true, true)]
        [TestCase(Limits.Quantity, Limits.Die, 1, false, true)]
        [TestCase(Limits.Quantity, Limits.Die, 1, true, true)]
        [TestCase(Limits.Quantity, Limits.Die, 2, false, true)]
        [TestCase(Limits.Quantity, Limits.Die, 2, true, true)]
        [TestCase(Limits.Quantity, Limits.Die, Limits.Quantity - 1, false, true)]
        [TestCase(Limits.Quantity, Limits.Die, Limits.Quantity - 1, true, true)]
        [TestCase(Limits.Quantity, Limits.Die, Limits.Quantity, false, true)]
        [TestCase(Limits.Quantity, Limits.Die, Limits.Quantity, true, true)]
        [TestCase(Limits.Quantity, Limits.Die, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity, Limits.Die, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, -2, false, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, -2, true, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, -1, false, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, -1, true, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, 0, false, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, 0, true, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, 1, false, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, 1, true, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, 2, false, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, 2, true, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity, Limits.Die + 1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity + 1, -1, -2, false, false)]
        [TestCase(Limits.Quantity + 1, -1, -2, true, false)]
        [TestCase(Limits.Quantity + 1, -1, -1, false, false)]
        [TestCase(Limits.Quantity + 1, -1, -1, true, false)]
        [TestCase(Limits.Quantity + 1, -1, 0, false, false)]
        [TestCase(Limits.Quantity + 1, -1, 0, true, false)]
        [TestCase(Limits.Quantity + 1, -1, 1, false, false)]
        [TestCase(Limits.Quantity + 1, -1, 1, true, false)]
        [TestCase(Limits.Quantity + 1, -1, 2, false, false)]
        [TestCase(Limits.Quantity + 1, -1, 2, true, false)]
        [TestCase(Limits.Quantity + 1, -1, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity + 1, -1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity + 1, -1, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity + 1, -1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity + 1, -1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity + 1, -1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity + 1, 0, -2, false, false)]
        [TestCase(Limits.Quantity + 1, 0, -2, true, false)]
        [TestCase(Limits.Quantity + 1, 0, -1, false, false)]
        [TestCase(Limits.Quantity + 1, 0, -1, true, false)]
        [TestCase(Limits.Quantity + 1, 0, 0, false, false)]
        [TestCase(Limits.Quantity + 1, 0, 0, true, false)]
        [TestCase(Limits.Quantity + 1, 0, 1, false, false)]
        [TestCase(Limits.Quantity + 1, 0, 1, true, false)]
        [TestCase(Limits.Quantity + 1, 0, 2, false, false)]
        [TestCase(Limits.Quantity + 1, 0, 2, true, false)]
        [TestCase(Limits.Quantity + 1, 0, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity + 1, 0, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity + 1, 0, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity + 1, 0, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity + 1, 0, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity + 1, 0, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity + 1, 1, -2, false, false)]
        [TestCase(Limits.Quantity + 1, 1, -2, true, false)]
        [TestCase(Limits.Quantity + 1, 1, -1, false, false)]
        [TestCase(Limits.Quantity + 1, 1, -1, true, false)]
        [TestCase(Limits.Quantity + 1, 1, 0, false, false)]
        [TestCase(Limits.Quantity + 1, 1, 0, true, false)]
        [TestCase(Limits.Quantity + 1, 1, 1, false, false)]
        [TestCase(Limits.Quantity + 1, 1, 1, true, false)]
        [TestCase(Limits.Quantity + 1, 1, 2, false, false)]
        [TestCase(Limits.Quantity + 1, 1, 2, true, false)]
        [TestCase(Limits.Quantity + 1, 1, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity + 1, 1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity + 1, 1, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity + 1, 1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity + 1, 1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity + 1, 1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity + 1, 2, -2, false, false)]
        [TestCase(Limits.Quantity + 1, 2, -2, true, false)]
        [TestCase(Limits.Quantity + 1, 2, -1, false, false)]
        [TestCase(Limits.Quantity + 1, 2, -1, true, false)]
        [TestCase(Limits.Quantity + 1, 2, 0, false, false)]
        [TestCase(Limits.Quantity + 1, 2, 0, true, false)]
        [TestCase(Limits.Quantity + 1, 2, 1, false, false)]
        [TestCase(Limits.Quantity + 1, 2, 1, true, false)]
        [TestCase(Limits.Quantity + 1, 2, 2, false, false)]
        [TestCase(Limits.Quantity + 1, 2, 2, true, false)]
        [TestCase(Limits.Quantity + 1, 2, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity + 1, 2, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity + 1, 2, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity + 1, 2, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity + 1, 2, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity + 1, 2, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, -2, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, -2, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, -1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, -1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, 0, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, 0, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, 1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, 1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, 2, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, 2, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die - 1, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, -2, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, -2, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, -1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, -1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, 0, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, 0, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, 1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, 1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, 2, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, 2, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die, Limits.Quantity + 1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, -2, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, -2, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, -1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, -1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, 0, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, 0, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, 1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, 1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, 2, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, 2, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, Limits.Quantity - 1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, Limits.Quantity - 1, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, Limits.Quantity, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, Limits.Quantity, true, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, Limits.Quantity + 1, false, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, Limits.Quantity + 1, true, false)]
        public void IsValid(int quantity, int die, int amountToKeep, bool explode, bool isValid)
        {
            roll.Quantity = quantity;
            roll.Die = die;
            roll.AmountToKeep = amountToKeep;
            roll.Explode = explode;

            Assert.That(roll.IsValid, Is.EqualTo(isValid));
        }

        [Test]
        public void IfGettingRollsAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetRolls(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll.\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingSumAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll.\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingAverageAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetPotentialAverage(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll.\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingTrueOrFalseAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetTrueOrFalse(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll.\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingMinimumAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetPotentialMinimum(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll.\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingMaximumAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetPotentialMaximum(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll.\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(Limits.Quantity + 1)]
        [TestCase(int.MaxValue)]
        public void IfQuantityNotValid_ThrowInvalidOperationException(int invalidQuantity)
        {
            roll.Quantity = invalidQuantity;
            roll.Die = 9266;

            var message = $"{invalidQuantity}d9266 is not a valid roll.";
            message += $"\n\tQuantity: 0 < {invalidQuantity} < {Limits.Quantity}";
            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(message));
        }

        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(Limits.Die + 1)]
        [TestCase(int.MaxValue)]
        public void IfDieNotValid_ThrowInvalidOperationException(int invalidDie)
        {
            roll.Quantity = 9266;
            roll.Die = invalidDie;

            var message = $"9266d{invalidDie} is not a valid roll.";
            message += $"\n\tDie: 0 < {invalidDie} < {Limits.Die}";
            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(message));
        }

        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(Limits.Quantity + 1)]
        [TestCase(int.MaxValue)]
        public void IfKeepNotValid_ThrowInvalidOperationException(int invalidKeep)
        {
            roll.Quantity = 9266;
            roll.Die = 42;
            roll.AmountToKeep = invalidKeep;

            var message = $"9266d42k{invalidKeep} is not a valid roll.";
            message += $"\n\tKeep: 0 <= {invalidKeep} < {Limits.Quantity}";
            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(message));
        }

        [Test]
        public void IfExplodeNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 9266;
            roll.Die = 1;
            roll.Explode = true;

            var message = $"9266d1! is not a valid roll.";
            message += $"\n\tExplode: Cannot explode die 1, must be > 1";
            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(message));
        }

        [Test]
        public void IfAllNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = Limits.Quantity + 1;
            roll.Die = 0;
            roll.AmountToKeep = -1;
            roll.Explode = true;

            var message = $"{Limits.Quantity + 1}d0!k-1 is not a valid roll.";
            message += $"\n\tQuantity: 0 < {Limits.Quantity + 1} < {Limits.Quantity}";
            message += $"\n\tDie: 0 < 0 < {Limits.Die}";
            message += $"\n\tKeep: 0 <= -1 < {Limits.Quantity}";
            message += $"\n\tExplode: Cannot explode die 0, must be > 1";
            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(message));
        }

        [Test]
        public void GetRolls()
        {
            roll.Quantity = 92;
            roll.Die = 66;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 0; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 27 ? 2 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(92).And.EqualTo(countTotal));
        }

        [Test]
        public void GetRollsAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 25; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 27 ? 2 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(42).And.EqualTo(countTotal));
        }

        [Test]
        public void GetRollsAndKeepDuplicates()
        {
            roll.Quantity = 92 * 2;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 47; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 53 && individualRoll > 48 ? 3 : 2;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(42).And.EqualTo(countTotal));
        }

        [Test]
        public void GetRollsAndExplode()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 0; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 28 ? 2 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(93).And.EqualTo(countTotal));
        }

        [Test]
        public void GetRollsAndExplodeAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;
            roll.AmountToKeep = 42;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 25; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll > 26 && individualRoll < 28 ? 2 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(42).And.EqualTo(countTotal));
        }

        [Test]
        public void GetRollsAndExplodeAndKeepDuplicates()
        {
            roll.Quantity = 92 * 2;
            roll.Die = 66;
            roll.Explode = true;
            roll.AmountToKeep = 42;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 48; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 55 ? 3 : 2;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(42).And.EqualTo(countTotal));
        }

        [Test]
        public void GetSumOfRolls()
        {
            roll.Quantity = 92;
            roll.Die = 66;

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(2562));
        }

        [Test]
        public void GetSumOfRollsAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(1912));
        }

        [Test]
        public void GetSumOfRollsAndExplode()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(2589));
        }

        [Test]
        public void GetSumOfRollsAndExplodeAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;
            roll.AmountToKeep = 42;

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(1913));
        }

        [Test]
        public void GetPotentialAverageRoll()
        {
            roll.Quantity = 92;
            roll.Die = 66;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(3082));
        }

        [Test]
        public void GetPotentialAverageRollAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(1407));
        }

        [Test]
        public void GetPotentialAverageRollAndExplode()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(3082));
        }

        [Test]
        public void GetPotentialAverageRollAndExplodeAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;
            roll.AmountToKeep = 42;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(1407));
        }

        [Test]
        public void GetPotentialMinimumRoll()
        {
            roll.Quantity = 92;
            roll.Die = 66;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(42));
        }

        [Test]
        public void GetPotentialMinimumRollAndExplode()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndExplodeAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;
            roll.AmountToKeep = 42;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(42));
        }

        [Test]
        public void GetPotentialMaximumRoll()
        {
            roll.Quantity = 92;
            roll.Die = 66;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 66));
        }

        [Test]
        public void GetPotentialMaximumRollAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(42 * 66));
        }

        [Test]
        public void GetPotentialMaximumRollAndExplode()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 66 * 10));
        }

        [Test]
        public void GetPotentialMaximumRollAndExplodeAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;
            roll.AmountToKeep = 42;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(42 * 66 * 10));
        }

        [Test]
        public void GetUnroundedPotentialAverageRoll()
        {
            roll.Quantity = 3;
            roll.Die = 2;
            roll.AmountToKeep = 0;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(4.5));
        }

        [Test]
        public void GetUnroundedPotentialAverageRollAndKeep()
        {
            roll.Quantity = 3;
            roll.Die = 2;
            roll.AmountToKeep = 1;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(1.5));
        }

        [Test]
        public void GetTrueIfHigh()
        {
            roll.Quantity = 92;
            roll.Die = 66;

            mockRandom.Setup(r => r.Next(66)).Returns(34);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetTrueIfHighAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            mockRandom.Setup(r => r.Next(66)).Returns(34);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetFalseIfLow()
        {
            roll.Quantity = 92;
            roll.Die = 66;

            mockRandom.Setup(r => r.Next(66)).Returns(32);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetFalseIfLowAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            mockRandom.Setup(r => r.Next(66)).Returns(32);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetTrueIfOnAverageExactly()
        {
            roll.Quantity = 3;
            roll.Die = 3;
            roll.AmountToKeep = 0;

            mockRandom.Setup(r => r.Next(3)).Returns(1);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetTrueIfOnAverageExactlyAndKeep()
        {
            roll.Quantity = 3;
            roll.Die = 3;
            roll.AmountToKeep = 1;

            mockRandom.Setup(r => r.Next(3)).Returns(1);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.True);
        }

        [Test]
        public void RollString()
        {
            roll.Quantity = 92;
            roll.Die = 66;

            Assert.That(roll.ToString(), Is.EqualTo("92d66"));
        }

        [Test]
        public void RollStringWithKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            Assert.That(roll.ToString(), Is.EqualTo("92d66k42"));
        }

        //INFO: This roll is not valid, but good to know for identification
        [Test]
        public void RollStringWithNegativeKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = -42;

            Assert.That(roll.ToString(), Is.EqualTo("92d66k-42"));
        }

        [Test]
        public void RollStringWithExplode()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;

            Assert.That(roll.ToString(), Is.EqualTo("92d66!"));
        }

        [Test]
        public void RollStringWithExplodeAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;
            roll.AmountToKeep = 42;

            Assert.That(roll.ToString(), Is.EqualTo("92d66!k42"));
        }

        //INFO: This roll is not valid, but good to know for identification
        [Test]
        public void RollStringWithExplodeAndNegativeKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Explode = true;
            roll.AmountToKeep = -42;

            Assert.That(roll.ToString(), Is.EqualTo("92d66!k-42"));
        }
    }
}
