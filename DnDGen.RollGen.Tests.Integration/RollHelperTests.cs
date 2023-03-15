using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration
{
    [TestFixture]
    public class RollHelperTests : IntegrationTests
    {
        private Dice dice;
        private Stopwatch stopwatch;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
            stopwatch = new Stopwatch();
        }

        [TestCase(0, 0, "0")]
        [TestCase(1, 1, "1")]
        [TestCase(2, 2, "2")]
        [TestCase(9266, 9266, "9266")]
        [TestCase(0, 1, "1d2-1")]
        [TestCase(1, 2, "1d2")]
        [TestCase(1, 3, "1d3")]
        [TestCase(1, 4, "1d4")]
        [TestCase(1, 5, "2d3-1")]
        [TestCase(1, 6, "1d6")]
        [TestCase(1, 7, "2d4-1")]
        [TestCase(1, 8, "1d8")]
        [TestCase(1, 9, "4d3-3")]
        [TestCase(1, 10, "1d10")]
        [TestCase(1, 12, "1d12")]
        [TestCase(1, 20, "1d20")]
        [TestCase(1, 35, "17d3-16")]
        [TestCase(1, 36, "5d8-4")]
        [TestCase(1, 48, "47d2-46")]
        [TestCase(1, 100, "1d100")]
        [TestCase(1, 1_000, "111d10-110")]
        [TestCase(1, 10_000, "101d100-100")]
        [TestCase(1, 100_000, "1010d100+1d10-1010")]
        [TestCase(1, 1_000_000, "10000d100+101d100-10100")]
        [TestCase(2, 8, "2d4")]
        [TestCase(4, 9, "1d6+3")]
        [TestCase(5, 40, "5d8")]
        [TestCase(10, 1_000, "10d100")]
        [TestCase(10, 10_000, "1110d10-1100")]
        [TestCase(16, 50, "17d3-1")]
        [TestCase(100, 10_000, "100d100")]
        [TestCase(101, 200, "1d100+100")]
        [TestCase(437, 1204, "767d2-330")]
        [TestCase(1336, 90210, "897d100+71d2+368")]
        [TestCase(2714, 8095, "5381d2-2667")]
        [TestCase(10_000, 1_000_000, "10000d100")]
        public void RollWithFewestDice(int lower, int upper, string expectedRoll)
        {
            stopwatch.Start();
            var roll = RollHelper.GetRollWithFewestDice(lower, upper);
            stopwatch.Stop();

            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(73422562, 1673270503, "REPEAT+80d100+3d8+57262479", "10000d100", 1616)]
        [TestCase(239762129, 792745843, "REPEAT+5694d100+4d3+234176431", "10000d100", 558)]
        [TestCase(524600879, 1213158805, "REPEAT+5130d100+8d8+517645741", "10000d100", 695)]
        public void RollWithFewestDice_LongRoll(int lower, int upper, string rollTemplate, string repeatTerm, int repeatCount)
        {
            var repeats = Enumerable.Repeat(repeatTerm, repeatCount);
            var expectedRoll = rollTemplate.Replace("REPEAT", string.Join("+", repeats));

            stopwatch.Start();
            var roll = RollHelper.GetRollWithFewestDice(lower, upper);
            stopwatch.Stop();

            Assert.That(roll, Has.Length.EqualTo(expectedRoll.Length));
            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(0, 0, "0")]
        [TestCase(1, 1, "1")]
        [TestCase(2, 2, "2")]
        [TestCase(9266, 9266, "9266")]
        [TestCase(0, 1, "1d2-1")]
        [TestCase(1, 2, "1d2")]
        [TestCase(1, 3, "1d3")]
        [TestCase(1, 4, "1d4")]
        [TestCase(1, 5, "1d4+1d2-1")]
        [TestCase(1, 6, "1d6")]
        [TestCase(1, 7, "1d6+1d2-1")]
        [TestCase(1, 8, "1d8")]
        [TestCase(1, 10, "1d10")]
        [TestCase(1, 12, "1d12")]
        [TestCase(1, 20, "1d20")]
        [TestCase(1, 35, "1d20+1d12+1d4+1d2-3")]
        [TestCase(1, 36, "1d20+1d12+1d6-2")]
        [TestCase(1, 48, "2d20+1d10-2")]
        [TestCase(1, 100, "1d100")]
        [TestCase(1, 1_000, "10d100+1d10-10")]
        [TestCase(1, 10_000, "101d100-100")]
        [TestCase(1, 100_000, "1010d100+1d10-1010")]
        [TestCase(1, 1_000_000, "10000d100+101d100-10100")]
        [TestCase(1, 9, "1d8+1d2-1")]
        [TestCase(2, 8, "1d6+1d2")]
        [TestCase(4, 9, "1d6+3")]
        [TestCase(5, 40, "1d20+1d12+1d6+2")]
        [TestCase(10, 1_000, "10d100")]
        [TestCase(10, 10_000, "100d100+4d20+1d12+1d4-96")]
        [TestCase(16, 50, "1d20+1d12+1d4+1d2+12")]
        [TestCase(100, 10_000, "100d100")]
        [TestCase(101, 200, "1d100+100")]
        [TestCase(437, 1204, "7d100+3d20+1d12+1d6+1d2+424")]
        [TestCase(1336, 90210, "897d100+3d20+1d12+1d4+434")]
        [TestCase(2714, 8095, "54d100+1d20+1d12+1d6+2657")]
        [TestCase(10_000, 1_000_000, "10000d100")]
        public void RollWithMostEvenDistribution(int lower, int upper, string expectedRoll)
        {
            stopwatch.Start();
            var roll = RollHelper.GetRollWithMostEvenDistribution(lower, upper);
            stopwatch.Stop();

            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(73422562, 1673270503, "REPEAT+80d100+1d20+1d3+57262480", "10000d100", 1616)]
        [TestCase(239762129, 792745843, "REPEAT+5694d100+1d8+1d2+234176433", "10000d100", 558)]
        [TestCase(524600879, 1213158805, "REPEAT+5130d100+2d20+1d12+1d8+517645745", "10000d100", 695)]
        public void RollWithMostEvenDistribution_LongRoll(int lower, int upper, string rollTemplate, string repeatTerm, int repeatCount)
        {
            var repeats = Enumerable.Repeat(repeatTerm, repeatCount);
            var expectedRoll = rollTemplate.Replace("REPEAT", string.Join("+", repeats));

            stopwatch.Start();
            var roll = RollHelper.GetRollWithMostEvenDistribution(lower, upper);
            stopwatch.Stop();

            Assert.That(roll, Has.Length.EqualTo(expectedRoll.Length));
            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(0, 0, "0")]
        [TestCase(1, 1, "1")]
        [TestCase(2, 2, "2")]
        [TestCase(9266, 9266, "9266")]
        [TestCase(0, 1, "1d2-1")]
        [TestCase(1, 2, "1d2")]
        [TestCase(1, 3, "1d3")]
        [TestCase(1, 4, "1d4")]
        [TestCase(1, 5, "1d5")]
        [TestCase(1, 6, "1d6")]
        [TestCase(1, 7, "1d7")]
        [TestCase(1, 8, "1d8")]
        [TestCase(1, 9, "1d9")]
        [TestCase(1, 10, "1d10")]
        [TestCase(1, 12, "1d12")]
        [TestCase(1, 20, "1d20")]
        [TestCase(1, 35, "1d35")]
        [TestCase(1, 36, "1d36")]
        [TestCase(1, 48, "1d48")]
        [TestCase(1, 100, "1d100")]
        [TestCase(1, 1_000, "1d1000")]
        [TestCase(1, 10_000, "1d10000")]
        [TestCase(1, 100_000, "10d10000+1d10-10")]
        [TestCase(1, 1_000_000, "100d10000+1d100-100")]
        [TestCase(2, 8, "1d7+1")]
        [TestCase(4, 9, "1d6+3")]
        [TestCase(5, 40, "1d36+4")]
        [TestCase(10, 1_000, "1d991+9")]
        [TestCase(10, 10_000, "1d9991+9")]
        [TestCase(16, 50, "1d35+15")]
        [TestCase(100, 10_000, "1d9901+99")]
        [TestCase(101, 200, "1d100+100")]
        [TestCase(437, 1204, "1d768+436")]
        [TestCase(1336, 90210, "8d10000+1d8883+1327")]
        [TestCase(2714, 8095, "1d5382+2713")]
        [TestCase(10_000, 1_000_000, "99d10000+1d100+9900")]
        public void RollWithMostEvenDistribution_AllowNonstandard(int lower, int upper, string expectedRoll)
        {
            stopwatch.Start();
            var roll = RollHelper.GetRollWithMostEvenDistribution(lower, upper, allowNonstandardDice: true);
            stopwatch.Stop();

            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(73422562, 1673270503, "REPEAT+1d7942+73262561", "10000d10000", 16)]
        [TestCase(239762129, 792745843, "REPEAT+5303d10000+1d9018+239706825", "10000d10000", 5)]
        [TestCase(524600879, 1213158805, "REPEAT+8862d10000+1d6789+524532016", "10000d10000", 6)]
        public void RollWithMostEvenDistribution_AllowNonstandard_LongRoll(int lower, int upper, string rollTemplate, string repeatTerm, int repeatCount)
        {
            var repeats = Enumerable.Repeat(repeatTerm, repeatCount);
            var expectedRoll = rollTemplate.Replace("REPEAT", string.Join("+", repeats));

            stopwatch.Start();
            var roll = RollHelper.GetRollWithMostEvenDistribution(lower, upper, allowNonstandardDice: true);
            stopwatch.Stop();

            Assert.That(roll, Has.Length.EqualTo(expectedRoll.Length));
            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(0, 0, "0")]
        [TestCase(1, 1, "1")]
        [TestCase(2, 2, "2")]
        [TestCase(9266, 9266, "9266")]
        [TestCase(0, 1, "1d2-1")]
        [TestCase(1, 2, "1d2")]
        [TestCase(1, 3, "1d3")]
        [TestCase(1, 4, "1d4")]
        [TestCase(1, 5, "1d4+1d2-1")]
        [TestCase(1, 6, "1d6")]
        [TestCase(1, 7, "1d6+1d2-1")]
        [TestCase(1, 8, "1d8")]
        [TestCase(1, 9, "(1d3-1)*3+1d3")]
        [TestCase(1, 10, "1d10")]
        [TestCase(1, 12, "1d12")]
        [TestCase(1, 20, "1d20")]
        [TestCase(1, 35, "1d20+1d12+1d4+1d2-3")]
        [TestCase(1, 36, "(1d12-1)*3+1d3")]
        [TestCase(1, 48, "(1d12-1)*4+1d4")]
        [TestCase(1, 100, "1d100")]
        [TestCase(1, 1_000, "(1d100-1)*10+1d10")]
        [TestCase(1, 10_000, "(1d100-1)*100+1d100")]
        [TestCase(1, 100_000, "(1d100-1)*1000+(1d100-1)*10+1d10")]
        [TestCase(1, 1_000_000, "(1d100-1)*10000+(1d100-1)*100+1d100")]
        [TestCase(2, 8, "1d6+1d2")]
        [TestCase(4, 9, "1d6+3")]
        [TestCase(5, 40, "(1d12-1)*3+1d3+4")]
        [TestCase(10, 1_000, "10d100")]
        [TestCase(10, 10_000, "100d100+4d20+1d12+1d4-96")]
        [TestCase(16, 50, "1d20+1d12+1d4+1d2+12")]
        [TestCase(100, 10_000, "100d100")]
        [TestCase(101, 200, "1d100+100")]
        [TestCase(437, 1204, "(1d12-1)*64+(1d8-1)*8+1d8+436")]
        [TestCase(1336, 90210, "(1d3-1)*29625+(1d3-1)*9875+99d100+3d20+1d12+1d6+1232")]
        [TestCase(2714, 8095, "(1d6-1)*897+(1d3-1)*299+3d100+1d2+2710")]
        [TestCase(10_000, 1_000_000, "10000d100")]
        public void RollWithMostEvenDistribution_AllowMultipliers(int lower, int upper, string expectedRoll)
        {
            stopwatch.Start();
            var roll = RollHelper.GetRollWithMostEvenDistribution(lower, upper, true);
            stopwatch.Stop();

            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(73422562, 1673270503, "(1d2-1)*799923971+REPEAT+40d100+1d10+1d2+65342520", "10000d100", 808)]
        [TestCase(239762129, 792745843, "(1d3-1)*184327905+(1d3-1)*61442635+REPEAT+632d100+3d20+1d10+239141493", "10000d100", 62)]
        [TestCase(524600879, 1213158805, "(1d3-1)*229519309+REPEAT+8376d100+4d20+1d8+1d2+522282497", "10000d100", 231)]
        public void RollWithMostEvenDistribution_AllowMultipliers_LongRoll(int lower, int upper, string rollTemplate, string repeatTerm, int repeatCount)
        {
            var repeats = Enumerable.Repeat(repeatTerm, repeatCount);
            var expectedRoll = rollTemplate.Replace("REPEAT", string.Join("+", repeats));

            stopwatch.Start();
            var roll = RollHelper.GetRollWithMostEvenDistribution(lower, upper, true);
            stopwatch.Stop();

            Assert.That(roll, Has.Length.EqualTo(expectedRoll.Length));
            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(0, 0, "0")]
        [TestCase(1, 1, "1")]
        [TestCase(2, 2, "2")]
        [TestCase(9266, 9266, "9266")]
        [TestCase(0, 1, "1d2-1")]
        [TestCase(1, 2, "1d2")]
        [TestCase(1, 3, "1d3")]
        [TestCase(1, 4, "1d4")]
        [TestCase(1, 5, "1d5")]
        [TestCase(1, 6, "1d6")]
        [TestCase(1, 7, "1d7")]
        [TestCase(1, 8, "1d8")]
        [TestCase(1, 9, "(1d3-1)*3+1d3")]
        [TestCase(1, 10, "1d10")]
        [TestCase(1, 12, "1d12")]
        [TestCase(1, 20, "1d20")]
        [TestCase(1, 35, "1d35")]
        [TestCase(1, 36, "(1d12-1)*3+1d3")]
        [TestCase(1, 48, "(1d12-1)*4+1d4")]
        [TestCase(1, 100, "1d100")]
        [TestCase(1, 1_000, "(1d100-1)*10+1d10")]
        [TestCase(1, 10_000, "(1d100-1)*100+1d100")]
        [TestCase(1, 100_000, "(1d100-1)*1000+(1d100-1)*10+1d10")]
        [TestCase(1, 1_000_000, "(1d100-1)*10000+(1d100-1)*100+1d100")]
        [TestCase(2, 8, "1d7+1")]
        [TestCase(4, 9, "1d6+3")]
        [TestCase(5, 40, "(1d12-1)*3+1d3+4")]
        [TestCase(10, 1_000, "1d991+9")]
        [TestCase(10, 10_000, "1d9991+9")]
        [TestCase(16, 50, "1d35+15")]
        [TestCase(100, 10_000, "1d9901+99")]
        [TestCase(101, 200, "1d100+100")]
        [TestCase(437, 1204, "(1d12-1)*64+(1d8-1)*8+1d8+436")]
        [TestCase(1336, 90210, "(1d3-1)*29625+(1d3-1)*9875+1d9875+1335")]
        [TestCase(2714, 8095, "(1d6-1)*897+(1d3-1)*299+1d299+2713")]
        [TestCase(10_000, 1_000_000, "99d10000+1d100+9900")]
        public void RollWithMostEvenDistribution_AllowMultipliersAndNonstandard(int lower, int upper, string expectedRoll)
        {
            stopwatch.Start();
            var roll = RollHelper.GetRollWithMostEvenDistribution(lower, upper, true, true);
            stopwatch.Stop();

            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }

        [TestCase(73422562, 1673270503, "(1d2-1)*799923971+REPEAT+1d3971+73342561", "10000d10000", 8)]
        [TestCase(239762129, 792745843, "(1d3-1)*184327905+(1d3-1)*61442635+6144d10000+1d8779+239755984", "REPEAT", 0)]
        [TestCase(524600879, 1213158805, "(1d3-1)*229519309+REPEAT+2954d10000+1d2263+524577924", "10000d10000", 2)]
        public void RollWithMostEvenDistribution_AllowMultipliersAndNonstandard_LongRoll(int lower, int upper, string rollTemplate, string repeatTerm, int repeatCount)
        {
            var repeats = Enumerable.Repeat(repeatTerm, repeatCount);
            var expectedRoll = rollTemplate.Replace("REPEAT", string.Join("+", repeats));

            stopwatch.Start();
            var roll = RollHelper.GetRollWithMostEvenDistribution(lower, upper, true, true);
            stopwatch.Stop();

            Assert.That(roll, Has.Length.EqualTo(expectedRoll.Length));
            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }
    }
}
