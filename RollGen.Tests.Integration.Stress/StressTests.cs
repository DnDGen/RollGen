using Ninject;
using NUnit.Framework;
using RollGen.Tests.Integration.Common;
using System;
using System.Collections.Generic;
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

        private readonly TimeSpan timeLimit;

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
            var timeLimitInSeconds = Math.Min(twoHourTimeLimitPerTest, TenMinutesInSeconds - 10);
#else
            var timeLimitInSeconds = 1;
#endif

            timeLimit = new TimeSpan(0, 0, timeLimitInSeconds);
        }

        [SetUp]
        public void StressTestSetup()
        {
            iterations = 0;

            Log($"Stress test time limit is {timeLimit}");

            Stopwatch.Start();
        }

        protected void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now}: {message}");
        }

        [TearDown]
        public void StressTestTearDown()
        {
            Stopwatch.Reset();
        }

        private bool TestShouldKeepRunning()
        {
            iterations++;
            return iterations < ConfidentIterations && Stopwatch.Elapsed < timeLimit;
        }

        protected void Stress(Action makeAssertions)
        {
            do makeAssertions();
            while (TestShouldKeepRunning());

            Log($"Stress test complete after {Stopwatch.Elapsed} and {iterations} iterations");

            if (Stopwatch.Elapsed.TotalSeconds > timeLimit.TotalSeconds + 1)
                Assert.Fail("Something took way too long");
        }

        protected IEnumerable<int> Populate(ICollection<int> target, Func<int> generate, int expectedTotal)
        {
            do
            {
                var generatedNumber = generate();
                target.Add(generatedNumber);
            }
            while (TestShouldKeepRunning() && target.Count < expectedTotal);

            Log($"Population complete after {Stopwatch.Elapsed} and {iterations} iterations");

            if (TestShouldKeepRunning() == false && target.Count < expectedTotal)
            {
                var message = $"Stress test timed out after {Stopwatch.Elapsed}, {iterations} iterations, and {target.Count} of {expectedTotal} populated:";
                message += $"\n{string.Join(", ", target.OrderBy(x => x))}";
                Assert.Fail(message);
            }

            return target;
        }
    }
}