using DnDGen.Stress;
using NUnit.Framework;
using System.Reflection;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class StressTests : IntegrationTests
    {
        protected Stressor stressor;

        protected const int QuantityLimit = 10_000;
        protected const int DieLimit = 1_000_000;

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

            stressor = new Stressor(options);
        }
    }
}