using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache.Core;

public class LRUCache<TValue> : ICache<TValue>
{
    private readonly Node<TValue> _l = new(string.Empty,default!); 
    private readonly Node<TValue> _r = new(string.Empty,default!);

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

            Remove(node);
            Insert(key,node.Value);
            return node.Value;
        }
        else
            throw new InvalidOperationException("key not find!");
    }

    private void Insert(string key, TValue value)
    {
        var node = new Node<TValue>(key,value);

        var pre = _r.Pre;

        pre!.Next = node;
        node.Pre = pre;

        _r.Pre = node;
        node.Next = _r;

        _map.Add(key, node);
    }

    public void Add(string key, TValue value)
    {
        if(_map.ContainsKey(key))
        {
            var node = _map[key];

            Remove(node);
            Insert(key,value);
            return;
        }

       if(_map.Count >= _capacity)
       {
            var node = _l.Next!;
            Remove(node);
            Insert(key, value);
        }
       else
       {
            Insert(key, value);
        }
    }
}


internal class Node<TValue>
{

    public Node(string key,TValue value,Node<TValue>? pre = null, Node<TValue>? next = null)
    {
        Key = key;
        Value = value;
        Pre = pre;
        Next = next;
    }
    public string Key { get; set; } = string.Empty;
    public TValue Value { get; set; }
    public Node<TValue>? Pre { get; set; }
    public Node<TValue>? Next { get; set; }
}