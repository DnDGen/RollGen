using NUnit.Framework;
using System;
using System.Collections;
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
        public void StandardDice_AreCorrect()
        {
            Assert.That(RollCollection.StandardDice, Is.EqualTo(new[] { 2, 3, 4, 6, 8, 10, 12, 20, 100 }));
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

        [Test]
        public void BuildRollWithMultiplier()
        {
            var prototype = new RollPrototype
            {
                Quantity = 9266,
                Die = 100,
                Multiplier = 90210
            };
            collection.Rolls.Add(prototype);

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo("(9266d100-9266)*90210"));
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

        [TestCase(-600, "(9266d100-9266)*90210-600")]
        [TestCase(0, "(9266d100-9266)*90210")]
        [TestCase(42, "(9266d100-9266)*90210+42")]
        public void BuildRollWithMultiplierAndAdjustment(int adjustment, string expectedroll)
        {
            var prototype = new RollPrototype
            {
                Quantity = 9266,
                Die = 100,
                Multiplier = 90210
            };

            collection.Rolls.Add(prototype);

            collection.Adjustment = adjustment;

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo(expectedroll));
        }

        [Test]
        public void BuildMultipleRolls()
        {
            var prototype1 = new RollPrototype
            {
                Quantity = 600,
                Die = 12,
                Multiplier = 1337
            };

            var prototype2 = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            var prototype3 = new RollPrototype
            {
                Quantity = 42,
                Die = 20
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);
            collection.Rolls.Add(prototype3);
            collection.Adjustment = 1336;

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo("(600d12-600)*1337+9266d100+42d20+1336"));
        }

        [Test]
        public void CollectionDescription()
        {
            var prototype1 = new RollPrototype
            {
                Quantity = 600,
                Die = 12,
                Multiplier = 1337
            };

            var prototype2 = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            var prototype3 = new RollPrototype
            {
                Quantity = 42,
                Die = 20
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);
            collection.Rolls.Add(prototype3);
            collection.Adjustment = 1336;

            Assert.That(collection.ToString(), Is.EqualTo("(600d12-600)*1337+9266d100+42d20+1336"));
        }

        [TestCase(-1336, "(600d12-600)*1337+9266d100+42d20-1336")]
        [TestCase(0, "(600d12-600)*1337+9266d100+42d20")]
        [TestCase(96, "(600d12-600)*1337+9266d100+42d20+96")]
        public void BuildMultipleRollsWithAdjustment(int adjustment, string expectedRoll)
        {
            var prototype1 = new RollPrototype
            {
                Quantity = 600,
                Die = 12,
                Multiplier = 1337
            };

            var prototype2 = new RollPrototype
            {
                Quantity = 9266,
                Die = 100
            };

            var prototype3 = new RollPrototype
            {
                Quantity = 42,
                Die = 20
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);
            collection.Rolls.Add(prototype3);
            collection.Adjustment = adjustment;

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo(expectedRoll));
        }

        [Test]
        public void BuildOrdersDiceFromLargestToSmallest()
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
        public void BuildOrdersMultipliersFromLargestToSmallest()
        {
            var prototype1 = new RollPrototype
            {
                Quantity = 9266,
                Die = 90210,
                Multiplier = 42
            };

            var prototype2 = new RollPrototype
            {
                Quantity = 600,
                Die = 1337,
                Multiplier = 1336
            };

            var prototype3 = new RollPrototype
            {
                Quantity = 96,
                Die = 783,
                Multiplier = 8245
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);
            collection.Rolls.Add(prototype3);

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo("(96d783-96)*8245+(600d1337-600)*1336+(9266d90210-9266)*42"));
        }

        [Test]
        public void BuildOrdersDiceFromLargestToSmallest_WithMultipliers()
        {
            var prototype = new RollPrototype
            {
                Quantity = 9266,
                Die = 20
            };

            var otherPrototype = new RollPrototype
            {
                Quantity = 42,
                Die = 12
            };

            var anotherPrototype = new RollPrototype
            {
                Quantity = 1337,
                Die = 100
            };

            var prototype1 = new RollPrototype
            {
                Quantity = 922,
                Die = 90210,
                Multiplier = 2022
            };

            var prototype2 = new RollPrototype
            {
                Quantity = 600,
                Die = 227,
                Multiplier = 1336
            };

            var prototype3 = new RollPrototype
            {
                Quantity = 96,
                Die = 783,
                Multiplier = 8245
            };

            collection.Rolls.Add(prototype);
            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(otherPrototype);
            collection.Rolls.Add(prototype2);
            collection.Rolls.Add(anotherPrototype);
            collection.Rolls.Add(prototype3);

            var roll = collection.Build();
            Assert.That(roll, Is.EqualTo("(96d783-96)*8245+(922d90210-922)*2022+(600d227-600)*1336+1337d100+9266d20+42d12"));
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
        public void ComputeDistribution(int q, int d, long D)
        {
            var prototype = new RollPrototype
            {
                Quantity = q,
                Die = d
            };

            collection.Rolls.Add(prototype);
            collection.Adjustment = 666;

            var distribution = collection.ComputeDistribution();
            var rawDistribution = ComputeDistribution(collection.Rolls);
            Assert.That(distribution, Is.EqualTo(D).And.EqualTo(rawDistribution));
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
        public void ComputeDistribution_WithMultiplier(int q, int d, long D)
        {
            var prototype1 = new RollPrototype
            {
                Quantity = q,
                Die = d
            };
            var prototype2 = new RollPrototype
            {
                Quantity = 9266,
                Die = 90210,
                Multiplier = 42,
            };

            collection.Rolls.Add(prototype1);
            collection.Rolls.Add(prototype2);
            collection.Adjustment = 666;

            var distribution = collection.ComputeDistribution();
            var rawDistribution = ComputeDistribution(collection.UnmultipliedRolls.ToList());
            Assert.That(distribution, Is.EqualTo(D).And.EqualTo(rawDistribution));
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
        [TestCase(1, 8, 2, 6, 32)]
        [TestCase(1, 8, 2, 8, 48)]
        [TestCase(1, 8, 2, 10, 64)]
        [TestCase(1, 8, 2, 12, 80)]
        [TestCase(1, 8, 2, 20, 144)]
        [TestCase(1, 8, 2, 100, 784)]
        [TestCase(1, 8, 2, 42, 320)]
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
        [TestCase(2, 2, 2, 2, 6)]
        [TestCase(2, 2, 2, 3, 10)]
        [TestCase(2, 2, 2, 4, 14)]
        [TestCase(2, 2, 2, 6, 22)]
        [TestCase(2, 2, 2, 8, 30)]
        [TestCase(2, 2, 2, 10, 38)]
        [TestCase(2, 2, 2, 12, 46)]
        [TestCase(2, 2, 2, 20, 78)]
        [TestCase(2, 2, 2, 100, 398)]
        [TestCase(2, 2, 2, 42, 166)]
        [TestCase(2, 3, 2, 2, 10)]
        [TestCase(2, 3, 2, 3, 19)]
        [TestCase(2, 3, 2, 4, 28)]
        [TestCase(2, 3, 2, 6, 46)]
        [TestCase(2, 3, 2, 8, 64)]
        [TestCase(2, 3, 2, 10, 82)]
        [TestCase(2, 3, 2, 12, 100)]
        [TestCase(2, 3, 2, 20, 172)]
        [TestCase(2, 3, 2, 100, 892)]
        [TestCase(2, 3, 2, 42, 370)]
        [TestCase(2, 4, 2, 2, 14)]
        [TestCase(2, 4, 2, 3, 28)]
        [TestCase(2, 4, 2, 4, 44)]
        [TestCase(2, 4, 2, 6, 76)]
        [TestCase(2, 4, 2, 8, 108)]
        [TestCase(2, 4, 2, 10, 140)]
        [TestCase(2, 4, 2, 12, 172)]
        [TestCase(2, 4, 2, 20, 300)]
        [TestCase(2, 4, 2, 100, 1580)]
        [TestCase(2, 4, 2, 42, 652)]
        [TestCase(2, 6, 2, 2, 22)]
        [TestCase(2, 6, 2, 3, 46)]
        [TestCase(2, 6, 2, 4, 76)]
        [TestCase(2, 6, 2, 6, 146)]
        [TestCase(2, 6, 2, 8, 218)] //9.46% * 6^2*8^2 = 218
        [TestCase(2, 6, 2, 10, 290)]
        [TestCase(2, 6, 2, 12, 362)]
        [TestCase(2, 6, 2, 20, 650)]
        [TestCase(2, 6, 2, 100, 3530)]
        [TestCase(2, 6, 2, 42, 1442)]
        [TestCase(2, 8, 2, 2, 30)]
        [TestCase(2, 8, 2, 3, 64)]
        [TestCase(2, 8, 2, 4, 108)]
        [TestCase(2, 8, 2, 6, 218)]
        [TestCase(2, 8, 2, 8, 344)]
        [TestCase(2, 8, 2, 10, 472)]
        [TestCase(2, 8, 2, 12, 600)]
        [TestCase(2, 8, 2, 20, 1112)]
        [TestCase(2, 8, 2, 100, 6232)]
        [TestCase(2, 8, 2, 42, 2520)]
        [TestCase(2, 10, 2, 2, 38)]
        [TestCase(2, 10, 2, 3, 82)]
        [TestCase(2, 10, 2, 4, 140)]
        [TestCase(2, 10, 2, 6, 290)]
        [TestCase(2, 10, 2, 8, 472)]
        [TestCase(2, 10, 2, 10, 670)]
        [TestCase(2, 10, 2, 12, 870)]
        [TestCase(2, 10, 2, 20, 1670)]
        [TestCase(2, 10, 2, 100, 9670)]
        [TestCase(2, 10, 2, 42, 3870)]
        [TestCase(2, 12, 2, 2, 46)]
        [TestCase(2, 12, 2, 3, 100)]
        [TestCase(2, 12, 2, 4, 172)]
        [TestCase(2, 12, 2, 6, 362)]
        [TestCase(2, 12, 2, 8, 600)]
        [TestCase(2, 12, 2, 10, 870)]
        [TestCase(2, 12, 2, 12, 1156)]
        [TestCase(2, 12, 2, 20, 2308)]
        [TestCase(2, 12, 2, 100, 13828)]
        [TestCase(2, 12, 2, 42, 5476)]
        [TestCase(2, 20, 2, 2, 78)]
        [TestCase(2, 20, 2, 3, 172)]
        [TestCase(2, 20, 2, 4, 300)]
        [TestCase(2, 20, 2, 6, 650)]
        [TestCase(2, 20, 2, 8, 1112)]
        [TestCase(2, 20, 2, 10, 1670)]
        [TestCase(2, 20, 2, 12, 2308)]
        [TestCase(2, 20, 2, 20, 5340)]
        [TestCase(2, 20, 2, 100, 37340)]
        [TestCase(2, 20, 2, 42, 14140)]
        [TestCase(2, 100, 2, 2, 398)]
        [TestCase(2, 100, 2, 3, 892)]
        [TestCase(2, 100, 2, 4, 1580)]
        [TestCase(2, 100, 2, 6, 3530)]
        [TestCase(2, 100, 2, 8, 6232)]
        [TestCase(2, 100, 2, 10, 9670)]
        [TestCase(2, 100, 2, 12, 13828)]
        [TestCase(2, 100, 2, 20, 37340)]
        [TestCase(2, 100, 2, 100, 666700)]
        [TestCase(2, 100, 2, 42, 151718)]
        [TestCase(2, 42, 2, 2, 166)]
        [TestCase(2, 42, 2, 3, 370)]
        [TestCase(2, 42, 2, 4, 652)]
        [TestCase(2, 42, 2, 6, 1442)]
        [TestCase(2, 42, 2, 8, 2520)]
        [TestCase(2, 42, 2, 10, 3870)]
        [TestCase(2, 42, 2, 12, 5476)]
        [TestCase(2, 42, 2, 20, 14140)]
        [TestCase(2, 42, 2, 100, 151718)]
        [TestCase(2, 42, 2, 42, 49406)]
        [TestCase(2, 10, 8, 10, 432457640)]
        [TestCase(90, 100, 10, 10, long.MaxValue)]
        public void ComputeDistribution_TwoRolls(int q1, int d1, int q2, int d2, long D)
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
            var rawDistribution = ComputeDistribution(collection.Rolls);
            Assert.That(distribution, Is.EqualTo(D).And.EqualTo(rawDistribution));
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
        [TestCase(90, 100, 4, 20, 2, 8, long.MaxValue)]
        public void ComputeDistribution_ThreeRolls(int q1, int d1, int q2, int d2, int q3, int d3, long D)
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
            var rawDistribution = ComputeDistribution(collection.Rolls);
            Assert.That(distribution, Is.EqualTo(D).And.EqualTo(rawDistribution));
        }

        [TestCase(1, 100, 1)]
        [TestCase(1, Limits.Die, 1)]
        [TestCase(2, 2, 2)]
        [TestCase(2, 10, 10)]
        [TestCase(2, 100, 100)]
        [TestCase(2, Limits.Die, 10_000)]
        [TestCase(3, 2, 3)]
        [TestCase(3, 6, 27)]
        [TestCase(3, 10, 75)]
        [TestCase(3, 20, 300)] //3.75% * 20^3 = 300
        [TestCase(3, 100, 7500)] //0.75% * 100^3 = 7500
        [TestCase(3, Limits.Die, 75000000)] //0.75% * 10,000^3 = 75000000
        [TestCase(4, 2, 6)]
        [TestCase(4, 6, 146)]
        [TestCase(4, 10, 670)]
        [TestCase(4, 20, 5340)]
        [TestCase(4, 100, 666700)]
        [TestCase(4, Limits.Die, 666666670000)]
        [TestCase(5, 2, 10)]
        [TestCase(5, 6, 780)]
        [TestCase(5, 10, 6000)]
        [TestCase(5, 20, 95875)]
        [TestCase(5, 100, 59896875)]
        [TestCase(5, Limits.Die, 5989583343750000)]
        [TestCase(6, 2, 20)]
        [TestCase(6, 6, 4332)]
        [TestCase(6, 10, 55252)]
        [TestCase(6, 20, 1762004)]
        [TestCase(6, 100, 5500250020)]
        [TestCase(6, Limits.Die, long.MaxValue)]
        [TestCase(8, 2, 70)]
        [TestCase(8, 10, 4816030)]
        [TestCase(8, 100, 47938730314300)]
        [TestCase(10, 2, 252)]
        [TestCase(10, 6, 4395456)] //7.27% * 6^10 = 4395891, rounding error
        [TestCase(10, 10, 432457640)]
        [TestCase(10, 20, 220633615280)]
        [TestCase(10, 100, 430438025018576400)]
        [TestCase(10, Limits.Die, long.MaxValue)]
        [TestCase(16, 2, 12870)]
        [TestCase(16, 10, 343900019857310)]
        [TestCase(16, 100, long.MaxValue)]
        [TestCase(20, 2, 184756)]
        [TestCase(20, 6, 189456975899496)]
        [TestCase(20, 10, 3081918923741896840)]
        [TestCase(20, 20, long.MaxValue)]
        [TestCase(20, 100, long.MaxValue)]
        [TestCase(20, Limits.Die, long.MaxValue)]
        [TestCase(32, 2, 601080390)]
        [TestCase(32, 10, long.MaxValue)]
        [TestCase(32, 100, long.MaxValue)]
        [TestCase(63, 2, 916312070471295267)]
        [TestCase(63, 10, long.MaxValue)]
        [TestCase(63, 100, long.MaxValue)]
        [TestCase(64, 2, 1832624140942590534)]
        [TestCase(64, 10, long.MaxValue)]
        [TestCase(64, 100, long.MaxValue)]
        [TestCase(65, 2, 3609714217008132870)]
        [TestCase(65, 10, long.MaxValue)]
        [TestCase(65, 100, long.MaxValue)]
        [TestCase(66, 2, 7219428434016265740)]
        [TestCase(66, 10, long.MaxValue)]
        [TestCase(66, 100, long.MaxValue)]
        [TestCase(67, 2, long.MaxValue)]
        [TestCase(67, 10, long.MaxValue)]
        [TestCase(67, 100, long.MaxValue)]
        [TestCase(100, 2, long.MaxValue)]
        [TestCase(100, 6, long.MaxValue)]
        [TestCase(100, 10, long.MaxValue)]
        [TestCase(100, 20, long.MaxValue)]
        [TestCase(100, 100, long.MaxValue)]
        [TestCase(100, Limits.Die, long.MaxValue)]
        [TestCase(128, 2, long.MaxValue)]
        [TestCase(128, 10, long.MaxValue)]
        [TestCase(128, 100, long.MaxValue)]
        [TestCase(1000, 2, long.MaxValue)]
        [TestCase(1000, 6, long.MaxValue)]
        [TestCase(1000, 10, long.MaxValue)]
        [TestCase(1000, 20, long.MaxValue)]
        [TestCase(1000, 100, long.MaxValue)]
        [TestCase(1000, Limits.Die, long.MaxValue)]
        [TestCase(Limits.Quantity, 2, long.MaxValue)]
        [TestCase(Limits.Quantity, 3, long.MaxValue)]
        [TestCase(Limits.Quantity, 4, long.MaxValue)]
        [TestCase(Limits.Quantity, 6, long.MaxValue)]
        [TestCase(Limits.Quantity, 8, long.MaxValue)]
        [TestCase(Limits.Quantity, 10, long.MaxValue)]
        [TestCase(Limits.Quantity, 12, long.MaxValue)]
        [TestCase(Limits.Quantity, 20, long.MaxValue)]
        [TestCase(Limits.Quantity, 100, long.MaxValue)]
        [TestCase(Limits.Quantity, Limits.Die, long.MaxValue)]
        public void ComputeDistribution_IsFast(int q1, int d1, long D)
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

            var rawDistribution = ComputeDistribution(collection.Rolls);
            Assert.That(distribution, Is.EqualTo(D).And.EqualTo(rawDistribution));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(2, 20, 2, 12, 2308)] //4.01% * 20^2*12^2 = 2310, with rounding error
        [TestCase(2, 100, 2, 20, 37340)] //0.93% * 100^2*20^2 = 37200, with rounding error
        [TestCase(2, 100, 3, 20, 735050)] //0.92% * 100^2*20^3 = 736000, with rounding error
        [TestCase(3, 10, 4, 12, 948501)] //4.57% * 10^3*12^4 = 947635, with rounding error
        [TestCase(3, 100, 2, 20, 2973400)] //0.74% * 100^3*20^2 = 2960000, with rounding error
        [TestCase(3, 100, 3, 20, 59204000)] //0.74% * 100^3*20^3 = 59200000, with rounding error
        [TestCase(20, 100, 20, 20, long.MaxValue)]
        [TestCase(100, 100, 100, 20, long.MaxValue)]
        [TestCase(Limits.Quantity, 100, Limits.Quantity, 20, long.MaxValue)]
        public void ComputeDistribution_TwoRolls_IsFast(int q1, int d1, int q2, int d2, long D)
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

            var rawDistribution = ComputeDistribution(collection.Rolls);
            Assert.That(distribution, Is.EqualTo(D).And.EqualTo(rawDistribution));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        private long ComputeDistribution(List<RollPrototype> rollPrototypes)
        {
            //We want to shortcut that when 1% of the possible iterations for the first die is greater than the max long,
            //Equation for xdy: 0.01y^x = 2^63
            //Solved for x, x = (2ln(10)+63ln(2))/ln(y)
            var quantityLimit = (2 * Math.Log(10) + 63 * Math.Log(2)) / Math.Log(rollPrototypes[0].Die);
            if (rollPrototypes[0].Quantity >= quantityLimit)
                return long.MaxValue;

            var quantities = rollPrototypes.Sum(r => r.Quantity);
            var mode = (rollPrototypes.Sum(r => r.Quantity * r.Die) + quantities) / 2;
            var rolls = new Dictionary<int, long>() { { mode, 1 } };

            for (var i = 0; i < rollPrototypes.Count; i++)
            {
                var nextRolls = Enumerable.Range(1, rollPrototypes[i].Die);
                var q = rollPrototypes[i].Quantity;

                while (q-- > 0)
                {
                    var newRolls = new Dictionary<int, long>();

                    foreach (var r1 in rolls)
                    {
                        foreach (var r2 in nextRolls)
                        {
                            var newSum = r1.Key - r2;

                            //Since we are always subtracting, once we are below 0, we won't ever get back to 0
                            //0 represents the permutations that result in the mode
                            //Also, nextRolls is ordered to increase, so once this is below 0, all following will be as well
                            if (newSum < 0)
                                break;

                            if (!newRolls.ContainsKey(newSum))
                                newRolls[newSum] = 0;

                            newRolls[newSum] += r1.Value;

                            //INFO: This means we went so high that we wrapped around
                            if (newRolls[newSum] < 1)
                                return long.MaxValue;
                        }
                    }

                    rolls = newRolls;
                }
            }

            //Since we are subtracting from the mode, the key of 0 is the cumulative number of ways we can roll the mode
            return rolls[0];
        }

        [TestCaseSource(nameof(SpecificDistributions))]
        public void BUG_DistributionIsCorrect(string roll, long D, List<(int Quantity, int Die)> rolls)
        {
            var prototypes = rolls.Select(r => new RollPrototype { Quantity = r.Quantity, Die = r.Die });
            collection.Rolls.AddRange(prototypes);

            var distribution = collection.ComputeDistribution();
            Assert.That(distribution, Is.EqualTo(D), roll);

            //HACK: We are ignoring this, as computing the raw distribution for these rolls takes too long
            //var rawDistribution = ComputeDistribution(collection.Rolls);
            //Assert.That(distribution, Is.EqualTo(rawDistribution), roll);
        }

        public static IEnumerable SpecificDistributions
        {
            get
            {
                yield return new TestCaseData("99d10000+1d100", long.MaxValue, new List<(int Quantity, int Die)> { (99, Limits.Die), (1, 100) });
                yield return new TestCaseData("100d10000", long.MaxValue, new List<(int Quantity, int Die)> { (100, Limits.Die) });
                yield return new TestCaseData("99d100+1d10", long.MaxValue, new List<(int Quantity, int Die)> { (99, 100), (1, 10) });
                yield return new TestCaseData("100d100", long.MaxValue, new List<(int Quantity, int Die)> { (100, 100) });
                yield return new TestCaseData("9d100+1d10", 45_270_937_006_218_890, new List<(int Quantity, int Die)> { (9, 100), (1, 10) });
                yield return new TestCaseData("10d100", 430_438_025_018_576_400, new List<(int Quantity, int Die)> { (10, 100) });
                yield return new TestCaseData("9d20+1d2", 23_207_634_900, new List<(int Quantity, int Die)> { (9, 20), (1, 2) });
                yield return new TestCaseData("10d20", 220_633_615_280, new List<(int Quantity, int Die)> { (10, 20) });
            }
        }

        [Test]
        public void IfUpperIsGreaterThanMaxInt_CollectionIsInvalid_BecauseUpperWontMatch()
        {
            var quantity = 22;
            while (quantity-- > 0)
            {
                var prototype = new RollPrototype
                {
                    Quantity = Limits.Quantity,
                    Die = Limits.Die
                };
                collection.Rolls.Add(prototype);
            }

            var isMatch = collection.Matches(22 * Limits.Quantity, int.MaxValue);
            Assert.That(isMatch, Is.False);
        }

        [Test]
        public void IfUpperIsGreaterThanMaxInt_CollectionIsStillValid_WhenAdjustmentLowersItWithinRange()
        {
            var quantity = 22;
            while (quantity-- > 0)
            {
                var prototype = new RollPrototype
                {
                    Quantity = Limits.Quantity,
                    Die = Limits.Die
                };
                collection.Rolls.Add(prototype);
            }

            collection.Adjustment = -1_000_000_000;

            var isMatch = collection.Matches(22 * Limits.Quantity - 1_000_000_000, 1_200_000_000);
            Assert.That(isMatch, Is.True);
        }

        [Test]
        public void IfUpperIsLessThanMaxInt_CollectionIsValid()
        {
            var quantity = 21;
            while (quantity-- > 0)
            {
                var prototype = new RollPrototype
                {
                    Quantity = Limits.Quantity,
                    Die = Limits.Die
                };
                collection.Rolls.Add(prototype);
            }

            collection.Adjustment = -1_000_000_000;

            var isMatch = collection.Matches(21 * Limits.Quantity - 1_000_000_000, 1_100_000_000);
            Assert.That(isMatch, Is.True);
        }
    }
}
