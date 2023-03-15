using DnDGen.Stress;
using NUnit.Framework;
using System.Reflection;

namespace DnDGen.RollGen.Tests.Integration.Stress
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
#if STRESS
                IsFullStress = true,
#else
                IsFullStress = false,
#endif
            };

            //INFO: Non-stress operations can take up to 12 minutes, or 20% of the 60 minute runtime.
            options.TimeLimitPercentage = .80;

            stressor = new Stressor(options);
        }
    }
}