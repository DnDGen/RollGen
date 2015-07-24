using Ninject;
using NUnit.Framework;
using System;

namespace RollGen.Tests.Integration.Dice
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
                Dice.Roll().d2();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d2SpeedTest()
        {
            Dice.Roll().d2();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d3IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll().d3();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d3SpeedTest()
        {
            Dice.Roll().d3();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d4IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll().d4();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d4SpeedTest()
        {
            Dice.Roll().d4();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d6IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll().d6();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d6SpeedTest()
        {
            Dice.Roll().d6();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d8IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll().d8();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d8SpeedTest()
        {
            Dice.Roll().d8();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d10IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll().d10();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d10SpeedTest()
        {
            Dice.Roll().d10();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d12IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll().d12();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d12SpeedTest()
        {
            Dice.Roll().d12();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void d20IterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll().d20();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void d20SpeedTest()
        {
            Dice.Roll().d20();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void PercentileIterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll().Percentile();

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void PercentileSpeedTest()
        {
            Dice.Roll().Percentile();
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }

        [Test]
        public void dIterationsTest()
        {
            while (LoopShouldStillRun())
                Dice.Roll().d(Random.Next());

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void dSpeedTest()
        {
            Dice.Roll().d(Random.Next());
            Assert.Pass("Elapsed ticks: {0}", Stopwatch.ElapsedTicks);
        }
    }
}