using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class RangeTests
    {
        [Test]
        public void RangeTest()
        {
            var actual = new List<int>();
            var result = Range(1, 5);
            result.Subscribe(actual.Add);
            Assert.That(actual.Count == 5);
        }

        IObservable<int> Range(int start, int end)
        {
            var max = start + end;
            return Observable.Generate(start,
                val => val < max,
                val => val + 1,
                v => v);
        }
    }
    
}

