using System;
using System.Diagnostics;
using D20Dice.Tests.Integration.Common;
using Ninject;
using NUnit.Framework;

namespace D20Dice.Tests.Integration.Dice
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
            return iterations++ < 1000000 && Stopwatch.Elapsed.Seconds < TimeLimitInSeconds;
        }
    }
}