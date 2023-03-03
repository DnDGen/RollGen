using NUnit.Framework;
using System;
using System.Diagnostics;

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

        [TestCase(1, 2, 1)]
        [TestCase(1, 3, 1)]
        [TestCase(1, 4, 1)]
        [TestCase(1, 6, 1)]
        [TestCase(1, 8, 1)]
        [TestCase(1, 10, 1)]
        [TestCase(1, 12, 1)]
        [TestCase(1, 20, 1)]
        [TestCase(1, 100, 1)]
        [TestCase(1, 42, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 3, 3)]
        [TestCase(2, 4, 4)]
        [TestCase(2, 6, 6)]
        [TestCase(2, 8, 8)]
        [TestCase(2, 10, 10)]
        [TestCase(2, 12, 12)]
        [TestCase(2, 20, 20)]
        [TestCase(2, 100, 100)]
        [TestCase(2, 42, 42)]
        [TestCase(3, 2, 3)] //37.5% * 2^3 = 3
        [TestCase(3, 3, 7)] //25.93% * 3^3 = 7
        [TestCase(3, 4, 12)] //18.75% * 4^3 = 12
        [TestCase(3, 6, 27)] //12.5% * 6^3 = 27
        [TestCase(3, 8, 48)] //9.38% * 8^3 = 48
        [TestCase(3, 10, 75)] //7.5% * 10^3 = 75
        [TestCase(3, 12, 108)] //6.25% * 12^3 = 108
        [TestCase(3, 20, 300)] //3.75% * 20^3 = 300
        [TestCase(3, 100, 7500)] //0.75% * 100^3 = 7500
        [TestCase(3, 42, 1323)] //1.79% * 42^3 = 1326 (rounding error of 3)
        [TestCase(10, 10, 432457640)] //4.32% * 10^10 = 432457640
        [TestCase(17, 2, 24310)] //18.55% * 2^17 = 24314 (rounding error of 4)
        public void ComputeDistribution(int q, int d, int D)
        {
            var prototype = new RollPrototype
            {
                Quantity = q,
                Die = d
            };

            collection.Rolls.Add(prototype);
            collection.Adjustment = 666;

            var distribution = collection.ComputeDistribution();
            Assert.That(distribution, Is.EqualTo(D));
        }

        [TestCase(1, 2, 1, 2, 2)]
        [TestCase(1, 2, 1, 3, 2)] //33.33% * 2*3 = 2
        [TestCase(1, 2, 1, 4, 2)] //25% * 2*4 = 2
        [TestCase(1, 2, 1, 6, 2)]
        [TestCase(1, 2, 1, 8, 2)]
        [TestCase(1, 2, 1, 10, 2)]
        [TestCase(1, 2, 1, 12, 2)]
        [TestCase(1, 2, 1, 20, 2)] //5% * 2*20 = 2
        [TestCase(1, 2, 1, 100, 2)] //1% * 2*100 = 2
        [TestCase(1, 2, 1, 42, 2)]
        [TestCase(1, 3, 1, 2, 2)] //33.33% * 3*2 = 2
        [TestCase(1, 3, 1, 3, 3)]
        [TestCase(1, 3, 1, 4, 3)] //25% * 3*4 = 3
        [TestCase(1, 3, 1, 6, 3)]
        [TestCase(1, 3, 1, 8, 3)]
        [TestCase(1, 3, 1, 10, 3)]
        [TestCase(1, 3, 1, 12, 3)]
        [TestCase(1, 3, 1, 20, 3)] //5% * 3*20 = 3
        [TestCase(1, 3, 1, 100, 3)] //1% * 3*100 = 3
        [TestCase(1, 3, 1, 42, 3)]
        [TestCase(1, 4, 1, 2, 2)]
        [TestCase(1, 4, 1, 3, 3)]
        [TestCase(1, 4, 1, 4, 4)]
        [TestCase(1, 4, 1, 6, 4)]
        [TestCase(1, 4, 1, 8, 4)]
        [TestCase(1, 4, 1, 10, 4)]
        [TestCase(1, 4, 1, 12, 4)]
        [TestCase(1, 4, 1, 20, 4)]
        [TestCase(1, 4, 1, 100, 4)]
        [TestCase(1, 4, 1, 42, 4)]
        [TestCase(1, 6, 1, 2, 2)]
        [TestCase(1, 6, 1, 3, 3)]
        [TestCase(1, 6, 1, 4, 4)]
        [TestCase(1, 6, 1, 6, 6)]
        [TestCase(1, 6, 1, 8, 6)]
        [TestCase(1, 6, 1, 10, 6)]
        [TestCase(1, 6, 1, 12, 6)]
        [TestCase(1, 6, 1, 20, 6)]
        [TestCase(1, 6, 1, 100, 6)]
        [TestCase(1, 6, 1, 42, 6)]
        [TestCase(1, 8, 1, 2, 2)]
        [TestCase(1, 8, 1, 3, 3)]
        [TestCase(1, 8, 1, 4, 4)]
        [TestCase(1, 8, 1, 6, 6)]
        [TestCase(1, 8, 1, 8, 8)]
        [TestCase(1, 8, 1, 10, 8)]
        [TestCase(1, 8, 1, 12, 8)]
        [TestCase(1, 8, 1, 20, 8)]
        [TestCase(1, 8, 1, 100, 8)]
        [TestCase(1, 8, 1, 42, 8)]
        [TestCase(1, 10, 1, 2, 2)]
        [TestCase(1, 10, 1, 3, 3)]
        [TestCase(1, 10, 1, 4, 4)]
        [TestCase(1, 10, 1, 6, 6)]
        [TestCase(1, 10, 1, 8, 8)]
        [TestCase(1, 10, 1, 10, 10)]
        [TestCase(1, 10, 1, 12, 10)]
        [TestCase(1, 10, 1, 20, 10)]
        [TestCase(1, 10, 1, 100, 10)]
        [TestCase(1, 10, 1, 42, 10)]
        [TestCase(1, 12, 1, 2, 2)]
        [TestCase(1, 12, 1, 3, 3)]
        [TestCase(1, 12, 1, 4, 4)]
        [TestCase(1, 12, 1, 6, 6)]
        [TestCase(1, 12, 1, 8, 8)]
        [TestCase(1, 12, 1, 10, 10)]
        [TestCase(1, 12, 1, 12, 12)]
        [TestCase(1, 12, 1, 20, 12)]
        [TestCase(1, 12, 1, 100, 12)]
        [TestCase(1, 12, 1, 42, 12)]
        [TestCase(1, 20, 1, 2, 2)]
        [TestCase(1, 20, 1, 3, 3)]
        [TestCase(1, 20, 1, 4, 4)]
        [TestCase(1, 20, 1, 6, 6)]
        [TestCase(1, 20, 1, 8, 8)]
        [TestCase(1, 20, 1, 10, 10)]
        [TestCase(1, 20, 1, 12, 12)]
        [TestCase(1, 20, 1, 20, 20)]
        [TestCase(1, 20, 1, 100, 20)]
        [TestCase(1, 20, 1, 42, 20)]
        [TestCase(1, 100, 1, 2, 2)]
        [TestCase(1, 100, 1, 3, 3)]
        [TestCase(1, 100, 1, 4, 4)]
        [TestCase(1, 100, 1, 6, 6)]
        [TestCase(1, 100, 1, 8, 8)]
        [TestCase(1, 100, 1, 10, 10)]
        [TestCase(1, 100, 1, 12, 12)]
        [TestCase(1, 100, 1, 20, 20)]
        [TestCase(1, 100, 1, 100, 100)]
        [TestCase(1, 100, 1, 42, 42)]
        [TestCase(1, 42, 1, 2, 2)]
        [TestCase(1, 42, 1, 3, 3)]
        [TestCase(1, 42, 1, 4, 4)]
        [TestCase(1, 42, 1, 6, 6)]
        [TestCase(1, 42, 1, 8, 8)]
        [TestCase(1, 42, 1, 10, 10)]
        [TestCase(1, 42, 1, 12, 12)]
        [TestCase(1, 42, 1, 20, 20)]
        [TestCase(1, 42, 1, 100, 42)]
        [TestCase(1, 42, 1, 42, 42)]
        [TestCase(1, 2, 2, 2, 3)]
        [TestCase(1, 2, 2, 3, 5)] //27.78% * 2*3^2 = 5
        [TestCase(1, 2, 2, 4, 7)] //21.88% * 2*4^2 = 7
        [TestCase(1, 2, 2, 6, 11)] //15.28% * 2*6^2 = 11. Tentative: maxdie*2-1
        [TestCase(1, 2, 2, 8, 15)]
        [TestCase(1, 2, 2, 10, 19)]
        [TestCase(1, 2, 2, 12, 23)]
        [TestCase(1, 2, 2, 20, 39)]
        [TestCase(1, 2, 2, 100, 199)]
        [TestCase(1, 2, 2, 42, 83)] //2.35% * 2*42^2 = 83
        [TestCase(1, 3, 2, 2, 4)] //33.33% * 3*2^2 = 4
        [TestCase(1, 3, 2, 3, 7)]
        [TestCase(1, 3, 2, 4, 10)] //20.83% * 3*4^2 = 10
        [TestCase(1, 3, 2, 6, 16)] //14.81% * 3*6^2 = 16
        [TestCase(1, 3, 2, 8, 22)] //11.46% * 3*8^2 = 22. Tentative: maxdie*3-2
        [TestCase(1, 3, 2, 10, 28)]
        [TestCase(1, 3, 2, 12, 34)]
        [TestCase(1, 3, 2, 20, 58)]
        [TestCase(1, 3, 2, 100, 298)]
        [TestCase(1, 3, 2, 42, 124)] //2.34% * 3*42^2 = 124
        [TestCase(1, 4, 2, 2, 4)] //25% * 4*2^2 = 4. 2*4-4
        [TestCase(1, 4, 2, 3, 8)] //22.22% * 4*3^2 = 8. 3*4-4
        [TestCase(1, 4, 2, 4, 12)]
        [TestCase(1, 4, 2, 6, 20)] //13.89% * 4*6^2 = 20
        [TestCase(1, 4, 2, 8, 28)] //10.94% * 4*8^2 = 28. Tentative: maxdie*4-4
        [TestCase(1, 4, 2, 10, 36)]
        [TestCase(1, 4, 2, 12, 44)]
        [TestCase(1, 4, 2, 20, 76)]
        [TestCase(1, 4, 2, 100, 396)]
        [TestCase(1, 4, 2, 42, 164)] //2.32% * 4*42^2 = 164
        [TestCase(1, 6, 2, 2, 4)] //16.67% * 6*2^2 = 4. 2*6-8
        [TestCase(1, 6, 2, 3, 9)] //16.67% * 6*3^2 = 9. 3*6-9
        [TestCase(1, 6, 2, 4, 15)] //15.63% * 6*4^2 = 15. 4*6-9
        [TestCase(1, 6, 2, 6, 27)]
        [TestCase(1, 6, 2, 8, 39)] //10.16% * 6*8^2 = 39. 8*6-9
        [TestCase(1, 6, 2, 10, 51)]
        [TestCase(1, 6, 2, 12, 63)]
        [TestCase(1, 6, 2, 20, 111)]
        [TestCase(1, 6, 2, 100, 591)]
        [TestCase(1, 6, 2, 42, 243)] //2.3% * 6*42^2 = 243
        [TestCase(1, 8, 2, 2, 4)]
        [TestCase(1, 8, 2, 3, 9)]
        [TestCase(1, 8, 2, 4, 16)]
        [TestCase(1, 8, 2, 6, 36)]
        [TestCase(1, 8, 2, 8, 48)]
        [TestCase(1, 8, 2, 10, 64)]
        [TestCase(1, 8, 2, 12, 80)]
        [TestCase(1, 8, 2, 20, 144)]
        [TestCase(1, 8, 2, 100, 784)]
        [TestCase(1, 8, 2, 42, 8)]
        [TestCase(1, 10, 2, 2, 4)]
        [TestCase(1, 10, 2, 3, 9)]
        [TestCase(1, 10, 2, 4, 16)]
        [TestCase(1, 10, 2, 6, 35)]
        [TestCase(1, 10, 2, 8, 55)]
        [TestCase(1, 10, 2, 10, 75)]
        [TestCase(1, 10, 2, 12, 95)]
        [TestCase(1, 10, 2, 20, 175)]
        [TestCase(1, 10, 2, 100, 975)]
        [TestCase(1, 10, 2, 42, 395)]
        [TestCase(1, 12, 2, 2, 4)]
        [TestCase(1, 12, 2, 3, 9)]
        [TestCase(1, 12, 2, 4, 16)]
        [TestCase(1, 12, 2, 6, 36)]
        [TestCase(1, 12, 2, 8, 60)]
        [TestCase(1, 12, 2, 10, 84)]
        [TestCase(1, 12, 2, 12, 108)]
        [TestCase(1, 12, 2, 20, 204)]
        [TestCase(1, 12, 2, 100, 1164)]
        [TestCase(1, 12, 2, 42, 468)]
        [TestCase(1, 20, 2, 2, 4)]
        [TestCase(1, 20, 2, 3, 9)]
        [TestCase(1, 20, 2, 4, 16)]
        [TestCase(1, 20, 2, 6, 36)]
        [TestCase(1, 20, 2, 8, 64)]
        [TestCase(1, 20, 2, 10, 100)]
        [TestCase(1, 20, 2, 12, 140)]
        [TestCase(1, 20, 2, 20, 300)]
        [TestCase(1, 20, 2, 100, 1900)]
        [TestCase(1, 20, 2, 42, 740)]
        [TestCase(1, 100, 2, 2, 4)]
        [TestCase(1, 100, 2, 3, 9)]
        [TestCase(1, 100, 2, 4, 16)]
        [TestCase(1, 100, 2, 6, 36)]
        [TestCase(1, 100, 2, 8, 64)]
        [TestCase(1, 100, 2, 10, 100)]
        [TestCase(1, 100, 2, 12, 144)]
        [TestCase(1, 100, 2, 20, 400)]
        [TestCase(1, 100, 2, 100, 7500)]
        [TestCase(1, 100, 2, 42, 1764)]
        [TestCase(1, 42, 2, 2, 4)]
        [TestCase(1, 42, 2, 3, 9)]
        [TestCase(1, 42, 2, 4, 16)]
        [TestCase(1, 42, 2, 6, 36)]
        [TestCase(1, 42, 2, 8, 64)]
        [TestCase(1, 42, 2, 10, 100)]
        [TestCase(1, 42, 2, 12, 144)]
        [TestCase(1, 42, 2, 20, 400)]
        [TestCase(1, 42, 2, 100, 3759)]
        [TestCase(1, 42, 2, 42, 1323)]
        [TestCase(2, 2, 1, 2, 3)]
        [TestCase(2, 2, 2, 2, 3)]
        [TestCase(2, 2, 2, 3, 5)]
        [TestCase(2, 2, 2, 4, 7)]
        [TestCase(2, 2, 2, 6, 11)]
        [TestCase(2, 2, 2, 8, 15)]
        [TestCase(2, 2, 2, 10, 19)]
        [TestCase(2, 2, 2, 12, 23)]
        [TestCase(2, 2, 2, 20, 39)]
        [TestCase(2, 2, 2, 100, 199)]
        [TestCase(2, 2, 2, 42, 83)]
        [TestCase(2, 3, 2, 2, 4)]
        [TestCase(2, 3, 2, 3, 7)]
        [TestCase(2, 3, 2, 4, 10)]
        [TestCase(2, 3, 2, 6, 16)]
        [TestCase(2, 3, 2, 8, 22)]
        [TestCase(2, 3, 2, 10, 28)]
        [TestCase(2, 3, 2, 12, 24)]
        [TestCase(2, 3, 2, 20, 58)]
        [TestCase(2, 3, 2, 100, 298)]
        [TestCase(2, 3, 2, 42, 124)]
        [TestCase(2, 4, 2, 2, 4)]
        [TestCase(2, 4, 2, 3, 8)]
        [TestCase(2, 4, 2, 4, 12)]
        [TestCase(2, 4, 2, 6, 20)]
        [TestCase(2, 4, 2, 8, 28)]
        [TestCase(2, 4, 2, 10, 36)]
        [TestCase(2, 4, 2, 12, 44)]
        [TestCase(2, 4, 2, 20, 76)]
        [TestCase(2, 4, 2, 100, 396)]
        [TestCase(2, 4, 2, 42, 164)]
        [TestCase(2, 6, 2, 2, 4)]
        [TestCase(2, 6, 2, 3, 9)]
        [TestCase(2, 6, 2, 4, 15)]
        [TestCase(2, 6, 2, 6, 27)]
        [TestCase(2, 6, 2, 8, 218)] //9.46% * 6^2*8^2 = 218
        [TestCase(2, 6, 2, 10, 51)]
        [TestCase(2, 6, 2, 12, 63)]
        [TestCase(2, 6, 2, 20, 111)]
        [TestCase(2, 6, 2, 100, 591)]
        [TestCase(2, 6, 2, 42, 243)]
        [TestCase(2, 8, 2, 2, 2)]
        [TestCase(2, 8, 2, 3, 3)]
        [TestCase(2, 8, 2, 4, 4)]
        [TestCase(2, 8, 2, 6, 6)]
        [TestCase(2, 8, 2, 8, 8)]
        [TestCase(2, 8, 2, 10, 8)]
        [TestCase(2, 8, 2, 12, 8)]
        [TestCase(2, 8, 2, 20, 8)]
        [TestCase(2, 8, 2, 100, 8)]
        [TestCase(2, 8, 2, 42, 8)]
        [TestCase(2, 10, 2, 2, 2)]
        [TestCase(2, 10, 2, 3, 3)]
        [TestCase(2, 10, 2, 4, 4)]
        [TestCase(2, 10, 2, 6, 6)]
        [TestCase(2, 10, 2, 8, 8)]
        [TestCase(2, 10, 2, 10, 10)]
        [TestCase(2, 10, 2, 12, 10)]
        [TestCase(2, 10, 2, 20, 10)]
        [TestCase(2, 10, 2, 100, 10)]
        [TestCase(2, 10, 2, 42, 10)]
        [TestCase(2, 12, 2, 2, 2)]
        [TestCase(2, 12, 2, 3, 3)]
        [TestCase(2, 12, 2, 4, 4)]
        [TestCase(2, 12, 2, 6, 6)]
        [TestCase(2, 12, 2, 8, 8)]
        [TestCase(2, 12, 2, 10, 10)]
        [TestCase(2, 12, 2, 12, 12)]
        [TestCase(2, 12, 2, 20, 12)]
        [TestCase(2, 12, 2, 100, 12)]
        [TestCase(2, 12, 2, 42, 12)]
        [TestCase(2, 20, 2, 2, 2)]
        [TestCase(2, 20, 2, 3, 3)]
        [TestCase(2, 20, 2, 4, 4)]
        [TestCase(2, 20, 2, 6, 6)]
        [TestCase(2, 20, 2, 8, 8)]
        [TestCase(2, 20, 2, 10, 10)]
        [TestCase(2, 20, 2, 12, 12)]
        [TestCase(2, 20, 2, 20, 20)]
        [TestCase(2, 20, 2, 100, 20)]
        [TestCase(2, 20, 2, 42, 20)]
        [TestCase(2, 100, 2, 2, 2)]
        [TestCase(2, 100, 2, 3, 3)]
        [TestCase(2, 100, 2, 4, 4)]
        [TestCase(2, 100, 2, 6, 6)]
        [TestCase(2, 100, 2, 8, 8)]
        [TestCase(2, 100, 2, 10, 10)]
        [TestCase(2, 100, 2, 12, 12)]
        [TestCase(2, 100, 2, 20, 20)]
        [TestCase(2, 100, 2, 100, 100)]
        [TestCase(2, 100, 2, 42, 42)]
        [TestCase(2, 42, 2, 2, 2)]
        [TestCase(2, 42, 2, 3, 3)]
        [TestCase(2, 42, 2, 4, 4)]
        [TestCase(2, 42, 2, 6, 6)]
        [TestCase(2, 42, 2, 8, 8)]
        [TestCase(2, 42, 2, 10, 10)]
        [TestCase(2, 42, 2, 12, 12)]
        [TestCase(2, 42, 2, 20, 20)]
        [TestCase(2, 42, 2, 100, 42)]
        [TestCase(2, 42, 2, 42, 42)]
        [TestCase(2, 10, 8, 10, 432457640)]
        public void ComputeDistribution_TwoRolls(int q1, int d1, int q2, int d2, int D)
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

            var distribution = collection.ComputeDistribution();
            Assert.That(distribution, Is.EqualTo(D));
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

        [TestCase(1, 2, 1, 3, 1, 4, 6)] //25% * 2*3*4 = 6
        [TestCase(1, 3, 1, 4, 1, 6, 12)] //16.67% * 3*4*6 = 12
        [TestCase(1, 4, 1, 6, 1, 2, 8)] //16.67% * 4*6*2 = 8
        [TestCase(1, 2, 1, 3, 2, 4, 19)] //19.79% * 2*3*4^2 = 19
        [TestCase(1, 3, 1, 4, 2, 6, 58)] //13.43% * 3*4*6^2 = 58
        [TestCase(1, 4, 1, 6, 2, 2, 16)] //16.67% * 4*6*2^2 = 16
        [TestCase(1, 2, 2, 3, 1, 4, 16)] //22.22% * 2*3^2*4 = 16
        [TestCase(1, 3, 2, 4, 1, 6, 43)] //14.93% * 3*4^2*6 = 43
        [TestCase(1, 4, 2, 6, 1, 2, 40)] //13.89% * 4*6^2*2 = 40
        [TestCase(1, 2, 2, 3, 2, 4, 53)] //18.4% * 2*3^2*4^2 = 53
        [TestCase(1, 3, 2, 4, 2, 6, 220)] //12.73% * 3*4^2*6^2 = 220
        [TestCase(1, 4, 2, 6, 2, 2, 78)] //13.54% * 4*6^2*2^2 = 78
        [TestCase(2, 2, 2, 3, 1, 4, 30)] //20.83% * 2^2*3^2*4 = 30
        [TestCase(2, 3, 2, 4, 1, 6, 124)] //14.35% * 3^2*4^2*6 = 124
        [TestCase(2, 4, 2, 6, 1, 2, 148)] //12.85% * 4^2*6^2*2 = 148
        [TestCase(2, 2, 2, 3, 2, 4, 106)] //18.4% * 2^2*3^2*4^2 = 106
        [TestCase(2, 3, 2, 4, 2, 6, 640)] //12.35% * 3^2*4^2*6^2 = 640
        [TestCase(2, 4, 2, 6, 2, 2, 296)] //12.85% * 4^2*6^2*2^2 = 296
        [TestCase(1, 10, 1, 10, 8, 10, 432457640)]
        public void ComputeDistribution_ThreeRolls(int q1, int d1, int q2, int d2, int q3, int d3, int D)
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

            var distribution = collection.ComputeDistribution();
            Assert.That(distribution, Is.EqualTo(D));
        }

        [TestCase(3, 2, 3)]
        [TestCase(3, 6, 27)]
        [TestCase(3, 10, 75)]
        [TestCase(3, 20, 300)] //3.75% * 20^3 = 300
        [TestCase(3, 100, 7500)] //0.75% * 100^3 = 7500
        [TestCase(3, Limits.Die, 75000000)] //0.75% * 10,000^3 = 7500000000
        [TestCase(4, 2, 6)]
        [TestCase(4, 6, 146)]
        [TestCase(4, 10, 670)]
        [TestCase(4, 20, 5340)]
        [TestCase(4, 100, 666700)]
        [TestCase(4, Limits.Die, 946739120)]
        [TestCase(5, 2, 10)]
        [TestCase(5, 6, 780)]
        [TestCase(5, 10, 6000)]
        [TestCase(5, 20, 95875)]
        [TestCase(5, 100, 59896875)]
        [TestCase(5, Limits.Die, int.MaxValue)]
        [TestCase(10, 2, 252)]
        [TestCase(10, 6, 4395456)] //7.27% * 6^10 = 4395891, rounding error
        [TestCase(10, 10, 432457640)]
        [TestCase(10, 20, 1590283184)]
        [TestCase(10, 100, int.MaxValue)]
        [TestCase(10, Limits.Die, int.MaxValue)]
        [TestCase(20, 2, 184756)]
        [TestCase(20, 6, 1673505640)]
        [TestCase(20, 10, int.MaxValue)]
        [TestCase(20, 20, int.MaxValue)]
        [TestCase(20, 100, int.MaxValue)]
        [TestCase(20, Limits.Die, int.MaxValue)]
        [TestCase(100, 2, int.MaxValue)]
        [TestCase(100, 6, int.MaxValue)]
        [TestCase(100, 10, int.MaxValue)]
        [TestCase(100, 20, int.MaxValue)]
        [TestCase(100, 100, int.MaxValue)]
        [TestCase(100, Limits.Die, int.MaxValue)]
        [TestCase(Limits.Quantity, 2, int.MaxValue)]
        [TestCase(Limits.Quantity, 3, int.MaxValue)]
        [TestCase(Limits.Quantity, 4, int.MaxValue)]
        [TestCase(Limits.Quantity, 6, int.MaxValue)]
        [TestCase(Limits.Quantity, 8, int.MaxValue)]
        [TestCase(Limits.Quantity, 10, int.MaxValue)]
        [TestCase(Limits.Quantity, 12, int.MaxValue)]
        [TestCase(Limits.Quantity, 20, int.MaxValue)]
        [TestCase(Limits.Quantity, 100, int.MaxValue)]
        [TestCase(Limits.Quantity, Limits.Die, int.MaxValue)]
        public void ComputeDistribution_IsFast(int q1, int d1, int D)
        {
            var prototype1 = new RollPrototype
            {
                Quantity = q1,
                Die = d1
            };

            collection.Rolls.Add(prototype1);
            collection.Adjustment = 666;

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var distribution = collection.ComputeDistribution();
            stopwatch.Stop();

            Assert.That(distribution, Is.EqualTo(D));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(2, 20, 2, 12, 2310)] //4.01% * 20^2*12^2 = 2310
        [TestCase(2, 100, 2, 20, 37200)] //0.93% * 100^2*20^2 = 37200
        [TestCase(2, 100, 3, 20, 736000)] //0.92% * 100^2*20^3 = 736000
        [TestCase(3, 10, 4, 12, 947635)] //4.57% * 10^3*12^4 = 947635
        [TestCase(3, 100, 2, 20, 2960000)] //0.74% * 100^3*20^2 = 2960000
        [TestCase(3, 100, 3, 20, 59200000)] //0.74% * 100^3*20^3 = 59200000
        [TestCase(20, 100, 20, 20, int.MaxValue)]
        [TestCase(100, 100, 100, 20, int.MaxValue)]
        [TestCase(Limits.Quantity, 100, Limits.Quantity, 20, int.MaxValue)]
        public void ComputeDistribution_TwoRolls_IsFast(int q1, int d1, int q2, int d2, int D)
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

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var distribution = collection.ComputeDistribution();
            stopwatch.Stop();

            Assert.That(distribution, Is.EqualTo(D));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }
    }
}
