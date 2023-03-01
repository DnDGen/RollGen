using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DnDGen.RollGen.Tests.Unit
{
    [TestFixture]
    public class RollCollectionTests
    {
        private RollCollection collection;

        [SetUp]
        public void Setup()
        {
            collection = new RollCollection();
        }

        [Test]
        public void RollCollectionInitialized()
        {
            Assert.That(collection.Adjustment, Is.Zero);
            Assert.That(collection.Rolls, Is.Empty);
        }

        [TestCase(-90210)]
        [TestCase(0)]
        [TestCase(9266)]
        public void BuildAdjustment(int adjustment)
        {
            collection.Adjustment = adjustment;

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo(adjustment.ToString()));
        }

        [Test]
        public void BuildRoll()
        {
            var prototype = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };
            collection.Rolls.Add(prototype);

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo("9266d100"));
        }

        [TestCase(-600, "9266d100-600")]
        [TestCase(0, "9266d100")]
        [TestCase(42, "9266d100+42")]
        public void BuildRollWithAdjustment(int adjustment, string expectedroll)
        {
            var prototype = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            collection.Adjustment = adjustment;

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo(expectedroll));
        }

        [Test]
        public void BuildMultipleRolls()
        {
            var prototype = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = 42,
                Die = 20
            };

            collection.Rolls.Add(otherPrototype);

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo("9266d100+42d20"));
        }

        [Test]
        public void CollectionDescription()
        {
            var prototype = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = 42,
                Die = 20
            };

            collection.Rolls.Add(otherPrototype);

            collection.Adjustment = 1337;

            Assert.That(collection.ToString(), Is.EqualTo("9266d100+42d20+1337"));
        }

        [TestCase(-1337, "9266d100+42d20-1337")]
        [TestCase(0, "9266d100+42d20")]
        [TestCase(1336, "9266d100+42d20+1336")]
        public void BuildMultipleRollsWithAdjustment(int adjustment, string expectedRoll)
        {
            var prototype = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = 42,
                Die = 20
            };

            collection.Rolls.Add(otherPrototype);

            collection.Adjustment = adjustment;

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo(expectedRoll));
        }

        [Test]
        public void BuildOrdersDicefromLargestToSmallest()
        {
            var prototype = new RollPrototype
            {
                Quantity = 9266,
                Die = 20
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = 42,
                Die = 12
            };

            collection.Rolls.Add(otherPrototype);

            var anotherPrototype = new RollPrototype
            {
                Quantity = 1337,
                Die = 100
            };

            collection.Rolls.Add(anotherPrototype);

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo("1337d100+9266d20+42d12"));
        }

        [Test]
        public void Ranking_FewestDice_ForOnlyAdjustmentInRange()
        {
            collection.Adjustment = 9266;

            var ranking = collection.GetRankingForFewestDice(9266, 9266);
            Assert.That(ranking, Is.Zero);
        }

        [TestCase(9264, 9264)]
        [TestCase(9264, 9265)]
        [TestCase(9264, 9266)]
        [TestCase(9264, 9267)]
        [TestCase(9264, 9268)]
        [TestCase(9265, 9265)]
        [TestCase(9265, 9266)]
        [TestCase(9265, 9267)]
        [TestCase(9265, 9268)]
        [TestCase(9266, 9267)]
        [TestCase(9266, 9268)]
        [TestCase(9267, 9267)]
        [TestCase(9267, 9268)]
        [TestCase(9268, 9268)]
        public void Ranking_FewestDice_ForOnlyAdjustmentOutOfRange(int lower, int upper)
        {
            collection.Adjustment = 9266;

            var ranking = collection.GetRankingForFewestDice(lower, upper);
            Assert.That(ranking, Is.EqualTo(int.MaxValue));
        }

        [TestCase(1, 100_000_000 + 1000)]
        [TestCase(2, 100_000_000 + 1000 * 2)]
        [TestCase(3, 100_000_000 + 1000 * 3)]
        public void Ranking_FewestDice_ForOneRollInRange(int quantity, int expectedRanking)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRankingForFewestDice(quantity + 9266, quantity * 100 + 9266);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(9264 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9264 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9264 + 1, 100 * 1 + 9266, 1)]
        [TestCase(9264 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9264 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9264 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9264 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9264 + 2, 100 * 2 + 9266, 2)]
        [TestCase(9264 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9264 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9264 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9264 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9264 + 3, 100 * 3 + 9266, 3)]
        [TestCase(9264 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9264 + 3, 100 * 3 + 9268, 3)]
        [TestCase(9265 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9265 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9265 + 1, 100 * 1 + 9266, 1)]
        [TestCase(9265 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9265 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9265 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9265 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9265 + 2, 100 * 2 + 9266, 2)]
        [TestCase(9265 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9265 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9265 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9265 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9265 + 3, 100 * 3 + 9266, 3)]
        [TestCase(9265 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9265 + 3, 100 * 3 + 9268, 3)]
        [TestCase(9266 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9266 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9266 + 1, 100 * 1 + 9266, 1, Ignore = "Is actually in range")]
        [TestCase(9266 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9266 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9266 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9266 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9266 + 2, 100 * 2 + 9266, 2, Ignore = "Is actually in range")]
        [TestCase(9266 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9266 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9266 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9266 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9266 + 3, 100 * 3 + 9266, 3, Ignore = "Is actually in range")]
        [TestCase(9266 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9266 + 3, 100 * 3 + 9268, 3)]
        [TestCase(9267 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9267 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9267 + 1, 100 * 1 + 9266, 1)]
        [TestCase(9267 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9267 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9267 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9267 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9267 + 2, 100 * 2 + 9266, 2)]
        [TestCase(9267 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9267 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9267 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9267 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9267 + 3, 100 * 3 + 9266, 3)]
        [TestCase(9267 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9267 + 3, 100 * 3 + 9268, 3)]
        [TestCase(9268 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9268 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9268 + 1, 100 * 1 + 9266, 1)]
        [TestCase(9268 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9268 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9268 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9268 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9268 + 2, 100 * 2 + 9266, 2)]
        [TestCase(9268 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9268 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9268 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9268 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9268 + 3, 100 * 3 + 9266, 3)]
        [TestCase(9268 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9268 + 3, 100 * 3 + 9268, 3)]
        public void Ranking_FewestDice_ForOneRollOutOfRange(int lower, int upper, int quantity)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRankingForFewestDice(lower, upper);
            Assert.That(ranking, Is.EqualTo(int.MaxValue));
        }

        [TestCase(1, 1, 100_000_000 * 2 + 1_000 * 2)]
        [TestCase(2, 1, 100_000_000 * 2 + 1_000 * 3)]
        [TestCase(3, 1, 100_000_000 * 2 + 1_000 * 4)]
        [TestCase(1, 2, 100_000_000 * 2 + 1_000 * 3)]
        [TestCase(2, 2, 100_000_000 * 2 + 1_000 * 4)]
        [TestCase(3, 2, 100_000_000 * 2 + 1_000 * 5)]
        [TestCase(1, 3, 100_000_000 * 2 + 1_000 * 4)]
        [TestCase(2, 3, 100_000_000 * 2 + 1_000 * 5)]
        [TestCase(3, 3, 100_000_000 * 2 + 1_000 * 6)]
        public void Ranking_FewestDice_ForMultipleRollsInRange(int quantity1, int quantity2, int expectedRanking)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity1,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = quantity2,
                Die = 20
            };

            collection.Rolls.Add(otherPrototype);

            var ranking = collection.GetRankingForFewestDice(quantity1 + quantity2 + 9266, quantity1 * 100 + quantity2 * 20 + 9266);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9266, 1, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9266, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9266, 1, 1, Ignore = "Actually in range")]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9266, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9266, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9266, 2, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9266, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9266, 2, 1, Ignore = "Actually in range")]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9266, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9266, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9266, 3, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9266, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9266, 3, 1, Ignore = "Actually in range")]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9266, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9266, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9266, 1, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9266, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9266, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9266, 1, 2, Ignore = "Actually in range")]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9266, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9266, 2, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9266, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9266, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9266, 2, 2, Ignore = "Actually in range")]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9266, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9266, 3, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9266, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9266, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9266, 3, 2, Ignore = "Actually in range")]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9266, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9266, 1, 3)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9266, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9266, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9266, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9266, 1, 3, Ignore = "Actually in range")]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9266, 2, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9266, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9266, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9266, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9266, 2, 3, Ignore = "Actually in range")]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9266, 3, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9266, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9266, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9266, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9266, 3, 3, Ignore = "Actually in range")]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        public void Ranking_FewestDice_ForMultipleRollsOutOfRange(int lower, int upper, int quantity1, int quantity2)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity1,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = quantity2,
                Die = 20
            };

            collection.Rolls.Add(otherPrototype);

            var ranking = collection.GetRankingForFewestDice(lower, upper);
            Assert.That(ranking, Is.EqualTo(int.MaxValue));
        }

        [TestCase(1, 8, 1, 2, -1, 100_000_000 * 2 + 1_000 * 2 + 92)]
        [TestCase(1, 6, 1, 4, -1, 100_000_000 * 2 + 1_000 * 2 + 94)]
        public void Ranking_FewestDice_ForMultipleRollsInRangeWithDifferentDice(int q1, int d1, int q2, int d2, int adjustment, int expectedRanking)
        {
            collection.Adjustment = adjustment;

            var prototype = new RollPrototype
            {
                Quantity = q1,
                Die = d1
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = q2,
                Die = d2
            };

            collection.Rolls.Add(otherPrototype);

            var ranking = collection.GetRankingForFewestDice(q1 + q2 + adjustment, q1 * d1 + q2 * d2 + adjustment);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(2, 100_000_000 + 1000 + 98)]
        [TestCase(3, 100_000_000 + 1000 + 97)]
        [TestCase(4, 100_000_000 + 1000 + 96)]
        [TestCase(6, 100_000_000 + 1000 + 94)]
        [TestCase(8, 100_000_000 + 1000 + 92)]
        [TestCase(10, 100_000_000 + 1000 + 90)]
        [TestCase(12, 100_000_000 + 1000 + 88)]
        [TestCase(20, 100_000_000 + 1000 + 80)]
        [TestCase(100, 100_000_000 + 1000)]
        public void Ranking_FewestDice_ForRollInRangeWithMaxDice(int die, int expectedRanking)
        {
            var prototype = new RollPrototype
            {
                Quantity = 1,
                Die = die
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRankingForFewestDice(1, die);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [Test]
        public void Ranking_MostEvenDistribution_ForOnlyAdjustmentInRange()
        {
            collection.Adjustment = 9266;

            var ranking = collection.GetRankingForMostEvenDistribution(9266, 9266);
            Assert.That(ranking, Is.Zero);
        }

        [TestCase(9264, 9264)]
        [TestCase(9264, 9265)]
        [TestCase(9264, 9266)]
        [TestCase(9264, 9267)]
        [TestCase(9264, 9268)]
        [TestCase(9265, 9265)]
        [TestCase(9265, 9266)]
        [TestCase(9265, 9267)]
        [TestCase(9265, 9268)]
        [TestCase(9266, 9267)]
        [TestCase(9266, 9268)]
        [TestCase(9267, 9267)]
        [TestCase(9267, 9268)]
        [TestCase(9268, 9268)]
        public void Ranking_MostEvenDistribution_ForOnlyAdjustmentOutOfRange(int lower, int upper)
        {
            collection.Adjustment = 9266;

            var ranking = collection.GetRankingForMostEvenDistribution(lower, upper);
            Assert.That(ranking, Is.EqualTo(int.MaxValue));
        }

        [TestCase(1, 100_000 + 1000)]
        [TestCase(2, 100_000 * 2 + 1000)]
        [TestCase(3, 100_000 * 3 + 1000)]
        public void Ranking_MostEvenDistribution_ForOneRollInRange(int quantity, int expectedRanking)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRankingForMostEvenDistribution(quantity + 9266, quantity * 100 + 9266);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(9264 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9264 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9264 + 1, 100 * 1 + 9266, 1)]
        [TestCase(9264 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9264 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9264 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9264 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9264 + 2, 100 * 2 + 9266, 2)]
        [TestCase(9264 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9264 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9264 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9264 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9264 + 3, 100 * 3 + 9266, 3)]
        [TestCase(9264 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9264 + 3, 100 * 3 + 9268, 3)]
        [TestCase(9265 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9265 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9265 + 1, 100 * 1 + 9266, 1)]
        [TestCase(9265 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9265 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9265 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9265 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9265 + 2, 100 * 2 + 9266, 2)]
        [TestCase(9265 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9265 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9265 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9265 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9265 + 3, 100 * 3 + 9266, 3)]
        [TestCase(9265 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9265 + 3, 100 * 3 + 9268, 3)]
        [TestCase(9266 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9266 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9266 + 1, 100 * 1 + 9266, 1, Ignore = "Is actually in range")]
        [TestCase(9266 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9266 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9266 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9266 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9266 + 2, 100 * 2 + 9266, 2, Ignore = "Is actually in range")]
        [TestCase(9266 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9266 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9266 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9266 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9266 + 3, 100 * 3 + 9266, 3, Ignore = "Is actually in range")]
        [TestCase(9266 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9266 + 3, 100 * 3 + 9268, 3)]
        [TestCase(9267 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9267 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9267 + 1, 100 * 1 + 9266, 1)]
        [TestCase(9267 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9267 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9267 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9267 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9267 + 2, 100 * 2 + 9266, 2)]
        [TestCase(9267 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9267 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9267 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9267 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9267 + 3, 100 * 3 + 9266, 3)]
        [TestCase(9267 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9267 + 3, 100 * 3 + 9268, 3)]
        [TestCase(9268 + 1, 100 * 1 + 9264, 1)]
        [TestCase(9268 + 1, 100 * 1 + 9265, 1)]
        [TestCase(9268 + 1, 100 * 1 + 9266, 1)]
        [TestCase(9268 + 1, 100 * 1 + 9267, 1)]
        [TestCase(9268 + 1, 100 * 1 + 9268, 1)]
        [TestCase(9268 + 2, 100 * 2 + 9264, 2)]
        [TestCase(9268 + 2, 100 * 2 + 9265, 2)]
        [TestCase(9268 + 2, 100 * 2 + 9266, 2)]
        [TestCase(9268 + 2, 100 * 2 + 9267, 2)]
        [TestCase(9268 + 2, 100 * 2 + 9268, 2)]
        [TestCase(9268 + 3, 100 * 3 + 9264, 3)]
        [TestCase(9268 + 3, 100 * 3 + 9265, 3)]
        [TestCase(9268 + 3, 100 * 3 + 9266, 3)]
        [TestCase(9268 + 3, 100 * 3 + 9267, 3)]
        [TestCase(9268 + 3, 100 * 3 + 9268, 3)]
        public void Ranking_MostEvenDistribution_ForOneRollOutOfRange(int lower, int upper, int quantity)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRankingForMostEvenDistribution(lower, upper);
            Assert.That(ranking, Is.EqualTo(int.MaxValue));
        }

        [TestCase(1, 1, 100_000 * 2 + 1_000 * 2)]
        [TestCase(1, 2, 100_000 * 3 + 1_000 * 2)]
        [TestCase(1, 3, 100_000 * 4 + 1_000 * 2)]
        [TestCase(2, 1, 100_000 * 3 + 1_000 * 2)]
        [TestCase(2, 2, 100_000 * 4 + 1_000 * 2)]
        [TestCase(2, 3, 100_000 * 5 + 1_000 * 2)]
        [TestCase(3, 1, 100_000 * 4 + 1_000 * 2)]
        [TestCase(3, 2, 100_000 * 5 + 1_000 * 2)]
        [TestCase(3, 3, 100_000 * 6 + 1_000 * 2)]
        public void Ranking_MostEvenDistribution_ForMultipleRollsInRange(int quantity1, int quantity2, int expectedRanking)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity1,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = quantity2,
                Die = 20
            };

            collection.Rolls.Add(otherPrototype);

            var ranking = collection.GetRankingForMostEvenDistribution(quantity1 + quantity2 + 9266, quantity1 * 100 + quantity2 * 20 + 9266);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9266, 1, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9266, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9266, 1, 1, Ignore = "Actually in range")]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9266, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9264, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9265, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9266, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9267, 1, 1)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9268, 1, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9266, 2, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9266, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9266, 2, 1, Ignore = "Actually in range")]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9266, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9264, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9265, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9266, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9267, 2, 1)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9268, 2, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9266, 3, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9266, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9266, 3, 1, Ignore = "Actually in range")]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9266, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9264, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9265, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9266, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9267, 3, 1)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9268, 3, 1)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9266, 1, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9266, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9266, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9266, 1, 2, Ignore = "Actually in range")]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9264, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9265, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9266, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9267, 1, 2)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9268, 1, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9266, 2, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9266, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9266, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9266, 2, 2, Ignore = "Actually in range")]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9264, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9265, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9266, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9267, 2, 2)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9268, 2, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9266, 3, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9266, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9266, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9266, 3, 2, Ignore = "Actually in range")]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9264, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9265, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9266, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9267, 3, 2)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9268, 3, 2)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9266, 1, 3)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9266, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9266, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9266, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9264, 1, 3)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9265, 1, 3)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9266, 1, 3, Ignore = "Actually in range")]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9267, 1, 3)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9268, 1, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9266, 2, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9266, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9266, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9266, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9264, 2, 3)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9265, 2, 3)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9266, 2, 3, Ignore = "Actually in range")]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9267, 2, 3)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9268, 2, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9266, 3, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9266, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9266, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9266, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9264, 3, 3)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9265, 3, 3)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9266, 3, 3, Ignore = "Actually in range")]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9267, 3, 3)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9268, 3, 3)]
        public void Ranking_MostEvenDistribution_ForMultipleRollsOutOfRange(int lower, int upper, int quantity1, int quantity2)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity1,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = quantity2,
                Die = 20
            };

            collection.Rolls.Add(otherPrototype);

            var ranking = collection.GetRankingForMostEvenDistribution(lower, upper);
            Assert.That(ranking, Is.EqualTo(int.MaxValue));
        }

        [TestCase(1, 8, 1, 2, -1, 100_000 * 2 + 1_000 * 2 + 92)]
        [TestCase(1, 6, 1, 4, -1, 100_000 * 2 + 1_000 * 2 + 94)]
        public void Ranking_MostEvenDistribution_ForMultipleRollsInRangeWithDifferentDice(int q1, int d1, int q2, int d2, int adjustment, int expectedRanking)
        {
            collection.Adjustment = adjustment;

            var prototype = new RollPrototype
            {
                Quantity = q1,
                Die = d1
            };

            collection.Rolls.Add(prototype);

            var otherPrototype = new RollPrototype
            {
                Quantity = q2,
                Die = d2
            };

            collection.Rolls.Add(otherPrototype);

            var ranking = collection.GetRankingForMostEvenDistribution(q1 + q2 + adjustment, q1 * d1 + q2 * d2 + adjustment);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(2, 100_000 + 1000 + 98)]
        [TestCase(3, 100_000 + 1000 + 97)]
        [TestCase(4, 100_000 + 1000 + 96)]
        [TestCase(6, 100_000 + 1000 + 94)]
        [TestCase(8, 100_000 + 1000 + 92)]
        [TestCase(10, 100_000 + 1000 + 90)]
        [TestCase(12, 100_000 + 1000 + 88)]
        [TestCase(20, 100_000 + 1000 + 80)]
        [TestCase(100, 100_000 + 1000)]
        public void Ranking_MostEvenDistribution_ForRollInRangeWithMaxDice(int die, int expectedRanking)
        {
            var prototype = new RollPrototype
            {
                Quantity = 1,
                Die = die
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRankingForMostEvenDistribution(1, die);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [Test]
        public void RollCollectionsEqual()
        {
            var roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(roll);
            collection.Adjustment = 42;

            roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            var otherCollection = new RollCollection
            {
                Adjustment = 42
            };
            otherCollection.Rolls.Add(roll);

            Assert.That(otherCollection, Is.EqualTo(collection));
        }

        [Test]
        public void RollCollectionsNotEqualByAdjustment()
        {
            var roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(roll);
            collection.Adjustment = 42;

            roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            var otherCollection = new RollCollection
            {
                Adjustment = 600
            };
            otherCollection.Rolls.Add(roll);

            Assert.That(otherCollection, Is.Not.EqualTo(collection));
        }

        [Test]
        public void RollCollectionsNotEqualBySingleRoll()
        {
            var roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(roll);
            collection.Adjustment = 42;

            roll = new RollPrototype
            {
                Quantity = 600,
                Die = 100
            };

            var otherCollection = new RollCollection
            {
                Adjustment = 42
            };
            otherCollection.Rolls.Add(roll);

            Assert.That(otherCollection, Is.Not.EqualTo(collection));

            otherCollection.Rolls[0] = new RollPrototype
            {
                Quantity = 9266,
                Die = 20
            };

            Assert.That(otherCollection, Is.Not.EqualTo(collection));
        }

        [Test]
        public void RollCollectionsNotEqualByMultipleRolls()
        {
            var roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(roll);

            roll = new RollPrototype
            {
                Quantity = 1337,
                Die = 1336
            };
            collection.Rolls.Add(roll);

            collection.Adjustment = 42;

            var otherCollection = new RollCollection
            {
                Adjustment = 42
            };

            roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };
            otherCollection.Rolls.Add(roll);

            roll = new RollPrototype
            {
                Quantity = 1337,
                Die = 20
            };
            otherCollection.Rolls.Add(roll);

            Assert.That(otherCollection, Is.Not.EqualTo(collection));

            otherCollection.Rolls[1] = new RollPrototype
            {
                Quantity = 600,
                Die = 100
            };

            Assert.That(otherCollection, Is.Not.EqualTo(collection));
        }

        [Test]
        public void RollCollectionsNotEqualByDifferentRolls()
        {
            var roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(roll);
            collection.Adjustment = 42;

            var otherCollection = new RollCollection
            {
                Adjustment = 42
            };

            Assert.That(otherCollection, Is.Not.EqualTo(collection));

            roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            otherCollection.Rolls.Add(roll);

            roll = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            collection.Rolls.Add(roll);

            Assert.That(otherCollection, Is.Not.EqualTo(collection));
        }

        [Test]
        public void Matches_0Rolls()
        {
            collection.Adjustment = 9266;

            var matches = collection.Matches(9266, 9266);
            Assert.That(matches, Is.True);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(1, 3)]
        [TestCase(1, 4)]
        [TestCase(1, 6)]
        [TestCase(1, 8)]
        [TestCase(1, 10)]
        [TestCase(1, 12)]
        [TestCase(1, 20)]
        [TestCase(1, 100)]
        [TestCase(1, 9266)]
        [TestCase(1, Limits.Die - 1)]
        [TestCase(1, Limits.Die)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(2, 4)]
        [TestCase(2, 6)]
        [TestCase(2, 8)]
        [TestCase(2, 10)]
        [TestCase(2, 12)]
        [TestCase(2, 20)]
        [TestCase(2, 100)]
        [TestCase(2, 9266)]
        [TestCase(2, Limits.Die - 1)]
        [TestCase(2, Limits.Die)]
        [TestCase(9266, 1)]
        [TestCase(9266, 2)]
        [TestCase(9266, 3)]
        [TestCase(9266, 4)]
        [TestCase(9266, 6)]
        [TestCase(9266, 8)]
        [TestCase(9266, 10)]
        [TestCase(9266, 12)]
        [TestCase(9266, 20)]
        [TestCase(9266, 100)]
        [TestCase(9266, 9266)]
        [TestCase(9266, Limits.Die - 1)]
        [TestCase(9266, Limits.Die)]
        [TestCase(Limits.Quantity - 1, 1)]
        [TestCase(Limits.Quantity - 1, 2)]
        [TestCase(Limits.Quantity - 1, 3)]
        [TestCase(Limits.Quantity - 1, 4)]
        [TestCase(Limits.Quantity - 1, 6)]
        [TestCase(Limits.Quantity - 1, 8)]
        [TestCase(Limits.Quantity - 1, 10)]
        [TestCase(Limits.Quantity - 1, 12)]
        [TestCase(Limits.Quantity - 1, 20)]
        [TestCase(Limits.Quantity - 1, 100)]
        [TestCase(Limits.Quantity - 1, 9266)]
        [TestCase(Limits.Quantity - 1, Limits.Die - 1)]
        [TestCase(Limits.Quantity - 1, Limits.Die)]
        [TestCase(Limits.Quantity, 1)]
        [TestCase(Limits.Quantity, 2)]
        [TestCase(Limits.Quantity, 3)]
        [TestCase(Limits.Quantity, 4)]
        [TestCase(Limits.Quantity, 6)]
        [TestCase(Limits.Quantity, 8)]
        [TestCase(Limits.Quantity, 10)]
        [TestCase(Limits.Quantity, 12)]
        [TestCase(Limits.Quantity, 20)]
        [TestCase(Limits.Quantity, 100)]
        [TestCase(Limits.Quantity, 9266)]
        [TestCase(Limits.Quantity, Limits.Die - 1)]
        [TestCase(Limits.Quantity, Limits.Die)]
        public void Matches_1Roll(int quantity, int die)
        {
            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = die
            };

            collection.Rolls.Add(prototype);

            var matches = collection.Matches(quantity, quantity * die);
            Assert.That(matches, Is.True);
        }

        [Test]
        public void DoesNotMatch_WhenLowerWrong()
        {
            var prototype = new RollPrototype
            {
                Quantity = 99,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var matches = collection.Matches(100, 10_000);
            Assert.That(matches, Is.False);
        }

        [Test]
        public void DoesNotMatch_WhenUpperWrong()
        {
            var prototype = new RollPrototype
            {
                Quantity = 99,
                Die = 100
            };

            collection.Rolls.Add(prototype);
            collection.Adjustment = 1;

            var matches = collection.Matches(100, 10_000);
            Assert.That(matches, Is.False);
        }

        [Test]
        public void DoesNotMatch_WhenQuantityTooHigh()
        {
            collection.Rolls.Add(new RollPrototype
            {
                Quantity = 10,
                Die = 10
            });
            collection.Rolls.Add(new RollPrototype
            {
                Quantity = Limits.Quantity + 1,
                Die = 2
            });

            var matches = collection.Matches(Limits.Quantity + 11, 100 + Limits.Quantity * 2 + 2);
            Assert.That(matches, Is.False);
        }

        [Test]
        public void DoesNotMatch_WhenDieTooHigh()
        {
            collection.Rolls.Add(new RollPrototype
            {
                Quantity = 10,
                Die = 10
            });
            collection.Rolls.Add(new RollPrototype
            {
                Quantity = 2,
                Die = Limits.Die + 1,
            });

            var matches = collection.Matches(12, 100 + Limits.Die * 2 + 2);
            Assert.That(matches, Is.False);
        }

        [TestCase(1, 2)]
        [TestCase(1, 3)]
        [TestCase(1, 4)]
        [TestCase(1, 6)]
        [TestCase(1, 8)]
        [TestCase(1, 10)]
        [TestCase(1, 12)]
        [TestCase(1, 20)]
        [TestCase(1, 100)]
        [TestCase(1, 42)]
        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(2, 4)]
        [TestCase(2, 6)]
        [TestCase(2, 8)]
        [TestCase(2, 10)]
        [TestCase(2, 12)]
        [TestCase(2, 20)]
        [TestCase(2, 100)]
        [TestCase(2, 42)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(3, 6)]
        [TestCase(3, 8)]
        [TestCase(3, 10)]
        [TestCase(3, 12)]
        [TestCase(3, 20)]
        [TestCase(3, 100)]
        [TestCase(3, 42)]
        public void ComputeDistribution(int q, int d)
        {
            var prototype = new RollPrototype
            {
                Quantity = q,
                Die = d
            };

            collection.Rolls.Add(prototype);
            collection.Adjustment = 666;

            var counts = new Dictionary<int, int>();
            var range = Enumerable.Range(q, q * d - q + 1);

            foreach (var total in range)
            {
                counts[total] = 0;
            }

            foreach (var total in GetRolls(q, d))
            {
                counts[total]++;
            }

            var maxCount = counts.Max(kvp => kvp.Value);

            var distribution = collection.ComputeDistribution();
            Assert.That(distribution, Is.EqualTo(maxCount), $"Counts: {JsonConvert.SerializeObject(counts)}");
        }

        private IEnumerable<int> GetRolls(int q, int d)
        {
            var range = Enumerable.Range(1, d);

            if (q == 1)
            {
                foreach (var roll in range)
                    yield return roll;
            }
            else
            {
                foreach (var subroll in GetRolls(q - 1, d))
                {
                    foreach (var roll in range)
                        yield return subroll + roll;
                }
            }
        }

        [TestCase(1, 2, 1, 2)]
        [TestCase(1, 2, 1, 3)]
        [TestCase(1, 2, 1, 4)]
        [TestCase(1, 2, 1, 6)]
        [TestCase(1, 3, 1, 2)]
        [TestCase(1, 3, 1, 3)]
        [TestCase(1, 3, 1, 4)]
        [TestCase(1, 3, 1, 6)]
        [TestCase(1, 4, 1, 2)]
        [TestCase(1, 4, 1, 3)]
        [TestCase(1, 4, 1, 4)]
        [TestCase(1, 4, 1, 6)]
        [TestCase(1, 6, 1, 2)]
        [TestCase(1, 6, 1, 3)]
        [TestCase(1, 6, 1, 4)]
        [TestCase(1, 6, 1, 6)]
        [TestCase(2, 2, 1, 2)]
        [TestCase(2, 2, 1, 3)]
        [TestCase(2, 2, 1, 4)]
        [TestCase(2, 2, 1, 6)]
        [TestCase(2, 3, 1, 2)]
        [TestCase(2, 3, 1, 3)]
        [TestCase(2, 3, 1, 4)]
        [TestCase(2, 3, 1, 6)]
        [TestCase(2, 4, 1, 2)]
        [TestCase(2, 4, 1, 3)]
        [TestCase(2, 4, 1, 4)]
        [TestCase(2, 4, 1, 6)]
        [TestCase(2, 6, 1, 2)]
        [TestCase(2, 6, 1, 3)]
        [TestCase(2, 6, 1, 4)]
        [TestCase(2, 6, 1, 6)]
        [TestCase(1, 2, 2, 2)]
        [TestCase(1, 2, 2, 3)]
        [TestCase(1, 2, 2, 4)]
        [TestCase(1, 2, 2, 6)]
        [TestCase(1, 3, 2, 2)]
        [TestCase(1, 3, 2, 3)]
        [TestCase(1, 3, 2, 4)]
        [TestCase(1, 3, 2, 6)]
        [TestCase(1, 4, 2, 2)]
        [TestCase(1, 4, 2, 3)]
        [TestCase(1, 4, 2, 4)]
        [TestCase(1, 4, 2, 6)]
        [TestCase(1, 6, 2, 2)]
        [TestCase(1, 6, 2, 3)]
        [TestCase(1, 6, 2, 4)]
        [TestCase(1, 6, 2, 6)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(2, 2, 2, 3)]
        [TestCase(2, 2, 2, 4)]
        [TestCase(2, 2, 2, 6)]
        [TestCase(2, 3, 2, 2)]
        [TestCase(2, 3, 2, 3)]
        [TestCase(2, 3, 2, 4)]
        [TestCase(2, 3, 2, 6)]
        [TestCase(2, 4, 2, 2)]
        [TestCase(2, 4, 2, 3)]
        [TestCase(2, 4, 2, 4)]
        [TestCase(2, 4, 2, 6)]
        [TestCase(2, 6, 2, 2)]
        [TestCase(2, 6, 2, 3)]
        [TestCase(2, 6, 2, 4)]
        [TestCase(2, 6, 2, 6)]
        public void ComputeDistribution_TwoRolls(int q1, int d1, int q2, int d2)
        {
            var prototype1 = new RollPrototype
            {
                Quantity = q1,
                Die = d1
            };
            var prototype2 = new RollPrototype
            {
                Quantity = q2,
                Die = d2
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);
            collection.Adjustment = 666;

            var counts = new Dictionary<int, int>();

            foreach (var total1 in GetRolls(q1, d1))
            {
                foreach (var total2 in GetRolls(q2, d2))
                {
                    if (!counts.ContainsKey(total1 + total2))
                        counts[total1 + total2] = 0;

                    counts[total1 + total2]++;
                }
            }

            var maxCount = counts.Max(kvp => kvp.Value);

            var distribution = collection.ComputeDistribution();
            Assert.That(distribution, Is.EqualTo(maxCount), $"Expected Counts: {JsonConvert.SerializeObject(counts)}");
        }

        [TestCase(2, 2)]
        [TestCase(2, 3)]
        [TestCase(2, 4)]
        [TestCase(2, 6)]
        [TestCase(2, 8)]
        [TestCase(2, 10)]
        [TestCase(2, 12)]
        [TestCase(2, 20)]
        [TestCase(2, 100)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(3, 6)]
        [TestCase(3, 8)]
        [TestCase(3, 10)]
        [TestCase(3, 12)]
        [TestCase(3, 20)]
        [TestCase(3, 100)]
        public void ComputeDistribution_TransitiveByMultiplication(int q, int d)
        {
            var prototype = new RollPrototype
            {
                Quantity = q,
                Die = d
            };

            collection.Rolls.Add(prototype);
            collection.Adjustment = 666;

            var expected = collection.ComputeDistribution();

            collection.Rolls.Clear();
            while (q-- > 0)
            {
                prototype = new RollPrototype
                {
                    Quantity = 1,
                    Die = d
                };

                collection.Rolls.Add(prototype);
            }

            var actual = collection.ComputeDistribution();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(2, 3)]
        [TestCase(2, 4)]
        [TestCase(2, 6)]
        [TestCase(2, 8)]
        [TestCase(2, 10)]
        [TestCase(2, 12)]
        [TestCase(2, 20)]
        [TestCase(2, 100)]
        [TestCase(3, 2)]
        [TestCase(3, 4)]
        [TestCase(3, 6)]
        [TestCase(3, 8)]
        [TestCase(3, 10)]
        [TestCase(3, 12)]
        [TestCase(3, 20)]
        [TestCase(3, 100)]
        [TestCase(4, 2)]
        [TestCase(4, 3)]
        [TestCase(4, 6)]
        [TestCase(4, 8)]
        [TestCase(4, 10)]
        [TestCase(4, 12)]
        [TestCase(4, 20)]
        [TestCase(4, 100)]
        [TestCase(6, 2)]
        [TestCase(6, 3)]
        [TestCase(6, 4)]
        [TestCase(6, 8)]
        [TestCase(6, 10)]
        [TestCase(6, 12)]
        [TestCase(6, 20)]
        [TestCase(6, 100)]
        public void ComputeDistribution_TransitiveByAddition(int d1, int d2)
        {
            var prototype1 = new RollPrototype
            {
                Quantity = 1,
                Die = d1
            };
            var prototype2 = new RollPrototype
            {
                Quantity = 1,
                Die = d2
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);
            collection.Adjustment = 666;

            var expected = collection.ComputeDistribution();

            collection.Rolls.Clear();
            prototype1 = new RollPrototype
            {
                Quantity = 1,
                Die = d2
            };
            prototype2 = new RollPrototype
            {
                Quantity = 1,
                Die = d1
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);

            var actual = collection.ComputeDistribution();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(1, 2, 1, 3, 1, 4)]
        [TestCase(1, 3, 1, 4, 1, 6)]
        [TestCase(1, 4, 1, 6, 1, 2)]
        [TestCase(1, 2, 1, 3, 2, 4)]
        [TestCase(1, 3, 1, 4, 2, 6)]
        [TestCase(1, 4, 1, 6, 2, 2)]
        [TestCase(1, 2, 2, 3, 1, 4)]
        [TestCase(1, 3, 2, 4, 1, 6)]
        [TestCase(1, 4, 2, 6, 1, 2)]
        [TestCase(1, 2, 2, 3, 2, 4)]
        [TestCase(1, 3, 2, 4, 2, 6)]
        [TestCase(1, 4, 2, 6, 2, 2)]
        [TestCase(2, 2, 2, 3, 1, 4)]
        [TestCase(2, 3, 2, 4, 1, 6)]
        [TestCase(2, 4, 2, 6, 1, 2)]
        [TestCase(2, 2, 2, 3, 2, 4)]
        [TestCase(2, 3, 2, 4, 2, 6)]
        [TestCase(2, 4, 2, 6, 2, 2)]
        public void ComputeDistribution_ThreeRolls(int q1, int d1, int q2, int d2, int q3, int d3)
        {
            var prototype1 = new RollPrototype
            {
                Quantity = q1,
                Die = d1
            };
            var prototype2 = new RollPrototype
            {
                Quantity = q2,
                Die = d2
            };
            var prototype3 = new RollPrototype
            {
                Quantity = q3,
                Die = d3
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);
            collection.Rolls.Add(prototype3);
            collection.Adjustment = 666;

            var counts = new Dictionary<int, int>();

            foreach (var total1 in GetRolls(q1, d1))
            {
                foreach (var total2 in GetRolls(q2, d2))
                {
                    foreach (var total3 in GetRolls(q3, d3))
                    {
                        if (!counts.ContainsKey(total1 + total2 + total3))
                            counts[total1 + total2 + total3] = 0;

                        counts[total1 + total2 + total3]++;
                    }
                }
            }

            var maxCount = counts.Max(kvp => kvp.Value);

            var distribution = collection.ComputeDistribution();
            Assert.That(distribution, Is.EqualTo(maxCount), $"Expected Counts: {JsonConvert.SerializeObject(counts)}");
        }

        [TestCase(2, 20)]
        [TestCase(2, 100)]
        [TestCase(3, 20)]
        [TestCase(3, 100)]
        [TestCase(20, 20)]
        [TestCase(100, 100)]
        [TestCase(Limits.Quantity, 20)]
        [TestCase(Limits.Quantity, 100)]
        [TestCase(Limits.Quantity, Limits.Die)]
        public void ComputeDistribution_IsFast(int q1, int d1)
        {
            var prototype1 = new RollPrototype
            {
                Quantity = q1,
                Die = d1
            };

            collection.Rolls.Add(prototype1);
            collection.Adjustment = 666;

            var counts = new Dictionary<int, int>();

            foreach (var total1 in GetRolls(q1, d1))
            {
                if (!counts.ContainsKey(total1))
                    counts[total1] = 0;

                counts[total1]++;
            }

            var maxCount = counts.Max(kvp => kvp.Value);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var distribution = collection.ComputeDistribution();
            stopwatch.Stop();

            Assert.That(distribution, Is.EqualTo(maxCount), $"Expected Counts: {JsonConvert.SerializeObject(counts)}");
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(2, 20, 2, 12)]
        [TestCase(2, 100, 2, 20)]
        [TestCase(2, 100, 3, 20)]
        [TestCase(3, 10, 4, 12)]
        [TestCase(3, 100, 2, 20)]
        [TestCase(3, 100, 3, 20)]
        [TestCase(20, 100, 20, 20)]
        [TestCase(100, 100, 100, 20)]
        [TestCase(Limits.Quantity, 100, Limits.Quantity, 20)]
        public void ComputeDistribution_TwoRolls_IsFast(int q1, int d1, int q2, int d2)
        {
            var prototype1 = new RollPrototype
            {
                Quantity = q1,
                Die = d1
            };
            var prototype2 = new RollPrototype
            {
                Quantity = q2,
                Die = d2
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);
            collection.Adjustment = 666;

            var counts = new Dictionary<int, int>();

            foreach (var total1 in GetRolls(q1, d1))
            {
                foreach (var total2 in GetRolls(q2, d2))
                {
                    if (!counts.ContainsKey(total1 + total2))
                        counts[total1 + total2] = 0;

                    counts[total1 + total2]++;
                }
            }

            var maxCount = counts.Max(kvp => kvp.Value);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var distribution = collection.ComputeDistribution();
            stopwatch.Stop();

            Assert.That(distribution, Is.EqualTo(maxCount), $"Expected Counts: {JsonConvert.SerializeObject(counts)}");
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }
    }
}
