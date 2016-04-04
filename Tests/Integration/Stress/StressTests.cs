using Ninject;
using NUnit.Framework;
using RollGen.Tests.Integration.Common;
using System;
using System.Diagnostics;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class StressTests : IntegrationTests
    {
        [Inject]
        public Stopwatch Stopwatch { get; set; }

        protected const int ConfidenceIterations = 1000000;
        private const int TenMinutesInSeconds = 600;

        private readonly int timeLimitInSeconds;

        private int iterations;

        public StressTests()
        {
#if STRESS
            timeLimitInSeconds = TenMinutesInSeconds - 10;
#else
            timeLimitInSeconds = 1;
#endif
        }

        [SetUp]
        public void StressTestSetup()
        {
            iterations = 0;
            Stopwatch.Start();
        }

        [TearDown]
        public void StressTestTearDown()
        {
            Stopwatch.Reset();
        }

        protected bool TestShouldKeepRunning()
        {
            iterations++;
            return iterations < ConfidenceIterations && Stopwatch.Elapsed.TotalSeconds < timeLimitInSeconds;
        }

        protected void Stress(Action makeAssertions)
        {
            do makeAssertions();
            while (TestShouldKeepRunning());

            Console.WriteLine($"Stress test complete after {Stopwatch.Elapsed} and {iterations} iterations");

            if (Stopwatch.Elapsed.TotalSeconds > timeLimitInSeconds + 1)
                Assert.Fail("Something took way too long");
        }
    }
}