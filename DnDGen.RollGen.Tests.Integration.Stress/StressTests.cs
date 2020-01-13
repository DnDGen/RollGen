using DnDGen.Stress;
using NUnit.Framework;
using RollGen.Tests.Integration.Common;
using System.Reflection;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class StressTests : IntegrationTests
    {
        protected Stressor stressor;

        [OneTimeSetUp]
        public void StressSetup()
        {
            var options = new StressorOptions()
            {
                RunningAssembly = Assembly.GetExecutingAssembly(),
                TimeLimitPercentage = .95,
#if STRESS
                IsFullStress = true,
#else
                IsFullStress = false,
#endif
            };

            stressor = new Stressor(options);
        }
    }
}