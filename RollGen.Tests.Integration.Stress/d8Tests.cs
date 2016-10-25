﻿using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d8Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 8; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d8().AsSum();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public override void FullRangeHit(int quantity)
        {
            AssertFullRangeHit(quantity);
        }

        [Test]
        public override void RollWithLargestDieRollPossible()
        {
            Stress(AssertRollWithLargestDieRollPossible);
        }
    }
}