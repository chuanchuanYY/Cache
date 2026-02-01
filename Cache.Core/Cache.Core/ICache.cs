using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache.Core;

public interface ICache<T>
{
    void Add(string key, T value);

    void Add(string key, T value, TimeSpan ttl);
    T Get(string key);
}
