﻿using Moq;
using NUnit.Framework;
using RollGen.Domain.PartialRolls;
using System;

namespace RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class d12Tests
    {
        private Mock<Random> mockRandom;
        private PartialRoll partialRoll;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
        }

        [Test]
        public void ReturnRollValue()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(12)).Returns(42);

            var roll = partialRoll.d12();
            Assert.That(roll, Is.EqualTo(43));
        }

        [Test]
        public void RollQuantity()
        {
            partialRoll = new RandomPartialRoll(2, mockRandom.Object);
            mockRandom.SetupSequence(r => r.Next(12)).Returns(4).Returns(2);

            var roll = partialRoll.d12();
            Assert.That(roll, Is.EqualTo(8));
        }

        [Test]
        public void AfterRoll_AlwaysReturnZero()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(12)).Returns(42);

            partialRoll.d12();
            var roll = partialRoll.d12();
            Assert.That(roll, Is.EqualTo(0));
        }

        [Test]
        public void AfterOtherRoll_AlwaysReturnZero()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(12)).Returns(42);

            partialRoll.d(21);
            var roll = partialRoll.d12();
            Assert.That(roll, Is.EqualTo(0));
        }
    }
}