using Addicted.Core;
using NUnit.Framework;

namespace Addicted.Tests
{
    public class SeededRandomTests
    {
        [Test]
        public void MesmaSeed_GeraMesmaSequencia()
        {
            var a = new SeededRandom(1234);
            var b = new SeededRandom(1234);

            for (var i = 0; i < 100; i++)
                Assert.AreEqual(a.Range(0, 1000), b.Range(0, 1000));
        }

        [Test]
        public void Range_RespeitaLimites()
        {
            var rng = new SeededRandom(42);

            for (var i = 0; i < 1000; i++)
            {
                var value = rng.Range(-5f, 5f);
                Assert.That(value, Is.InRange(-5f, 5f));
            }
        }
    }
}
