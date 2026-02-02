using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache.Core;

public class LRUKCache<TValue> : ICache<TValue>
{
    private readonly int _k;
    private LRUCache<int> _history;
    private Dictionary<string, TValue> _historyKeyToValue = [];
    private LRUCache<TValue> _cache;
    public LRUKCache(int k,int capacity)
    {
        _k = k;
        _history = new LRUCache<int>(capacity);
        _cache  = new LRUCache<TValue>(capacity);
    }
    public void Add(string key, TValue value)
    {
        Add(key,value,TimeSpan.MaxValue);
    }

    public void Add(string key, TValue value, TimeSpan ttl)
    {
        
        try
        {
            var history = _history.Get(key);
            _history.Add(key, history + 1);
            if(history + 1 >= _k)
            {
                _cache.Add(key, value, ttl);
                _historyKeyToValue.Remove(key);
                _history.Add(key, _k);
            }
        }
        catch(InvalidOperationException)
        {
            _history.Add(key, 1);
            _historyKeyToValue.Add(key,value);
        }
        
    }

    public TValue Get(string key)
    {
        if(TryGetValue(key,out TValue value,_cache))
        {
            return value;
        }

        if(TryGetValue(key,out int history,_history))
        {
            _history.Add(key, history + 1);
            if (history + 1 >= _k)
            {
                if (_historyKeyToValue.ContainsKey(key))
                {
                    _cache.Add(key, _historyKeyToValue[key]);
                    _historyKeyToValue.Remove(key);
                    _history.Add(key, _k);
                    return _cache.Get(key);
                }
                return _cache.Get(key);
            }
        }
        else
        {
            _history.Add(key, 1);
        }
     
        throw new InvalidOperationException();
    }


    private bool TryGetValue<T>(string key,out T value,ICache<T> cache)
    {
        value = default!;

        try
        {
            value = cache.Get(key);
            return true;
        }
        catch (InvalidOperationException)
        {
            
            return false;
        }
    }
}
