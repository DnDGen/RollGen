using Ninject;
using NUnit.Framework;
using RollGen.Tests.Integration.Common;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class StressTests : IntegrationTests
    {
        [Inject]
        public Stopwatch Stopwatch { get; set; }

        private const int ConfidentIterations = 1000000;
        private const int TenMinutesInSeconds = 600;
        private const int TwoHoursInSeconds = 3600 * 2;

        private readonly int timeLimitInSeconds;

        private int iterations;

        public StressTests()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var methods = types.SelectMany(t => t.GetMethods());
            var stressTestsCount = methods.Sum(m => m.GetCustomAttributes<TestAttribute>(true).Count());
            var stressTestCasesCount = methods.Sum(m => m.GetCustomAttributes<TestCaseAttribute>().Count());
            var stressTestsTotal = stressTestsCount + stressTestCasesCount;

            var twoHourTimeLimitPerTest = TwoHoursInSeconds / stressTestsTotal;
#if STRESS
            timeLimitInSeconds = Math.Min(twoHourTimeLimitPerTest, TenMinutesInSeconds - 10);
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
            return iterations < ConfidentIterations && Stopwatch.Elapsed.TotalSeconds < timeLimitInSeconds;
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