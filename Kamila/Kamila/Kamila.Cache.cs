using System;
using System.Runtime.Caching;

namespace Kamila {
    public static class Cache {
        private const string NameCache = "Kamila";
        private static readonly MemoryCache fCache;
        private static readonly CacheItemPolicy fPolicy;
        static Cache () {
            fCache = new MemoryCache ( NameCache );
            fPolicy = new CacheItemPolicy () {
                AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration,
                SlidingExpiration = new TimeSpan ( 0, 15, 0 )
            };
        }
        public static object Get ( string Key ) {
            return fCache.Get ( Key );
        }
        public static void Add ( string Key, object Value ) {
            fCache.Add ( Key, Value, fPolicy );
        }
        public static void Add ( string Key, object Value, DateTimeOffset AbsoluteExpiration ) {
            fCache.Add ( Key, Value, new CacheItemPolicy () {
                AbsoluteExpiration = AbsoluteExpiration,
                SlidingExpiration = MemoryCache.NoSlidingExpiration
            } );
        }
        public static void Add ( string Key, object Value, TimeSpan SlidingExpiration ) {
            fCache.Add ( Key, Value, new CacheItemPolicy () {
                AbsoluteExpiration = MemoryCache.InfiniteAbsoluteExpiration,
                SlidingExpiration = SlidingExpiration
            } );
        }
    }
}
