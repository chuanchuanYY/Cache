using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache.Core;

public class LRUCache<TValue> : ICache<TValue>
{
    private readonly Node<TValue> _l = new(string.Empty,default!,DateTimeOffset.MinValue,TimeSpan.Zero); 
    private readonly Node<TValue> _r = new(string.Empty,default!,DateTimeOffset.MaxValue,TimeSpan.Zero);

    private readonly Dictionary<string,Node<TValue>> _map = new();  

    private  int _capacity = 100;

    public LRUCache(int capacity)
    {
        
        _capacity = capacity;
        _l.Next = _r;
        _r.Pre = _l;
    }

    public int Count => _map.Count;
    private void Remove(Node<TValue> node)
    {
        node.Pre!.Next = node.Next;
        node.Next!.Pre = node.Pre;

        _map.Remove(node.Key);
    }

    public TValue Get(string key)
    {
        if (_map.ContainsKey(key))
        {
            var node = _map[key];

            if(DateTimeOffset.Now >= node.ExpireTime)
            {
                Remove(node);
                throw new InvalidOperationException("key expired!");
            }

            Remove(node);
            Insert(key,node.Value,node.TTL);
            return node.Value;
        }
        else
            throw new InvalidOperationException("key not find!");
    }

    private void Insert(string key, TValue value,TimeSpan ttl)
    {
        var expireTime = ttl == TimeSpan.MaxValue ? DateTimeOffset.MaxValue : DateTimeOffset.Now.Add(ttl);
        var node = new Node<TValue>(key,value,expireTime,ttl);

        var pre = _r.Pre;

        pre!.Next = node;
        node.Pre = pre;

        _r.Pre = node;
        node.Next = _r;

        _map.Add(key, node);
    }

    public void Add(string key, TValue value)
    {
        Add(key,value,TimeSpan.MaxValue);
    }

    public void Add(string key, TValue value, TimeSpan ttl)
    {
        if (_map.ContainsKey(key))
        {
            var node = _map[key];

            Remove(node);
            Insert(key, value,ttl);
            return;
        }

        if (_map.Count >= _capacity)
        {
            var node = _l.Next!;
            Remove(node);
            Insert(key, value,ttl);
        }
        else
        {
            Insert(key, value,ttl);
        }
    }
}


internal class Node<TValue>
{

    public Node(string key,TValue value,DateTimeOffset expireTime,TimeSpan ttl,Node<TValue>? pre = null, Node<TValue>? next = null)
    {
        Key = key;
        Value = value;
        ExpireTime = expireTime;
        TTL = ttl;
        Pre = pre;
        Next = next;
    }
    public string Key { get; set; } = string.Empty;
    public TValue Value { get; set; }
    public DateTimeOffset ExpireTime { get; set; }
    public TimeSpan TTL { get; set; }
    public Node<TValue>? Pre { get; set; }
    public Node<TValue>? Next { get; set; }
}