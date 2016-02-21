using Ninject;
using NUnit.Framework;
using RollGen.Tests.Integration.Common;
using System;
using System.Diagnostics;

namespace RollGen.Tests.Integration.Rolls
{
    [TestFixture]
    public abstract class DiceTests : IntegrationTests
    {
        [Inject]
        public Stopwatch Stopwatch { get; set; }

        protected const int ConfidenceIterations = 1000000;

        private const int TimeLimitInSeconds = 1;

        private int iterations;

        [SetUp]
        public void DiceTestSetup()
        {
            iterations = 0;
            Stopwatch.Start();
        }

        [TearDown]
        public void DiceTestTearDown()
        {
            Stopwatch.Reset();
        }

        protected bool LoopShouldKeepRunning()
        {
            return iterations++ < ConfidenceIterations && Stopwatch.Elapsed.Seconds < TimeLimitInSeconds;
        }

        protected void Stress(Action makeAssertions)
        {
            do makeAssertions();
            while (LoopShouldKeepRunning());

            if (Stopwatch.Elapsed.TotalSeconds > TimeLimitInSeconds + 1)
                Assert.Fail("Something took way too long");
        }
    }
}