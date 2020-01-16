using NUnit.Framework;

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
        public void RankingForOnlyAdjustmentInRange()
        {
            collection.Adjustment = 9266;

            var ranking = collection.GetRanking(9266, 9266);
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
        public void RankingForOnlyAdjustmentOutOfRange(int lower, int upper)
        {
            collection.Adjustment = 9266;

            var ranking = collection.GetRanking(lower, upper);
            Assert.That(ranking, Is.EqualTo(int.MaxValue));
        }

        [TestCase(1, 100_000 + 1000)]
        [TestCase(2, 100_000 + 1000 * 2)]
        [TestCase(3, 100_000 + 1000 * 3)]
        public void RankingForOneRollInRange(int quantity, int expectedRanking)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRanking(quantity + 9266, quantity * 100 + 9266);
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
        public void RankingForOneRollOutOfRange(int lower, int upper, int quantity)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRanking(lower, upper);
            Assert.That(ranking, Is.EqualTo(int.MaxValue));
        }

        [TestCase(1, 1, 100_000 * 2 + 1_000 * 2)]
        [TestCase(2, 1, 100_000 * 2 + 1_000 * 3)]
        [TestCase(3, 1, 100_000 * 2 + 1_000 * 4)]
        [TestCase(1, 2, 100_000 * 2 + 1_000 * 3)]
        [TestCase(2, 2, 100_000 * 2 + 1_000 * 4)]
        [TestCase(3, 2, 100_000 * 2 + 1_000 * 5)]
        [TestCase(1, 3, 100_000 * 2 + 1_000 * 4)]
        [TestCase(2, 3, 100_000 * 2 + 1_000 * 5)]
        [TestCase(3, 3, 100_000 * 2 + 1_000 * 6)]
        public void RankingForMultipleRollsInRange(int quantity1, int quantity2, int expectedRanking)
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

            var ranking = collection.GetRanking(quantity1 + quantity2 + 9266, quantity1 * 100 + quantity2 * 20 + 9266);
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
        public void RankingForMultipleRollsOutOfRange(int lower, int upper, int quantity1, int quantity2)
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

            var ranking = collection.GetRanking(lower, upper);
            Assert.That(ranking, Is.EqualTo(int.MaxValue));
        }

        [TestCase(1, 8, 1, 2, -1, 100_000 * 2 + 1_000 * 2 + 92)]
        [TestCase(1, 6, 1, 4, -1, 100_000 * 2 + 1_000 * 2 + 94)]
        public void RankingForMultipleRollsInRangeWithDifferentDice(int q1, int d1, int q2, int d2, int adjustment, int expectedRanking)
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

            var ranking = collection.GetRanking(q1 + q2 + adjustment, q1 * d1 + q2 * d2 + adjustment);
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
        public void RankingForRollInRangeWithMaxDice(int die, int expectedRanking)
        {
            var prototype = new RollPrototype
            {
                Quantity = 1,
                Die = die
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRanking(1, die);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(1000, 10000, 1000, 10, 100_000_000 * 2 + 100_000 * 11 + 90)]
        public void ComputeSingleDieRanking(int lower, int upper, int quantity, int die, int rank)
        {
            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = die
            };

            collection.Rolls.Add(prototype);
            collection.Adjustment = lower - quantity;

            var ranking = collection.GetRanking(lower, upper);
            Assert.That(ranking, Is.EqualTo(rank));
        }

        [TestCase(11, 40, 1, 20, 2, 6, 100_000 * 2 + 1_000 * 3 + 80)]
        [TestCase(11, 40, 2, 12, 1, 8, 100_000 * 2 + 1_000 * 3 + 88)]
        public void ComputeMultiDieRanking(int lower, int upper, int q1, int d1, int q2, int d2, int rank)
        {
            var p1 = new RollPrototype
            {
                Quantity = q1,
                Die = d1
            };
            var p2 = new RollPrototype
            {
                Quantity = q2,
                Die = d2
            };

            collection.Rolls.Add(p1);
            collection.Rolls.Add(p2);
            collection.Adjustment = lower - collection.Quantities;

            var ranking = collection.GetRanking(lower, upper);
            Assert.That(ranking, Is.EqualTo(rank));
        }

        [TestCase(101, 2, 100_000_000 * 2 + 100_000 + 1000 * 101 + 98)]
        [TestCase(11, 2, 100_000_000 + 100_000 + 1000 * 11 + 98)]
        [TestCase(101, 3, 100_000_000 * 2 + 100_000 + 1000 * 101 + 97)]
        [TestCase(16, 3, 100_000_000 + 100_000 + 1000 * 16 + 97)]
        [TestCase(101, 4, 100_000_000 * 2 + 100_000 + 1000 * 101 + 96)]
        [TestCase(21, 4, 100_000_000 + 100_000 + 1000 * 21 + 96)]
        [TestCase(101, 6, 100_000_000 * 2 + 100_000 + 1000 * 101 + 94)]
        [TestCase(31, 6, 100_000_000 + 100_000 + 1000 * 31 + 94)]
        [TestCase(101, 8, 100_000_000 * 2 + 100_000 + 1000 * 101 + 92)]
        [TestCase(41, 8, 100_000_000 + 100_000 + 1000 * 41 + 92)]
        [TestCase(101, 10, 100_000_000 * 2 + 100_000 + 1000 * 101 + 90)]
        [TestCase(51, 10, 100_000_000 + 100_000 + 1000 * 51 + 90)]
        [TestCase(101, 12, 100_000_000 * 2 + 100_000 + 1000 * 101 + 88)]
        [TestCase(61, 12, 100_000_000 + 100_000 + 1000 * 61 + 88)]
        [TestCase(101, 20, 100_000_000 * 2 + 100_000 + 1000 * 101 + 80)]
        [TestCase(101, 100, 100_000_000 + 100_000 + 1000 * 101)]
        [TestCase(501, 100, 100_000_000 * 2 + 100_000 + 1000 * 501)]
        public void RankingForRollInRangeWithExcessivelyHighQuantity(int quantity, int die, int rank)
        {
            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = die
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRanking(quantity, quantity * die);
            Assert.That(ranking, Is.EqualTo(rank));
        }

        [Test]
        public void RankingForRollInRangeWithExcessivelyHighQuantity_MultipleRolls()
        {
            var prototype = new RollPrototype
            {
                Quantity = 101,
                Die = 100
            };

            var otherPrototype = new RollPrototype
            {
                Quantity = 50,
                Die = 2
            };

            collection.Rolls.Add(prototype);
            collection.Rolls.Add(otherPrototype);

            var ranking = collection.GetRanking(151, 10200);
            Assert.That(ranking, Is.EqualTo(100_000_000 * 2 + 100_000 * 2 + 1000 * 151));
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
    }
}
