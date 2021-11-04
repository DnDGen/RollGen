using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration
{
    [TestFixture]
    public class RollTests : IntegrationTests
    {
        private Dice dice;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
        }

        [TestCase("1d2", 1, 2)]
        [TestCase("1d2+1", 2, 3)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("1d2+9", 10, 11)]
        [TestCase("2d10", 2, 20)]
        [TestCase("3d6", 3, 18)]
        [TestCase("4d5!t1t2k3", 9, 150)]
        [TestCase("4d6k3", 3, 18)]
        [TestCase("6d5d4k3d2k1", 1, 2)]
        [TestCase("7d6k5", 5, 30)]
        [TestCase("7d8", 7, 56)]
        [TestCase("7d8!", 7, 560)]
        //From README
        [TestCase("4d6", 4, 24)]
        [TestCase("92d66", 92, 92 * 66)]
        [TestCase("5+3d4*2", 11, 29)]
        [TestCase("((1d2)d5k1)d6", 1, 30)]
        [TestCase("4d6k3", 3, 18)]
        [TestCase("3d4k2", 2, 8)]
        [TestCase("5+3d4*3", 14, 41)]
        [TestCase("1d6+3", 4, 9)]
        [TestCase("1d8+1d2-1", 1, 9)]
        [TestCase("4d3-3", 1, 9)]
        [TestCase("4d6!", 4, 240)]
        [TestCase("3d4!", 3, 120)]
        [TestCase("3d4!k2", 2, 80)]
        [TestCase("3d6t1", 6, 18)]
        [TestCase("3d6t1t5", 6, 18)]
        [TestCase("3d6!t1k2", 4, 120)]
        [TestCase("4d3t2k1", 1, 3)]
        [TestCase("4d3k1t2", 1, 3)]
        [TestCase("4d3!t2k1", 1, 30)]
        [TestCase("4d3!k1t2", 1, 30)]
        [TestCase("4d3t2!k1", 1, 30)]
        [TestCase("4d3k1!t2", 1, 30)]
        [TestCase("4d3t2k1!", 1, 30)]
        [TestCase("4d3k1t2!", 1, 30)]
        public void RollRange(string rollExpression, int lower, int upper)
        {
            var roll = dice.Roll(rollExpression);
            var sum = roll.AsSum();
            var min = roll.AsPotentialMinimum();
            var max = roll.AsPotentialMaximum();

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
        [TestCase("7d6k5", 5, 60)]
        [TestCase("7d8!", 7, 1120)]
        [TestCase("1-2+3(4)", 11, 22)]
        [TestCase("1-2+3(4d5)", 11, 118)]
        [TestCase("(1)(2)(3)", 6, 12)]
        [TestCase("(1d2+3)(6d5k4)+(7d8!)+(11d10t9)", 34, 1540)]
        [TestCase("(3)d(2)k(1)", 1, 4)]
        [TestCase("(9d8!k7)d(6d4!)!k(3d2k1)", 1, 9600)]
        public void ParantheticalQuantity(string quantity, int lower, int upper)
        {
            var roll = dice.Roll(quantity).d2();
            var sum = roll.AsSum();
            var min = roll.AsPotentialMinimum();
            var max = roll.AsPotentialMaximum();

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
        [TestCase("7d6k5", 1, 30)]
        [TestCase("7d8!", 1, 560)]
        [TestCase("1-2+3(4)", 1, 11)]
        [TestCase("1-2+3(4d5)", 1, 59)]
        [TestCase("(1)(2)(3)", 1, 6)]
        [TestCase("(1d2!+3)(6d5k4)", 1, 460)]
        [TestCase("(3)d(2)k(1)", 1, 2)]
        [TestCase("(9d8!k7)d(6d4!)!k(3d2k1)", 1, 4800)]
        public void ParantheticalDie(string quantity, int lower, int upper)
        {
            var roll = dice.Roll().d(quantity);
            var sum = roll.AsSum();
            var min = roll.AsPotentialMinimum();
            var max = roll.AsPotentialMaximum();

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
        [TestCase("7d6k5", 5, 60)]
        [TestCase("7d8!", 7, 1120)]
        [TestCase("1-2+3(4)", 11, 22)]
        [TestCase("1-2+3(4d5)", 11, 118)]
        [TestCase("(1)(2)(3)", 6, 12)]
        [TestCase("(1d2!+3)(6d5k4)", 16, 920)]
        [TestCase("(3)d(2)k(1)", 1, 4)]
        [TestCase("(9d8!k7)d(6d4!)!k(3d2k1)", 1, 9600)]
        public void ParantheticalKeep(string keep, int lower, int upper)
        {
            var roll = dice.Roll(Limits.Quantity).d2().Keeping(keep);
            var sum = roll.AsSum();
            var min = roll.AsPotentialMinimum();
            var max = roll.AsPotentialMaximum();

            Assert.That(min, Is.EqualTo(lower));
            Assert.That(max, Is.EqualTo(upper));
            Assert.That(sum, Is.InRange(lower, upper));
        }

        [TestCase("1d2", 20_000, 1_000_000)]
        [TestCase("(1d2)", 20_000, 1_000_000)]
        [TestCase("((1d2))", 20_000, 1_000_000)]
        [TestCase("1d2+3", 10_000, 1_000_000)]
        [TestCase("1d2+3d4", 10_000, 1_000_000)]
        [TestCase("(1d2)+(3d4)", 10_000, 1_000_000)]
        [TestCase("(1d2)d3", 20_000, 1_000_000)]
        [TestCase("1d(2d3)", 20_000, 1_000_000)]
        [TestCase("1d2+1", 10_000, 1_000_000)]
        [TestCase("(1d2+1)", 10_000, 1_000_000)]
        [TestCase("(1d2)+1", 10_000, 1_000_000)]
        [TestCase("(1d2+1)+1", 10_000, 1_000_000)]
        [TestCase("((1d2+1))", 10_000, 1_000_000)]
        [TestCase("((1d2+1)+1)", 10_000, 1_000_000)]
        [TestCase("((1d2+1))+1", 10_000, 1_000_000)]
        [TestCase("((1d2+1)+1)+1", 10_000, 1_000_000)]
        [TestCase("((1d2)+1)", 10_000, 1_000_000)]
        [TestCase("((1d2)+1)+1", 10_000, 1_000_000)]
        [TestCase("((1d2))+1", 10_000, 1_000_000)]
        [TestCase("(1d2+1)+(3d4+1)", 10_000, 1_000_000)]
        [TestCase("(1d2+1)+(3d4)+1", 10_000, 1_000_000)]
        [TestCase("(1d2+1)+(3d4+1)+1", 10_000, 1_000_000)]
        [TestCase("(1d2+1)+1+(3d4+1)", 10_000, 1_000_000)]
        [TestCase("(1d2+1)+1+(3d4)+1", 10_000, 1_000_000)]
        [TestCase("(1d2+1)+1+(3d4+1)+1", 10_000, 1_000_000)]
        [TestCase("(1d2)+1+(3d4+1)", 10_000, 1_000_000)]
        [TestCase("(1d2)+1+(3d4)+1", 10_000, 1_000_000)]
        [TestCase("(1d2)+1+(3d4+1)+1", 10_000, 1_000_000)]
        [TestCase("(1d2+1)d3", 10_000, 1_000_000)]
        [TestCase("1d(2d3+1)", 20_000, 1_000_000)]
        [TestCase("6d5d4k3d2k1", 20_000, 1_000_000)]
        [TestCase("6d5d4k(3d2k1)", 20_000, 1_000_000)]
        [TestCase("6d(5d4k3)d2k1", 20_000, 1_000_000)]
        [TestCase("1+2-(3*4/5)%6", 20_000, 1_000_000)]
        [TestCase("7d6k5", 10_000, 1_000_000)]
        [TestCase("1d3!", 20_000, 1_000_000)]
        [TestCase("2d3!", 10_000, 1_000_000)]
        [TestCase("1-2+3(4)", 10_000, 1_000_000)]
        [TestCase("1-2+3(4d5)", 10_000, 1_000_000)]
        [TestCase("(1)(2)(3)", 10_000, 1_000_000)]
        [TestCase("(1d2!+3)+(6d5k4)", 10_000, 1_000_000)]
        [TestCase("(3)d(2)k(1)", 20_000, 1_000_000)]
        public void ParantheticalTransform(string transform, int lower, int upper)
        {
            var roll = dice.Roll(Limits.Quantity).Percentile().Transforming(transform);
            var sum = roll.AsSum();
            var min = roll.AsPotentialMinimum();
            var max = roll.AsPotentialMaximum();

            Assert.That(min, Is.EqualTo(lower), "Min");
            Assert.That(max, Is.EqualTo(upper), "Max");
            Assert.That(sum, Is.InRange(lower, upper));
        }

        [Test]
        public void AllParantheticalExpressions()
        {
            var roll = dice.Roll("7d8").d("3d4").Transforming("1d2").Keeping("5d6");
            var sum = roll.AsSum();
            var min = roll.AsPotentialMinimum();
            var max = roll.AsPotentialMaximum();

            Assert.That(min, Is.EqualTo(10));
            Assert.That(max, Is.EqualTo(360));
            Assert.That(sum, Is.InRange(10, 360));
        }
    }
}
