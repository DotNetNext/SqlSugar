using System;

namespace CacheTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SugarCache cache = new SugarCache();
            cache.Add("a", "1");

            var x = cache.Get<string>("a");
            cache.Add("a2", "11",5);
            var x2 = cache.Get<string>("a2");
            var isa= cache.ContainsKey<string>("a2");
            var allKeys = cache.GetAllKey<string>();
            var testr=cache.GetOrCreate<string>("a33",()=> { return "aaa"; },10);
            cache.Remove<string>("aaaaaaaa");
            cache.Remove<string>("a");
            Console.WriteLine("Hello World!");
        }
    }
}
