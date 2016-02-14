using Ninject;
using NUnit.Framework;
using RollGen.Tests.Integration.Common;
using System.Diagnostics;

namespace RollGen.Tests.Integration.Rolls
{
    [TestFixture]
    public abstract class DiceTests : IntegrationTests
    {
        [Inject]
        public Stopwatch Stopwatch { get; set; }

        protected const int ConfidentIterations = 1000000;

        protected int iterations;

        private const int TimeLimitInSeconds = 1;

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

        protected bool LoopShouldStillRun()
        {
            return iterations++ < ConfidentIterations && Stopwatch.Elapsed.Seconds < TimeLimitInSeconds;
        }
    }
}