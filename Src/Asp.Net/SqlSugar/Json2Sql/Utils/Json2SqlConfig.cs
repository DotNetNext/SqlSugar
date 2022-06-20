using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    public static class JsonProviderConfig
    {
        public const string KeyInsertable = "Insertable";
        public const string KeyUpdateable = "Updateable";
        public const string KeyQueryable = "Queryable";
        public const string KeyDeleteable = "Deleteable";

        private static Dictionary<string, string> words = new Dictionary<string, string>()
        {
            { KeyInsertable,"Table"},
            { KeyUpdateable,"Table"},
            { KeyQueryable,"Table"},
            { KeyDeleteable,"Table"}
        };
        public static string Rename(string key,string name)
        {
            return words[key]=name;
        }
        internal static string Get(this string value)
        {
            return words[value];
        }
        internal static string GetWord(string key) 
        {
            Check.ExceptionEasy(words.ContainsKey(key) == false, $"{key} is error", $"{key} 不存在 ");
            return words[key];
        }
    }
}
