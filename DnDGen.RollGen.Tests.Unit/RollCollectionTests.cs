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

        [TestCase(9264, 9264, 400000)]
        [TestCase(9264, 9265, 300000)]
        [TestCase(9264, 9266, 200000)]
        [TestCase(9264, 9267, 300000)]
        [TestCase(9264, 9268, 400000)]
        [TestCase(9265, 9265, 200000)]
        [TestCase(9265, 9266, 100000)]
        [TestCase(9265, 9267, 200000)]
        [TestCase(9265, 9268, 300000)]
        [TestCase(9266, 9267, 100000)]
        [TestCase(9266, 9268, 200000)]
        [TestCase(9267, 9267, 200000)]
        [TestCase(9267, 9268, 300000)]
        [TestCase(9268, 9268, 400000)]
        public void RankingForOnlyAdjustmentOutOfRange(int lower, int upper, int expectedRanking)
        {
            collection.Adjustment = 9266;

            var ranking = collection.GetRanking(lower, upper);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(1, 1001)]
        [TestCase(2, 11001)]
        [TestCase(3, 21001)]
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

        [TestCase(9264 + 1, 100 * 1 + 9264, 1, 401001)]
        [TestCase(9264 + 1, 100 * 1 + 9265, 1, 301001)]
        [TestCase(9264 + 1, 100 * 1 + 9266, 1, 201001)]
        [TestCase(9264 + 1, 100 * 1 + 9267, 1, 301001)]
        [TestCase(9264 + 1, 100 * 1 + 9268, 1, 401001)]
        [TestCase(9264 + 2, 100 * 2 + 9264, 2, 411001)]
        [TestCase(9264 + 2, 100 * 2 + 9265, 2, 311001)]
        [TestCase(9264 + 2, 100 * 2 + 9266, 2, 211001)]
        [TestCase(9264 + 2, 100 * 2 + 9267, 2, 311001)]
        [TestCase(9264 + 2, 100 * 2 + 9268, 2, 411001)]
        [TestCase(9264 + 3, 100 * 3 + 9264, 3, 421001)]
        [TestCase(9264 + 3, 100 * 3 + 9265, 3, 321001)]
        [TestCase(9264 + 3, 100 * 3 + 9266, 3, 221001)]
        [TestCase(9264 + 3, 100 * 3 + 9267, 3, 321001)]
        [TestCase(9264 + 3, 100 * 3 + 9268, 3, 421001)]
        [TestCase(9265 + 1, 100 * 1 + 9264, 1, 301001)]
        [TestCase(9265 + 1, 100 * 1 + 9265, 1, 201001)]
        [TestCase(9265 + 1, 100 * 1 + 9266, 1, 101001)]
        [TestCase(9265 + 1, 100 * 1 + 9267, 1, 201001)]
        [TestCase(9265 + 1, 100 * 1 + 9268, 1, 301001)]
        [TestCase(9265 + 2, 100 * 2 + 9264, 2, 311001)]
        [TestCase(9265 + 2, 100 * 2 + 9265, 2, 211001)]
        [TestCase(9265 + 2, 100 * 2 + 9266, 2, 111001)]
        [TestCase(9265 + 2, 100 * 2 + 9267, 2, 211001)]
        [TestCase(9265 + 2, 100 * 2 + 9268, 2, 311001)]
        [TestCase(9265 + 3, 100 * 3 + 9264, 3, 321001)]
        [TestCase(9265 + 3, 100 * 3 + 9265, 3, 221001)]
        [TestCase(9265 + 3, 100 * 3 + 9266, 3, 121001)]
        [TestCase(9265 + 3, 100 * 3 + 9267, 3, 221001)]
        [TestCase(9265 + 3, 100 * 3 + 9268, 3, 321001)]
        [TestCase(9266 + 1, 100 * 1 + 9264, 1, 201001)]
        [TestCase(9266 + 1, 100 * 1 + 9265, 1, 101001)]
        [TestCase(9266 + 1, 100 * 1 + 9266, 1, 1001)]
        [TestCase(9266 + 1, 100 * 1 + 9267, 1, 101001)]
        [TestCase(9266 + 1, 100 * 1 + 9268, 1, 201001)]
        [TestCase(9266 + 2, 100 * 2 + 9264, 2, 211001)]
        [TestCase(9266 + 2, 100 * 2 + 9265, 2, 111001)]
        [TestCase(9266 + 2, 100 * 2 + 9266, 2, 11001)]
        [TestCase(9266 + 2, 100 * 2 + 9267, 2, 111001)]
        [TestCase(9266 + 2, 100 * 2 + 9268, 2, 211001)]
        [TestCase(9266 + 3, 100 * 3 + 9264, 3, 221001)]
        [TestCase(9266 + 3, 100 * 3 + 9265, 3, 121001)]
        [TestCase(9266 + 3, 100 * 3 + 9266, 3, 21001)]
        [TestCase(9266 + 3, 100 * 3 + 9267, 3, 121001)]
        [TestCase(9266 + 3, 100 * 3 + 9268, 3, 221001)]
        [TestCase(9267 + 1, 100 * 1 + 9264, 1, 301001)]
        [TestCase(9267 + 1, 100 * 1 + 9265, 1, 201001)]
        [TestCase(9267 + 1, 100 * 1 + 9266, 1, 101001)]
        [TestCase(9267 + 1, 100 * 1 + 9267, 1, 201001)]
        [TestCase(9267 + 1, 100 * 1 + 9268, 1, 301001)]
        [TestCase(9267 + 2, 100 * 2 + 9264, 2, 311001)]
        [TestCase(9267 + 2, 100 * 2 + 9265, 2, 211001)]
        [TestCase(9267 + 2, 100 * 2 + 9266, 2, 111001)]
        [TestCase(9267 + 2, 100 * 2 + 9267, 2, 211001)]
        [TestCase(9267 + 2, 100 * 2 + 9268, 2, 311001)]
        [TestCase(9267 + 3, 100 * 3 + 9264, 3, 321001)]
        [TestCase(9267 + 3, 100 * 3 + 9265, 3, 221001)]
        [TestCase(9267 + 3, 100 * 3 + 9266, 3, 121001)]
        [TestCase(9267 + 3, 100 * 3 + 9267, 3, 221001)]
        [TestCase(9267 + 3, 100 * 3 + 9268, 3, 321001)]
        [TestCase(9268 + 1, 100 * 1 + 9264, 1, 401001)]
        [TestCase(9268 + 1, 100 * 1 + 9265, 1, 301001)]
        [TestCase(9268 + 1, 100 * 1 + 9266, 1, 201001)]
        [TestCase(9268 + 1, 100 * 1 + 9267, 1, 301001)]
        [TestCase(9268 + 1, 100 * 1 + 9268, 1, 401001)]
        [TestCase(9268 + 2, 100 * 2 + 9264, 2, 411001)]
        [TestCase(9268 + 2, 100 * 2 + 9265, 2, 311001)]
        [TestCase(9268 + 2, 100 * 2 + 9266, 2, 211001)]
        [TestCase(9268 + 2, 100 * 2 + 9267, 2, 311001)]
        [TestCase(9268 + 2, 100 * 2 + 9268, 2, 411001)]
        [TestCase(9268 + 3, 100 * 3 + 9264, 3, 421001)]
        [TestCase(9268 + 3, 100 * 3 + 9265, 3, 321001)]
        [TestCase(9268 + 3, 100 * 3 + 9266, 3, 221001)]
        [TestCase(9268 + 3, 100 * 3 + 9267, 3, 321001)]
        [TestCase(9268 + 3, 100 * 3 + 9268, 3, 421001)]
        public void RankingForOneRollOutOfRange(int lower, int upper, int quantity, int expectedRanking)
        {
            collection.Adjustment = 9266;

            var prototype = new RollPrototype
            {
                Quantity = quantity,
                Die = 100
            };

            collection.Rolls.Add(prototype);

            var ranking = collection.GetRanking(lower, upper);
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(1, 1, 12001)]
        [TestCase(2, 1, 22001)]
        [TestCase(3, 1, 32001)]
        [TestCase(1, 2, 22001)]
        [TestCase(2, 2, 32001)]
        [TestCase(3, 2, 42001)]
        [TestCase(1, 3, 32001)]
        [TestCase(2, 3, 42001)]
        [TestCase(3, 3, 52001)]
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

        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9264, 1, 1, 412001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9265, 1, 1, 312001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9266, 1, 1, 212001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9267, 1, 1, 312001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 1 + 9268, 1, 1, 412001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9264, 1, 1, 312001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9265, 1, 1, 212001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9266, 1, 1, 112001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9267, 1, 1, 212001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 1 + 9268, 1, 1, 312001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9264, 1, 1, 212001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9265, 1, 1, 112001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9266, 1, 1, 12001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9267, 1, 1, 112001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 1 + 9268, 1, 1, 212001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9264, 1, 1, 312001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9265, 1, 1, 212001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9266, 1, 1, 112001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9267, 1, 1, 212001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 1 + 9268, 1, 1, 312001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9264, 1, 1, 412001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9265, 1, 1, 312001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9266, 1, 1, 212001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9267, 1, 1, 312001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 1 + 9268, 1, 1, 412001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9264, 2, 1, 422001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9265, 2, 1, 322001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9266, 2, 1, 222001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9267, 2, 1, 322001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 1 + 9268, 2, 1, 422001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9264, 2, 1, 322001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9265, 2, 1, 222001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9266, 2, 1, 122001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9267, 2, 1, 222001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 1 + 9268, 2, 1, 322001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9264, 2, 1, 222001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9265, 2, 1, 122001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9266, 2, 1, 22001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9267, 2, 1, 122001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 1 + 9268, 2, 1, 222001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9264, 2, 1, 322001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9265, 2, 1, 222001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9266, 2, 1, 122001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9267, 2, 1, 222001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 1 + 9268, 2, 1, 322001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9264, 2, 1, 422001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9265, 2, 1, 322001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9266, 2, 1, 222001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9267, 2, 1, 322001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 1 + 9268, 2, 1, 422001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9264, 3, 1, 432001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9265, 3, 1, 332001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9266, 3, 1, 232001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9267, 3, 1, 332001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 1 + 9268, 3, 1, 432001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9264, 3, 1, 332001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9265, 3, 1, 232001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9266, 3, 1, 132001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9267, 3, 1, 232001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 1 + 9268, 3, 1, 332001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9264, 3, 1, 232001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9265, 3, 1, 132001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9266, 3, 1, 32001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9267, 3, 1, 132001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 1 + 9268, 3, 1, 232001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9264, 3, 1, 332001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9265, 3, 1, 232001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9266, 3, 1, 132001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9267, 3, 1, 232001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 1 + 9268, 3, 1, 332001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9264, 3, 1, 432001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9265, 3, 1, 332001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9266, 3, 1, 232001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9267, 3, 1, 332001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 1 + 9268, 3, 1, 432001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9264, 1, 2, 522001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9265, 1, 2, 422001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9266, 1, 2, 322001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9267, 1, 2, 422001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 2 + 9268, 1, 2, 522001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9264, 1, 2, 422001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9265, 1, 2, 322001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9266, 1, 2, 222001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9267, 1, 2, 322001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 2 + 9268, 1, 2, 422001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9264, 1, 2, 322001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9265, 1, 2, 222001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9266, 1, 2, 122001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9267, 1, 2, 222001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 2 + 9268, 1, 2, 322001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9264, 1, 2, 222001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9265, 1, 2, 122001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9266, 1, 2, 22001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9267, 1, 2, 122001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 2 + 9268, 1, 2, 222001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9264, 1, 2, 322001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9265, 1, 2, 222001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9266, 1, 2, 122001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9267, 1, 2, 222001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 2 + 9268, 1, 2, 322001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9264, 2, 2, 532001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9265, 2, 2, 432001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9266, 2, 2, 332001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9267, 2, 2, 432001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 2 + 9268, 2, 2, 532001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9264, 2, 2, 432001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9265, 2, 2, 332001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9266, 2, 2, 232001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9267, 2, 2, 332001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 2 + 9268, 2, 2, 432001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9264, 2, 2, 332001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9265, 2, 2, 232001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9266, 2, 2, 132001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9267, 2, 2, 232001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 2 + 9268, 2, 2, 332001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9264, 2, 2, 232001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9265, 2, 2, 132001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9266, 2, 2, 32001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9267, 2, 2, 132001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 2 + 9268, 2, 2, 232001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9264, 2, 2, 332001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9265, 2, 2, 232001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9266, 2, 2, 132001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9267, 2, 2, 232001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 2 + 9268, 2, 2, 332001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9264, 3, 2, 542001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9265, 3, 2, 442001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9266, 3, 2, 342001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9267, 3, 2, 442001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 2 + 9268, 3, 2, 542001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9264, 3, 2, 442001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9265, 3, 2, 342001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9266, 3, 2, 242001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9267, 3, 2, 342001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 2 + 9268, 3, 2, 442001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9264, 3, 2, 342001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9265, 3, 2, 242001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9266, 3, 2, 142001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9267, 3, 2, 242001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 2 + 9268, 3, 2, 342001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9264, 3, 2, 242001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9265, 3, 2, 142001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9266, 3, 2, 42001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9267, 3, 2, 142001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 2 + 9268, 3, 2, 242001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9264, 3, 2, 342001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9265, 3, 2, 242001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9266, 3, 2, 142001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9267, 3, 2, 242001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 2 + 9268, 3, 2, 342001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9264, 1, 3, 632001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9265, 1, 3, 532001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9266, 1, 3, 432001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9267, 1, 3, 532001)]
        [TestCase(1 + 1 + 9264, 100 * 1 + 20 * 3 + 9268, 1, 3, 632001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9264, 1, 3, 532001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9265, 1, 3, 432001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9266, 1, 3, 332001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9267, 1, 3, 432001)]
        [TestCase(1 + 1 + 9265, 100 * 1 + 20 * 3 + 9268, 1, 3, 532001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9264, 1, 3, 432001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9265, 1, 3, 332001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9266, 1, 3, 232001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9267, 1, 3, 332001)]
        [TestCase(1 + 1 + 9266, 100 * 1 + 20 * 3 + 9268, 1, 3, 432001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9264, 1, 3, 332001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9265, 1, 3, 232001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9266, 1, 3, 132001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9267, 1, 3, 232001)]
        [TestCase(1 + 1 + 9267, 100 * 1 + 20 * 3 + 9268, 1, 3, 332001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9264, 1, 3, 232001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9265, 1, 3, 132001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9266, 1, 3, 32001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9267, 1, 3, 132001)]
        [TestCase(1 + 1 + 9268, 100 * 1 + 20 * 3 + 9268, 1, 3, 232001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9264, 2, 3, 642001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9265, 2, 3, 542001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9266, 2, 3, 442001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9267, 2, 3, 542001)]
        [TestCase(2 + 1 + 9264, 100 * 2 + 20 * 3 + 9268, 2, 3, 642001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9264, 2, 3, 542001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9265, 2, 3, 442001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9266, 2, 3, 342001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9267, 2, 3, 442001)]
        [TestCase(2 + 1 + 9265, 100 * 2 + 20 * 3 + 9268, 2, 3, 542001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9264, 2, 3, 442001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9265, 2, 3, 342001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9266, 2, 3, 242001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9267, 2, 3, 342001)]
        [TestCase(2 + 1 + 9266, 100 * 2 + 20 * 3 + 9268, 2, 3, 442001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9264, 2, 3, 342001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9265, 2, 3, 242001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9266, 2, 3, 142001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9267, 2, 3, 242001)]
        [TestCase(2 + 1 + 9267, 100 * 2 + 20 * 3 + 9268, 2, 3, 342001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9264, 2, 3, 242001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9265, 2, 3, 142001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9266, 2, 3, 42001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9267, 2, 3, 142001)]
        [TestCase(2 + 1 + 9268, 100 * 2 + 20 * 3 + 9268, 2, 3, 242001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9264, 3, 3, 652001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9265, 3, 3, 552001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9266, 3, 3, 452001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9267, 3, 3, 552001)]
        [TestCase(3 + 1 + 9264, 100 * 3 + 20 * 3 + 9268, 3, 3, 652001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9264, 3, 3, 552001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9265, 3, 3, 452001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9266, 3, 3, 352001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9267, 3, 3, 452001)]
        [TestCase(3 + 1 + 9265, 100 * 3 + 20 * 3 + 9268, 3, 3, 552001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9264, 3, 3, 452001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9265, 3, 3, 352001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9266, 3, 3, 252001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9267, 3, 3, 352001)]
        [TestCase(3 + 1 + 9266, 100 * 3 + 20 * 3 + 9268, 3, 3, 452001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9264, 3, 3, 352001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9265, 3, 3, 252001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9266, 3, 3, 152001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9267, 3, 3, 252001)]
        [TestCase(3 + 1 + 9267, 100 * 3 + 20 * 3 + 9268, 3, 3, 352001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9264, 3, 3, 252001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9265, 3, 3, 152001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9266, 3, 3, 52001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9267, 3, 3, 152001)]
        [TestCase(3 + 1 + 9268, 100 * 3 + 20 * 3 + 9268, 3, 3, 252001)]
        public void RankingForMultipleRollsOutOfRange(int lower, int upper, int quantity1, int quantity2, int expectedRanking)
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
            Assert.That(ranking, Is.EqualTo(expectedRanking));
        }

        [TestCase(1, 8, 1, 2, -1, 12093)]
        [TestCase(1, 6, 1, 4, -1, 12095)]
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

        [TestCase(2, 1099)]
        [TestCase(3, 1098)]
        [TestCase(4, 1097)]
        [TestCase(6, 1095)]
        [TestCase(8, 1093)]
        [TestCase(10, 1091)]
        [TestCase(12, 1089)]
        [TestCase(20, 1081)]
        [TestCase(100, 1001)]
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
