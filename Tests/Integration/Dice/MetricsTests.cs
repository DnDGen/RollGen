using System;
using Ninject;
using NUnit.Framework;

namespace D20Dice.Tests.Integration.Dice
{
    [TestFixture]
    public class MetricsTests : DiceTests
    {
        [Inject]
        public IDice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        [Test]
        public void d2IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.d2();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d2SpeedTest()
        {
            Dice.d2();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d3IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.d3();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d3SpeedTest()
        {
            Dice.d3();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d4IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.d4();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d4SpeedTest()
        {
            Dice.d4();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d6IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.d6();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d6SpeedTest()
        {
            Dice.d6();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d8IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.d8();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d8SpeedTest()
        {
            Dice.d8();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d10IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.d10();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d10SpeedTest()
        {
            Dice.d10();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d12IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.d12();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d12SpeedTest()
        {
            Dice.d12();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d20IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.d20();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d20SpeedTest()
        {
            Dice.d20();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void PercentileIterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Percentile();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void PercentileSpeedTest()
        {
            Dice.Percentile();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void RollIterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll("1d10*2-1d10/2+12-10");

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void RollSpeedTest()
        {
            Dice.Roll("1d10*2-1d10/2+12-10");
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void RollIndexIterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.RollIndex(Random.Next());

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void RollIndexSpeedTest()
        {
            Dice.RollIndex(Random.Next());
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }
    }
}