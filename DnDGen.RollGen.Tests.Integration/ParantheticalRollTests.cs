using Ninject;
using NUnit.Framework;

namespace RollGen.Tests.Integration.Common
{
    [TestFixture]
    public class ParantheticalRollTests : IntegrationTests
    {
        [Inject]
        public Dice Dice { get; set; }

        [TestCase("1d2", 1, 2)]
        [TestCase("(1d2)", 1, 2)]
        [TestCase("((1d2))", 1, 2)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("(1d2)+(3d4)", 4, 14)]
        [TestCase("(1d2)d3", 1, 6)]
        [TestCase("1d(2d3)", 1, 6)]
        [TestCase("1d2+1", 2, 3)]
        [TestCase("(1d2+1)", 2, 3)]
        [TestCase("(1d2)+1", 2, 3)]
        [TestCase("(1d2+1)+1", 3, 4)]
        [TestCase("((1d2+1))", 2, 3)]
        [TestCase("((1d2+1)+1)", 3, 4)]
        [TestCase("((1d2+1))+1", 3, 4)]
        [TestCase("((1d2+1)+1)+1", 4, 5)]
        [TestCase("((1d2)+1)", 2, 3)]
        [TestCase("((1d2)+1)+1", 3, 4)]
        [TestCase("((1d2))+1", 2, 3)]
        [TestCase("(1d2+1)+(3d4+1)", 6, 16)]
        [TestCase("(1d2+1)+(3d4)+1", 6, 16)]
        [TestCase("(1d2+1)+(3d4+1)+1", 7, 17)]
        [TestCase("(1d2+1)+1+(3d4+1)", 7, 17)]
        [TestCase("(1d2+1)+1+(3d4)+1", 7, 17)]
        [TestCase("(1d2+1)+1+(3d4+1)+1", 8, 18)]
        [TestCase("(1d2)+1+(3d4+1)", 6, 16)]
        [TestCase("(1d2)+1+(3d4)+1", 6, 16)]
        [TestCase("(1d2)+1+(3d4+1)+1", 7, 17)]
        [TestCase("(1d2+1)d3", 2, 9)]
        [TestCase("1d(2d3+1)", 1, 7)]
        [TestCase("6d5d4k3d2k1", 1, 2)]
        [TestCase("6d5d4k(3d2k1)", 1, 8)]
        [TestCase("6d(5d4k3)d2k1", 1, 2)]
        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("7d6k5", 5, 30)]
        [TestCase("7d8!", 7, 560)]
        [TestCase("1-2+3(4)", 11, 11)]
        [TestCase("1-2+3(4d5)", 11, 59)]
        [TestCase("(1)(2)(3)", 6, 6)]
        [TestCase("(1d2+3)(6d5k4)(7d8!)", 112, 56000)]
        [TestCase("(3)d(2)k(1)", 1, 2)]
        [TestCase("(9d8!k7)d(6d4!)!k(3d2k1)", 1, 4800)]
        public void ParantheticalQuantity(string quantity, int lower, int upper)
        {
            var sum = Dice.Roll(quantity).AsSum();
            var min = Dice.Roll(quantity).AsPotentialMinimum();
            var max = Dice.Roll(quantity).AsPotentialMaximum();

            Assert.That(min, Is.EqualTo(lower));
            Assert.That(max, Is.EqualTo(upper));
            Assert.That(sum, Is.InRange(lower, upper));
        }

        [TestCase("1d2", 1, 2)]
        [TestCase("(1d2)", 1, 2)]
        [TestCase("((1d2))", 1, 2)]
        [TestCase("1d2+3", 1, 5)]
        [TestCase("1d2+3d4", 1, 14)]
        [TestCase("(1d2)+(3d4)", 1, 14)]
        [TestCase("(1d2)d3", 1, 6)]
        [TestCase("1d(2d3)", 1, 6)]
        [TestCase("1d2+1", 1, 3)]
        [TestCase("(1d2+1)", 1, 3)]
        [TestCase("(1d2)+1", 1, 3)]
        [TestCase("(1d2+1)+1", 1, 4)]
        [TestCase("((1d2+1))", 1, 3)]
        [TestCase("((1d2+1)+1)", 1, 4)]
        [TestCase("((1d2+1))+1", 1, 4)]
        [TestCase("((1d2+1)+1)+1", 1, 5)]
        [TestCase("((1d2)+1)", 1, 3)]
        [TestCase("((1d2)+1)+1", 1, 4)]
        [TestCase("((1d2))+1", 1, 3)]
        [TestCase("(1d2+1)+(3d4+1)", 1, 16)]
        [TestCase("(1d2+1)+(3d4)+1", 1, 16)]
        [TestCase("(1d2+1)+(3d4+1)+1", 1, 17)]
        [TestCase("(1d2+1)+1+(3d4+1)", 1, 17)]
        [TestCase("(1d2+1)+1+(3d4)+1", 1, 17)]
        [TestCase("(1d2+1)+1+(3d4+1)+1", 1, 18)]
        [TestCase("(1d2)+1+(3d4+1)", 1, 16)]
        [TestCase("(1d2)+1+(3d4)+1", 1, 16)]
        [TestCase("(1d2)+1+(3d4+1)+1", 1, 17)]
        [TestCase("(1d2+1)d3", 1, 9)]
        [TestCase("1d(2d3+1)", 1, 7)]
        [TestCase("6d5d4k3d2k1", 1, 2)]
        [TestCase("6d5d4k(3d2k1)", 1, 8)]
        [TestCase("6d(5d4k3)d2k1", 1, 2)]
        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 1, 5)]
        [TestCase("1d2+3d4", 1, 14)]
        [TestCase("7d6k5", 1, 30)]
        [TestCase("7d8!", 1, 560)]
        [TestCase("1-2+3(4)", 1, 11)]
        [TestCase("1-2+3(4d5)", 1, 59)]
        [TestCase("(1)(2)(3)", 1, 6)]
        [TestCase("(1d2+3)(6d5k4)(7d8!)", 1, 56000)]
        [TestCase("(3)d(2)k(1)", 1, 2)]
        [TestCase("(9d8!k7)d(6d4!)!k(3d2k1)", 1, 4800)]
        public void ParantheticalDie(string quantity, int lower, int upper)
        {
            var sum = Dice.Roll().d(quantity).AsSum();
            var min = Dice.Roll().d(quantity).AsPotentialMinimum();
            var max = Dice.Roll().d(quantity).AsPotentialMaximum();

            Assert.That(min, Is.EqualTo(lower));
            Assert.That(max, Is.EqualTo(upper));
            Assert.That(sum, Is.InRange(lower, upper));
        }

        [TestCase("1d2", 1, 4)]
        [TestCase("(1d2)", 1, 4)]
        [TestCase("((1d2))", 1, 4)]
        [TestCase("1d2+3", 4, 10)]
        [TestCase("1d2+3d4", 4, 28)]
        [TestCase("(1d2)+(3d4)", 4, 28)]
        [TestCase("(1d2)d3", 1, 12)]
        [TestCase("1d(2d3)", 1, 12)]
        [TestCase("1d2+1", 2, 6)]
        [TestCase("(1d2+1)", 2, 6)]
        [TestCase("(1d2)+1", 2, 6)]
        [TestCase("(1d2+1)+1", 3, 8)]
        [TestCase("((1d2+1))", 2, 6)]
        [TestCase("((1d2+1)+1)", 3, 8)]
        [TestCase("((1d2+1))+1", 3, 8)]
        [TestCase("((1d2+1)+1)+1", 4, 10)]
        [TestCase("((1d2)+1)", 2, 6)]
        [TestCase("((1d2)+1)+1", 3, 8)]
        [TestCase("((1d2))+1", 2, 6)]
        [TestCase("(1d2+1)+(3d4+1)", 6, 32)]
        [TestCase("(1d2+1)+(3d4)+1", 6, 32)]
        [TestCase("(1d2+1)+(3d4+1)+1", 7, 34)]
        [TestCase("(1d2+1)+1+(3d4+1)", 7, 34)]
        [TestCase("(1d2+1)+1+(3d4)+1", 7, 34)]
        [TestCase("(1d2+1)+1+(3d4+1)+1", 8, 36)]
        [TestCase("(1d2)+1+(3d4+1)", 6, 32)]
        [TestCase("(1d2)+1+(3d4)+1", 6, 32)]
        [TestCase("(1d2)+1+(3d4+1)+1", 7, 34)]
        [TestCase("(1d2+1)d3", 2, 18)]
        [TestCase("1d(2d3+1)", 1, 14)]
        [TestCase("6d5d4k3d2k1", 1, 4)]
        [TestCase("6d5d4k(3d2k1)", 1, 16)]
        [TestCase("6d(5d4k3)d2k1", 1, 4)]
        [TestCase("1+2-(3*4/5)%6", 1, 2)]
        [TestCase("1d2+3", 4, 10)]
        [TestCase("1d2+3d4", 4, 28)]
        [TestCase("7d6k5", 5, 60)]
        [TestCase("7d8!", 7, 1120)]
        [TestCase("1-2+3(4)", 11, 22)]
        [TestCase("1-2+3(4d5)", 11, 118)]
        [TestCase("(1)(2)(3)", 6, 12)]
        [TestCase("(1d2+3)(6d5k4)(7d8!)", 112, 112000)]
        [TestCase("(3)d(2)k(1)", 1, 4)]
        [TestCase("(9d8!k7)d(6d4!)!k(3d2k1)", 1, 9600)]
        public void ParantheticalKeep(string keep, int lower, int upper)
        {
            var sum = Dice.Roll(100_000).d2().Keeping(keep).AsSum();
            var min = Dice.Roll(100_000).d2().Keeping(keep).AsPotentialMinimum();
            var max = Dice.Roll(100_000).d2().Keeping(keep).AsPotentialMaximum();

            Assert.That(min, Is.EqualTo(lower));
            Assert.That(max, Is.EqualTo(upper));
            Assert.That(sum, Is.InRange(lower, upper));
        }

        [Test]
        public void ParantheticalQuantityAndDie()
        {
            var sum = Dice.Roll("1d2").d("3d4").AsSum();
            var min = Dice.Roll("1d2").d("3d4").AsPotentialMinimum();
            var max = Dice.Roll("1d2").d("3d4").AsPotentialMaximum();

            Assert.That(min, Is.EqualTo(1));
            Assert.That(max, Is.EqualTo(24));
            Assert.That(sum, Is.InRange(1, 24));
        }

        [Test]
        public void ParantheticalQuantityAndDieAndKeep()
        {
            var sum = Dice.Roll("5d6").d("3d4").Keeping("1d2").AsSum();
            var min = Dice.Roll("5d6").d("3d4").Keeping("1d2").AsPotentialMinimum();
            var max = Dice.Roll("5d6").d("3d4").Keeping("1d2").AsPotentialMaximum();

            Assert.That(min, Is.EqualTo(1));
            Assert.That(max, Is.EqualTo(24));
            Assert.That(sum, Is.InRange(1, 24));
        }
    }
}
