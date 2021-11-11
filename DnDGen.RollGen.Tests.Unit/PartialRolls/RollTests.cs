using DnDGen.RollGen.PartialRolls;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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

        [TestCaseSource(nameof(ParsedExpressions))]
        public void ParseExpression(string expression, int quantity, int die, int toKeep, int[] explodes, Dictionary<int, int> transforms)
        {
            roll = new Roll(expression);
            Assert.That(roll.Quantity, Is.EqualTo(quantity));
            Assert.That(roll.Die, Is.EqualTo(die));
            Assert.That(roll.ExplodeOn, Is.EquivalentTo(explodes));
            Assert.That(roll.AmountToKeep, Is.EqualTo(toKeep));
            Assert.That(roll.Transforms, Is.EquivalentTo(transforms));
            Assert.That(roll.IsValid, Is.True);
        }

        private static IEnumerable ParsedExpressions
        {
            get
            {
                yield return new TestCaseData("1d2!", 1, 2, 0, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData("1d2!!", 1, 2, 0, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData(" 1 d 2 ", 1, 2, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData(" 1 d 2 ! ", 1, 2, 0, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData(" 1 d 2 ! ! ", 1, 2, 0, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData("1d2k3", 1, 2, 3, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("1d2!k3", 1, 2, 3, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData("1d2k3!", 1, 2, 3, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData("1d2!k3!", 1, 2, 3, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData(" 1 d 2 k 3 ", 1, 2, 3, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData(" 1 d 2 ! k 3 ", 1, 2, 3, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData(" 1 d 2 k 3 ! ", 1, 2, 3, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData(" 1 d 2 ! k 3 ! ", 1, 2, 3, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData("d2", 1, 2, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("d2!", 1, 2, 0, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData("d2!!", 1, 2, 0, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData(" d 2 ", 1, 2, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData(" d 2 ! ", 1, 2, 0, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData(" d 2 ! ! ", 1, 2, 0, new[] { 2 }, new Dictionary<int, int>());
                yield return new TestCaseData("1230d456k789", 1230, 456, 789, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("1230d456!k789", 1230, 456, 789, new[] { 456 }, new Dictionary<int, int>());
                yield return new TestCaseData("1230d456k789!", 1230, 456, 789, new[] { 456 }, new Dictionary<int, int>());
                yield return new TestCaseData("1230d456!k789!", 1230, 456, 789, new[] { 456 }, new Dictionary<int, int>());
                yield return new TestCaseData(" 1230 d 456 k 789 ", 1230, 456, 789, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData(" 1230 d 456 ! k 789 ", 1230, 456, 789, new[] { 456 }, new Dictionary<int, int>());
                yield return new TestCaseData(" 1230 d 456 k 789 ! ", 1230, 456, 789, new[] { 456 }, new Dictionary<int, int>());
                yield return new TestCaseData(" 1230 d 456 ! k 789 ! ", 1230, 456, 789, new[] { 456 }, new Dictionary<int, int>());
                yield return new TestCaseData("92d66k42", 92, 66, 42, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("92 d 66 k 42 ", 92, 66, 42, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("92d66!k42", 92, 66, 42, new[] { 66 }, new Dictionary<int, int>());
                yield return new TestCaseData("92d66k42!", 92, 66, 42, new[] { 66 }, new Dictionary<int, int>());
                yield return new TestCaseData("92d66!k42!", 92, 66, 42, new[] { 66 }, new Dictionary<int, int>());
                yield return new TestCaseData("92 d 66 ! k 42 ", 92, 66, 42, new[] { 66 }, new Dictionary<int, int>());
                yield return new TestCaseData("92 d 66 k 42 ! ", 92, 66, 42, new[] { 66 }, new Dictionary<int, int>());
                yield return new TestCaseData("92 d 66 ! k 42 ! ", 92, 66, 42, new[] { 66 }, new Dictionary<int, int>());
                yield return new TestCaseData("3d6", 3, 6, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("3d6t1t2", 3, 6, 0, new int[0], new Dictionary<int, int> { { 1, 6 }, { 2, 6 } });
                yield return new TestCaseData("3d6t1:2t3:4k5", 3, 6, 5, new int[0], new Dictionary<int, int> { { 1, 2 }, { 3, 4 } });
                yield return new TestCaseData("3d6t1t3:4k5", 3, 6, 5, new int[0], new Dictionary<int, int> { { 1, 6 }, { 3, 4 } });
                yield return new TestCaseData("3d6t1:2t3k5", 3, 6, 5, new int[0], new Dictionary<int, int> { { 1, 2 }, { 3, 6 } });
                yield return new TestCaseData("4d6!t1t2k3", 4, 6, 3, new[] { 6 }, new Dictionary<int, int> { { 1, 6 }, { 2, 6 } });
                //From README
                yield return new TestCaseData("4d6", 4, 6, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("92d66", 92, 66, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("3d4", 3, 4, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("1d6", 1, 6, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("2d6", 2, 6, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("3d6", 3, 6, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("5d6", 5, 6, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("4d6k3", 4, 6, 3, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("3d4k2", 3, 4, 2, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("1d8", 1, 8, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("1d2", 1, 2, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("4d3", 4, 3, 0, new int[0], new Dictionary<int, int>());
                yield return new TestCaseData("4d6!", 4, 6, 0, new[] { 6 }, new Dictionary<int, int>());
                yield return new TestCaseData("3d4!", 3, 4, 0, new[] { 4 }, new Dictionary<int, int>());
                yield return new TestCaseData("3d4!k2", 3, 4, 2, new[] { 4 }, new Dictionary<int, int>());
                yield return new TestCaseData("3d4!e3", 3, 4, 0, new[] { 4, 3 }, new Dictionary<int, int>());
                yield return new TestCaseData("3d4e1e2k2", 3, 4, 2, new[] { 1, 2 }, new Dictionary<int, int>());
                yield return new TestCaseData("3d6t1", 3, 6, 0, new int[0], new Dictionary<int, int> { { 1, 6 } });
                yield return new TestCaseData("3d6t1t5", 3, 6, 0, new int[0], new Dictionary<int, int> { { 1, 6 }, { 5, 6 } });
                yield return new TestCaseData("3d6!t1k2", 3, 6, 2, new[] { 6 }, new Dictionary<int, int> { { 1, 6 } });
                yield return new TestCaseData("3d6t1:2", 3, 6, 0, new int[0], new Dictionary<int, int> { { 1, 2 } });
                yield return new TestCaseData("4d3t2k1", 4, 3, 1, new int[0], new Dictionary<int, int> { { 2, 3 } });
                yield return new TestCaseData("4d3k1t2", 4, 3, 1, new int[0], new Dictionary<int, int> { { 2, 3 } });
                yield return new TestCaseData("4d3!t2k1", 4, 3, 1, new[] { 3 }, new Dictionary<int, int> { { 2, 3 } });
                yield return new TestCaseData("4d3!k1t2", 4, 3, 1, new[] { 3 }, new Dictionary<int, int> { { 2, 3 } });
                yield return new TestCaseData("4d3t2!k1", 4, 3, 1, new[] { 3 }, new Dictionary<int, int> { { 2, 3 } });
                yield return new TestCaseData("4d3k1!t2", 4, 3, 1, new[] { 3 }, new Dictionary<int, int> { { 2, 3 } });
                yield return new TestCaseData("4d3t2k1!", 4, 3, 1, new[] { 3 }, new Dictionary<int, int> { { 2, 3 } });
                yield return new TestCaseData("4d3k1t2!", 4, 3, 1, new[] { 3 }, new Dictionary<int, int> { { 2, 3 } });
            }
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
        [TestCase("3d6", true)]
        [TestCase("4d6k3", true)]
        [TestCase("3d6t1", true)]
        [TestCase("3d6t1t2", true)]
        [TestCase("4d6!t1t2k3", true)]
        //From README
        [TestCase("4d6", true)]
        [TestCase("92d66", true)]
        [TestCase("5+3d4*2", false)]
        [TestCase("((1d2)d5k1)d6", false)]
        [TestCase("4d6k3", true)]
        [TestCase("3d4k2", true)]
        [TestCase("5+3d4*3", false)]
        [TestCase("1d6+3", false)]
        [TestCase("1d8+1d2-1", false)]
        [TestCase("4d3-3", false)]
        [TestCase("4d6!", true)]
        [TestCase("3d4!", true)]
        [TestCase("3d4!k2", true)]
        [TestCase("3d4!e3", true)]
        [TestCase("3d4e1e2k2", true)]
        [TestCase("3d6t1", true)]
        [TestCase("3d6t1t5", true)]
        [TestCase("3d6!t1k2", true)]
        [TestCase("4d3t2k1", true)]
        [TestCase("4d3k1t2", true)]
        [TestCase("4d3!t2k1", true)]
        [TestCase("4d3!k1t2", true)]
        [TestCase("4d3t2!k1", true)]
        [TestCase("4d3k1!t2", true)]
        [TestCase("4d3t2k1!", true)]
        [TestCase("4d3k1t2!", true)]
        public void CanParse(string expression, bool canParse)
        {
            Assert.That(Roll.CanParse(expression), Is.EqualTo(canParse));
        }

        [TestCase(-1, false)]
        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(10, true)]
        [TestCase(100, true)]
        [TestCase(Limits.Quantity - 1, true)]
        [TestCase(Limits.Quantity, true)]
        [TestCase(Limits.Quantity + 1, false)]
        public void IsValid_Quantity(int quantity, bool isValid)
        {
            roll.Quantity = quantity;
            roll.Die = 2;
            roll.AmountToKeep = 0;

            Assert.That(roll.IsValid, Is.EqualTo(isValid));
        }

        [TestCase(-1, false)]
        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(6, true)]
        [TestCase(8, true)]
        [TestCase(10, true)]
        [TestCase(12, true)]
        [TestCase(20, true)]
        [TestCase(100, true)]
        [TestCase(Limits.Die - 1, true)]
        [TestCase(Limits.Die, true)]
        [TestCase(Limits.Die + 1, false)]
        public void IsValid_Die(int die, bool isValid)
        {
            roll.Quantity = 1;
            roll.Die = die;
            roll.AmountToKeep = 0;

            Assert.That(roll.IsValid, Is.EqualTo(isValid));
        }

        [TestCase(-2, false)]
        [TestCase(-1, false)]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(10, true)]
        [TestCase(100, true)]
        [TestCase(Limits.Quantity - 1, true)]
        [TestCase(Limits.Quantity, true)]
        [TestCase(Limits.Quantity + 1, false)]
        public void IsValid_Keep(int keep, bool isValid)
        {
            roll.Quantity = 1;
            roll.Die = 2;
            roll.AmountToKeep = keep;

            Assert.That(roll.IsValid, Is.EqualTo(isValid));
        }

        [TestCase(true)]
        [TestCase(true, -1)]
        [TestCase(true, 0)]
        [TestCase(true, 1)]
        [TestCase(true, 4)]
        [TestCase(true, 1, 4)]
        [TestCase(true, 0, 4)]
        [TestCase(true, 1, 0)]
        [TestCase(true, 2, 3, 4)]
        [TestCase(true, 1, 3, 4)]
        [TestCase(true, 1, 2, 4)]
        [TestCase(true, 1, 2, 3)]
        [TestCase(false, 1, 2, 3, 4)]
        [TestCase(true, 1, 2, 3, 3)]
        [TestCase(true, 2, 3, 4, 5)]
        [TestCase(false, 1, 2, 3, 4, 5)]
        [TestCase(false, 0, 1, 2, 3, 4)]
        [TestCase(true, 0, 1, 2, 4, 5)]
        public void IsValid_Explode(bool isValid, params int[] explodes)
        {
            roll.Quantity = 1;
            roll.Die = 4;
            roll.ExplodeOn.AddRange(explodes);

            Assert.That(roll.IsValid, Is.EqualTo(isValid));
        }

        [TestCase(true)]
        [TestCase(false, -1)]
        [TestCase(false, 0)]
        [TestCase(true, 1)]
        [TestCase(true, 4)]
        [TestCase(true, Limits.Die - 1)]
        [TestCase(true, Limits.Die)]
        [TestCase(false, Limits.Die + 1)]
        [TestCase(true, 1, 4)]
        [TestCase(false, 0, 4)]
        [TestCase(false, 1, 0)]
        [TestCase(true, 2, 3, 4)]
        [TestCase(true, 1, 3, 4)]
        [TestCase(true, 1, 2, 4)]
        [TestCase(true, 1, 2, 3)]
        [TestCase(true, 1, 2, 3, 4)]
        [TestCase(true, 1, 2, 3, 3)]
        [TestCase(true, 2, 3, 4, 5)]
        [TestCase(true, 1, 2, 3, 4, 5)]
        [TestCase(false, 0, 1, 2, 3, 4)]
        [TestCase(false, 0, 1, 2, 4, 5)]
        [TestCase(false, 1, 2, 4, 5, Limits.Die + 1)]
        public void IsValid_Transform(bool isValid, params int[] transforms)
        {
            roll.Quantity = 1;
            roll.Die = 4;

            foreach (var transform in transforms)
                roll.Transforms[transform] = 4;

            Assert.That(roll.IsValid, Is.EqualTo(isValid));
        }

        [TestCase(true)]
        [TestCase(false, -1)]
        [TestCase(false, 0)]
        [TestCase(true, 1)]
        [TestCase(true, 4)]
        [TestCase(true, Limits.Die - 1)]
        [TestCase(true, Limits.Die)]
        [TestCase(false, Limits.Die + 1)]
        [TestCase(true, 1, 4)]
        [TestCase(false, 0, 4)]
        [TestCase(false, 1, 0)]
        [TestCase(true, 2, 3, 4)]
        [TestCase(true, 1, 3, 4)]
        [TestCase(true, 1, 2, 4)]
        [TestCase(true, 1, 2, 3)]
        [TestCase(true, 1, 2, 3, 4)]
        [TestCase(true, 1, 2, 3, 3)]
        [TestCase(true, 2, 3, 4, 5)]
        [TestCase(true, 1, 2, 3, 4, 5)]
        [TestCase(false, 0, 1, 2, 3, 4)]
        [TestCase(false, 0, 1, 2, 4, 5)]
        [TestCase(false, 1, 2, 4, 5, Limits.Die + 1)]
        public void IsValid_Transform_NonMax(bool isValid, params int[] transforms)
        {
            roll.Quantity = 1;
            roll.Die = 4;

            foreach (var transform in transforms)
                roll.Transforms[transform] = 1;

            Assert.That(roll.IsValid, Is.EqualTo(isValid));
        }

        [TestCase(5, 4, 3, false, 1, true)]
        [TestCase(Limits.Quantity + 1, 4, 3, false, 1, false)]
        [TestCase(5, Limits.Die + 1, 3, false, 1, false)]
        [TestCase(5, 4, Limits.Quantity + 1, false, 1, false)]
        [TestCase(5, 4, 3, true, 1, false)]
        [TestCase(5, 4, 3, false, Limits.Die + 1, false)]
        [TestCase(Limits.Quantity + 1, Limits.Die + 1, Limits.Quantity + 1, true, Limits.Die + 1, false)]
        public void IsValid_AllProperties(int quantity, int die, int keep, bool explodeMaxed, int transform, bool isValid)
        {
            roll.Quantity = quantity;
            roll.Die = die;
            roll.AmountToKeep = keep;
            roll.Transforms[transform] = die;
            roll.ExplodeOn.AddRange(Enumerable.Range(1, die));

            if (!explodeMaxed)
                roll.ExplodeOn.Remove(1);

            Assert.That(roll.IsValid, Is.EqualTo(isValid));
        }

        [Test]
        public void IfGettingRollsAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetRolls(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingSumAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingAverageAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetPotentialAverage(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingTrueOrFalseAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetTrueOrFalse(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingMinimumAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetPotentialMinimum(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
        }

        [Test]
        public void IfGettingMaximumAndRollIsNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetPotentialMaximum(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342 is not a valid roll\n\tQuantity: 0 < 46341 < 10000\n\tDie: 0 < 46342 < 10000"));
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

            var message = $"{invalidQuantity}d9266 is not a valid roll";
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

            var message = $"9266d{invalidDie} is not a valid roll";
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

            var message = $"9266d42k{invalidKeep} is not a valid roll";
            message += $"\n\tKeep: 0 <= {invalidKeep} < {Limits.Quantity}";
            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(message));
        }

        [Test]
        public void IfExplodeNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = 9266;
            roll.Die = 1;
            roll.ExplodeOn.Add(1);

            var message = $"9266d1e1 is not a valid roll";
            message += $"\n\tExplode: Must have at least 1 non-exploded roll";
            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(message));
        }

        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(Limits.Die + 1)]
        public void IfTransformNotValid_ThrowInvalidOperationException(int transform)
        {
            roll.Quantity = 9266;
            roll.Die = 42;
            roll.Transforms[transform] = 42;

            var message = $"9266d42t{transform}:42 is not a valid roll";
            message += $"\n\tTransform: 0 < [{transform}:42] <= {Limits.Die}";
            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(message));
        }

        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(Limits.Die + 1)]
        public void IfAnyTransformNotValid_ThrowInvalidOperationException(int transform)
        {
            roll.Quantity = 9266;
            roll.Die = 42;
            roll.Transforms[21] = 42;
            roll.Transforms[transform] = 42;

            var message = $"9266d42t21:42t{transform}:42 is not a valid roll";
            message += $"\n\tTransform: 0 < [21:42,{transform}:42] <= {Limits.Die}";
            Assert.That(() => roll.GetSum(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(message));
        }

        [Test]
        public void IfAllNotValid_ThrowInvalidOperationException()
        {
            roll.Quantity = Limits.Quantity + 1;
            roll.Die = 0;
            roll.AmountToKeep = -1;
            roll.ExplodeOn.Add(0);
            roll.Transforms[Limits.Die + 1] = 0;

            var message = $"{Limits.Quantity + 1}d0e0t{Limits.Die + 1}:0k-1 is not a valid roll";
            message += $"\n\tQuantity: 0 < {Limits.Quantity + 1} < {Limits.Quantity}";
            message += $"\n\tDie: 0 < 0 < {Limits.Die}";
            message += $"\n\tKeep: 0 <= -1 < {Limits.Quantity}";
            message += $"\n\tExplode: Must have at least 1 non-exploded roll";
            message += $"\n\tTransform: 0 < [{Limits.Die + 1}:0] <= {Limits.Die}";
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
            roll.ExplodeOn.Add(66);

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
        public void GetRollsAndExplodeMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(6);
            roll.ExplodeOn.Add(42);

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 0; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 30 ? 2 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(95).And.EqualTo(countTotal));
        }

        [Test]
        public void GetRollsAndTransform()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 66;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 0; individualRoll--)
            {
                if (individualRoll == 42)
                {
                    Assert.That(rolls, Does.Not.Contain(individualRoll));
                    continue;
                }

                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 27 || individualRoll == 66 ? 2 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(92).And.EqualTo(countTotal));
        }

        [Test]
        public void GetRollsAndTransformMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 66;
            roll.Transforms[2] = 66;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 0; individualRoll--)
            {
                if (individualRoll == 4 || individualRoll == 2)
                {
                    Assert.That(rolls, Does.Not.Contain(individualRoll));
                    continue;
                }

                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 27 ? 2 :
                    individualRoll == 66 ? 5 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(92).And.EqualTo(countTotal));
        }

        [Test]
        public void GetRollsAndTransformCustom()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 21;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 0; individualRoll--)
            {
                if (individualRoll == 42)
                {
                    Assert.That(rolls, Does.Not.Contain(individualRoll));
                    continue;
                }

                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll == 21 ? 3 :
                    individualRoll < 27 ? 2 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
                countTotal += expectedCount;
            }

            Assert.That(rolls.Count(), Is.EqualTo(92).And.EqualTo(countTotal));
        }

        [Test]
        public void GetRollsWithAllOperations()
        {
            roll.Quantity = 92 * 2;
            roll.Die = 66;
            roll.ExplodeOn.Add(60);
            roll.ExplodeOn.Add(13);
            roll.AmountToKeep = 42;
            roll.Transforms[9] = 66;
            roll.Transforms[6] = 52;

            var rolls = roll.GetRolls(mockRandom.Object);
            var countTotal = 0;

            for (var individualRoll = 66; individualRoll > 51; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll == 52 ? 6 :
                    individualRoll < 58 ? 3 :
                    individualRoll == 66 ? 5 : 2;
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
            roll.ExplodeOn.Add(66);

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(2589));
        }

        [Test]
        public void GetSumOfRollsAndExplodeMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(6);
            roll.ExplodeOn.Add(42);

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(2646));
        }

        [Test]
        public void GetSumOfRollsAndTransform()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 66;

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(2586));
        }

        [Test]
        public void GetSumOfRollsAndTransformMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 66;
            roll.Transforms[2] = 66;

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(2814));
        }

        [Test]
        public void GetSumOfRollsAndTransformCustom()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 21;

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(2541));
        }

        [Test]
        public void GetSumOfRollsWithAllOperations()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(60);
            roll.ExplodeOn.Add(13);
            roll.AmountToKeep = 42;
            roll.Transforms[9] = 66;
            roll.Transforms[6] = 44;

            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(2025));
        }

        [Test]
        public void GetPotentialAverageRoll()
        {
            roll.Quantity = 92;
            roll.Die = 66;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndKeep()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.AmountToKeep = 42;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((42 + 42 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndExplode()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(66);

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndExplodeMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(6);
            roll.ExplodeOn.Add(42);

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndExplodeMinimum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(1);

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 * 2 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndExplodeMultipleMinimum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(1);
            roll.ExplodeOn.Add(2);

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 * 3 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransform()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 66;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransform_TransformedFromMinimum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[1] = 66;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 * 2 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransform_TransformedToMinimum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 1;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransform_TransformedToZero()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 0;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransform_TransformedFromMaximum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[66] = 42;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 65) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransform_TransformedToMaximum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 66;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransform_TransformedAboveMaximum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 600;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 600) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransformMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 66;
            roll.Transforms[2] = 66;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransformCustom()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 9;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransformMultipleCustom()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 9;
            roll.Transforms[2] = 21;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransformMultiple_TransformedFromMinimums()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[1] = 66;
            roll.Transforms[2] = 66;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 * 3 + 92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransformMultiple_TransformedToMinimums()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 1;
            roll.Transforms[9] = 0;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 * 66) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransformMultiple_TransformedFromMaximums()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[66] = 42;
            roll.Transforms[65] = 9;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 64) / 2));
        }

        [Test]
        public void GetPotentialAverageRollAndTransformMultiple_TransformedToMaximums()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 66;
            roll.Transforms[9] = 600;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((92 + 92 * 600) / 2));
        }

        [Test]
        public void GetPotentialAverageRollWithAllOperations()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(60);
            roll.ExplodeOn.Add(13);
            roll.AmountToKeep = 42;
            roll.Transforms[9] = 600;
            roll.Transforms[6] = 0;
            roll.Transforms[1] = 37;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo((42 * 600) / 2));
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
            roll.ExplodeOn.Add(66);

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndExplodeMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(6);
            roll.ExplodeOn.Add(42);

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndExplodeMinimum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(1);

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92 * 2));
        }

        [Test]
        public void GetPotentialMinimumRollAndExplodeMultipleMinimums()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(1);
            roll.ExplodeOn.Add(2);

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92 * 3));
        }

        [Test]
        public void GetPotentialMinimumRollAndExplodeMultiple_AllButMaxTransformed()
        {
            roll.Quantity = 2;
            roll.Die = 3;
            roll.ExplodeOn.Add(1);
            roll.ExplodeOn.Add(2);

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(6));
        }

        [Test]
        public void GetPotentialMinimumRollAndTransform()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 66;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndTransform_TransformedFromMinimum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[1] = 66;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92 * 2));
        }

        [Test]
        public void GetPotentialMinimumRollAndTransform_TransformedToMinimum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 1;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndTransform_TransformedToZero()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 0;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.Zero);
        }

        [Test]
        public void GetPotentialMinimumRollAndTransformMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 66;
            roll.Transforms[2] = 66;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndTransformCustom()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 90210;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndTransformMultipleCustom()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 90210;
            roll.Transforms[2] = 600;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndTransformMultiple_TransformedFromMinimums()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[1] = 66;
            roll.Transforms[2] = 42;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92 * 3));
        }

        [Test]
        public void GetPotentialMinimumRollAndTransformMultiple_TransformedToMinimums()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 1;
            roll.Transforms[9] = 0;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.Zero);
        }

        [Test]
        public void GetPotentialMinimumRollAndTransformMultiple_AllButMaxTransformed()
        {
            roll.Quantity = 2;
            roll.Die = 3;
            roll.Transforms[1] = 4;
            roll.Transforms[2] = 5;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(6));
        }

        [Test]
        public void GetPotentialMinimumRollAndTransformMultiple_AllTransformed()
        {
            roll.Quantity = 2;
            roll.Die = 3;
            roll.Transforms[1] = 4;
            roll.Transforms[2] = 5;
            roll.Transforms[3] = 6;

            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(8));
        }

        [Test]
        public void GetPotentialMinimumRollWithAllOperations()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(2);
            roll.ExplodeOn.Add(5);
            roll.AmountToKeep = 42;
            roll.Transforms[4] = 1;
            roll.Transforms[1] = 7;

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
            roll.ExplodeOn.Add(66);

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 66 * 10));
        }

        [Test]
        public void GetPotentialMaximumRollAndExplode_WithoutExplode()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(66);

            var maximum = roll.GetPotentialMaximum(false);
            Assert.That(maximum, Is.EqualTo(92 * 66));
        }

        [Test]
        public void GetPotentialMaximumRollAndExplodeMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(6);
            roll.ExplodeOn.Add(42);

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 66 * 10));
        }

        [Test]
        public void GetPotentialMaximumRollAndTransform()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 66;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 66));
        }

        [Test]
        public void GetPotentialMaximumRollAndTransformMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 66;
            roll.Transforms[2] = 66;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 66));
        }

        [Test]
        public void GetPotentialMaximumRollAndTransformCustom()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 60;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 66));
        }

        [Test]
        public void GetPotentialMaximumRollAndTransformCustom_FromMaximum()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[66] = 42;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 65));
        }

        [Test]
        public void GetPotentialMaximumRollAndTransformMultipleCustom()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 60;
            roll.Transforms[2] = 9;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 66));
        }

        [Test]
        public void GetPotentialMaximumRollAndTransformMultipleCustom_FromMaximums()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[66] = 60;
            roll.Transforms[65] = 42;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 64));
        }

        [Test]
        public void GetPotentialMaximumRollAndTransformCustom_HigherThanDie()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 600;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 600));
        }

        [Test]
        public void GetPotentialMaximumRollWithAllOperations()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(60);
            roll.ExplodeOn.Add(13);
            roll.AmountToKeep = 42;
            roll.Transforms[9] = 90;
            roll.Transforms[6] = 21;

            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(42 * 90 * 10));
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
            roll.ExplodeOn.Add(66);

            Assert.That(roll.ToString(), Is.EqualTo("92d66e66"));
        }

        [Test]
        public void RollStringWithExplodeMultiple()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(6);
            roll.ExplodeOn.Add(42);

            Assert.That(roll.ToString(), Is.EqualTo("92d66e6e42"));
        }

        [Test]
        public void RollStringWithTransform()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 66;

            Assert.That(roll.ToString(), Is.EqualTo("92d66t42:66"));
        }

        [Test]
        public void RollStringWithMultipleTransforms()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 66;
            roll.Transforms[2] = 66;

            Assert.That(roll.ToString(), Is.EqualTo("92d66t4:66t2:66"));
        }

        [Test]
        public void RollStringWithCustomTransform()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[42] = 600;

            Assert.That(roll.ToString(), Is.EqualTo("92d66t42:600"));
        }

        [Test]
        public void RollStringWithMultipleCustomTransforms()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.Transforms[4] = 60;
            roll.Transforms[2] = 0;

            Assert.That(roll.ToString(), Is.EqualTo("92d66t4:60t2:0"));
        }

        [Test]
        public void RollStringWithAllOperations()
        {
            roll.Quantity = 92;
            roll.Die = 66;
            roll.ExplodeOn.Add(60);
            roll.ExplodeOn.Add(13);
            roll.AmountToKeep = 42;
            roll.Transforms[9] = 66;
            roll.Transforms[6] = 36;

            Assert.That(roll.ToString(), Is.EqualTo("92d66e60e13t9:66t6:36k42"));
        }
    }
}
