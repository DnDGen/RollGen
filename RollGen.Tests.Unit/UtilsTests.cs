using NUnit.Framework;
using System;

namespace RollGen.Tests.Unit
{
    [TestFixture]
    class UtilsTests
    {
        // Work around NUnit not supporting Generics, by using this function.
        object GenericTester(string function_name, Type T, object[] parameters)
            => typeof(Utils).GetMethod(function_name).MakeGenericMethod(T).Invoke(null, parameters);

        [TestCase(typeof(double), 8)]
        [TestCase(typeof(int), true)]
        [TestCase(typeof(string), 8)]
        public void ChangeType(Type T, object arg)
        {
            var result = GenericTester("ChangeType", T, new[] { arg });
            Assert.That(result, Is.TypeOf(T));
        }

        [TestCase(typeof(string), "true", "true")]
        [TestCase(typeof(double), true, "True")]
        [TestCase(typeof(string), false, "False")]
        [TestCase(typeof(float), 0.8f, "0.8")]
        [TestCase(typeof(int), 0.8f, "1")]
        [TestCase(typeof(int), 0.3f, "0")]
        public void BooleanOrType(Type T, object arg, string expected)
        {
            var result = GenericTester("BooleanOrType", T, new[] { arg });
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
