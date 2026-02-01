using Cache.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache.Test;

internal class LRUTest
{

    [Test]
    public void TestAdd()
    {
        var lru = new LRUCache<int>(10);
        lru.Add("one", 1);

        Assert.That(lru.Get("one"), Is.EqualTo(1));
    }

    [Test]
    public void TestGet()
    {
        var lru = new LRUCache<string>(5);
        lru.Add("first", "hello");
        lru.Add("second", "world");
        Assert.That(lru.Get("first"), Is.EqualTo("hello"));
        Assert.That(lru.Get("second"), Is.EqualTo("world"));
    }

    [Test]
    public void TestCount()
    {
        var lru = new LRUCache<double>(3);
        lru.Add("a", 1.1);
        lru.Add("b", 2.2);
        lru.Add("c", 3.3);
        Assert.That(lru.Count, Is.EqualTo(3));
        lru.Add("d", 4.4); // This should evict the least recently used item
        Assert.That(lru.Count, Is.EqualTo(3));
    }


    [Test]
    public void TestEviction()
    {
        var lru = new LRUCache<int>(2);
        lru.Add("x", 10);
        lru.Add("y", 20);
        lru.Get("x"); // Access 'x' to make it recently used
        lru.Add("z", 30); // This should evict 'y'
        
        Assert.Throws<InvalidOperationException>(() => lru.Get("y"));
        Assert.That(lru.Get("x"), Is.EqualTo(10));
        Assert.That(lru.Get("z"), Is.EqualTo(30));
    }


    [Test]
    public void TestExpiration()
    {
        var lru = new LRUCache<string>(2);
        lru.Add("temp", "data", TimeSpan.FromMilliseconds(100));
        Assert.That(lru.Get("temp"), Is.EqualTo("data"));
        System.Threading.Thread.Sleep(200);
        Assert.Throws<InvalidOperationException>(() => lru.Get("temp"));
    }
}
