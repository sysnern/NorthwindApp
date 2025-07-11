using Microsoft.Extensions.Caching.Memory;
using NorthwindApp.Business.Services.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NorthwindApp.Business.Services.Abstract
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null);
        void Remove(string key);
        void RemoveByPrefix(string prefix);
    }
}


