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

            //INFO: Non-stress operations take roughly 4.5 minutes, or 7.5% of the 60 minute runtime. Rounding up to 10% for padding.
            options.TimeLimitPercentage = .90;

            stressor = new Stressor(options);
        }
    }
}