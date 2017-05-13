using NUnit.Framework;
using UnmanagedArray;
using System;
using System.Linq;
using System.Diagnostics;

namespace Tests
{
    [TestFixture]
    public class Main : Common
    {
        [Test]
        public void ZeroInit()
        {
            using (var arr = new Array<int>(1000))
            {
                Assert.IsTrue(arr.All(x => x == 0));
            }
        }

        [Test]
        public void VeryLargeAllocation([Values(1, 2, 4, 8)] long gigaBytes)
        {
            Require64();
            using(var arr = new Array<byte>(gigaBytes * GB))
            {
                arr[0] = 255;
                arr[arr.Length - 1] = 255;
            }
        }

        [Test]
        public void SetAndGet()
        {
            Func<long, long> f = i => i * i;
            using (var arr = new Array<long>(5, f))
            {
                for (var i = 0L; i < arr.Length; i++)
                {
                    Assert.AreEqual(f(i), arr[i]);
                }
            }
        }

        [Test]
        public void Enumerable()
        {
            using (var arr = new Array<int>(5, 15))
            {
                foreach (var e in arr)
                {
                    Assert.AreEqual(e, 15);
                }
            }
        }

        [Test]
        public void Linq()
        {
            using(var arr = new Array<long>(13, i => i * i))
            {
                Assert.AreEqual(0, arr.First());
                Assert.AreEqual(1, arr.Skip(1).First());
                Assert.AreEqual(144, arr.Last());
                Assert.AreEqual(144, arr.Reverse().First());
            }
        }

        [Test]
        [TestCase(5, 10)]
        [TestCase(4096, 8192)]
        [TestCase(1024 * 1024, 2L * 1024 * 1024 * 1024, false)]
        public void Resize(long initialLength, long newLength, bool verifyBeyondOldBoundary = true)
        {
            Require64();

            Func<long, long> f = i => i * i;
            using (var arr = new Array<long>(initialLength, f))
            {
                Assert.AreEqual(initialLength, arr.Length);

                arr.Resize(newLength);
                Assert.AreEqual(newLength, arr.Length);

                var end = verifyBeyondOldBoundary ? newLength : initialLength;
                for (var i = 0L; i < end; i++)
                {
                    if (i < initialLength)
                    {
                        Assert.AreEqual(f(i), arr[i]);
                    }
                    else
                    {
                        Assert.AreEqual(0, arr[i]);
                    }
                }
            }
        }

        [Test]
        public void BreakEnumerator()
        {
            using (var arr = new Array<long>(5, 1))
            {
                var sum = arr.Sum(x =>
                {
                    arr.Resize(arr.Length - 1);
                    return x;
                });
                Assert.AreEqual(3, sum);
            }
        }

        [Test]
        public void ClassArray()
        {
            using (var arr = new Array<string>(5, x => x.ToString()))
            {
                GC.AddMemoryPressure(4 * GB);
                GC.Collect();
                for (var i = 0L; i < arr.Length; i++)
                {
                    Assert.AreEqual(i.ToString(), arr[i]);
                }
                GC.AddMemoryPressure(4 * GB);
                GC.Collect();
                for (var i = 0L; i < arr.Length; i++)
                {
                    arr[i] = "";
                }
                GC.AddMemoryPressure(4 * GB);
                GC.Collect();
                for (var i = 0L; i < arr.Length; i++)
                {
                    Assert.AreEqual("", arr[i]);
                }
            }
        }


    }
}
