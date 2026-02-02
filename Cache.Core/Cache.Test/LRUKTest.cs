using Cache.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache.Test;

internal class LRUKTest
{

    [Test]
    public void TestK1Add()
    {
        var lru = new LRUKCache<int>(1, 2);
        lru.Add("one", 1);
        lru.Add("two", 2);
        Assert.That(lru.Get("one"), Is.EqualTo(1));
        Assert.That(lru.Get("two"), Is.EqualTo(2));
    }


    [Test]
    public void TestK2Add()
    {
        var lru = new LRUKCache<int>(2, 2);
        lru.Add("one", 1);
        lru.Add("two", 2);
        Assert.That(lru.Get("one"), Is.EqualTo(1));
    }


    [Test]
    public void TestK3Add()
    {
        var lru = new LRUKCache<int>(3,10);
        lru.Add("one", 1);
        lru.Add("two", 2);
        lru.Add("three", 3);
       
        Assert.Throws<InvalidOperationException>(() => lru.Get("one"));
        Assert.That(lru.Get("one"), Is.EqualTo(1));
    }
}
