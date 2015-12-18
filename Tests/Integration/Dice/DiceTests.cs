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

        protected const Int32 ConfidentIterations = 1000000;

        protected Int32 iterations;

        private const Int32 TimeLimitInSeconds = 1;

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

        protected Boolean LoopShouldStillRun()
        {
            return iterations++ < ConfidentIterations && Stopwatch.Elapsed.Seconds < TimeLimitInSeconds;
        }
    }
}