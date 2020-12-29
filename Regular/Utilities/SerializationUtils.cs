﻿using Newtonsoft.Json;
using Regular.Models;

namespace Regular.Utilities
{
    public static class SerializationUtils
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };
        public static string SerializeRegexRule(RegexRule regexRule)
        {
            return JsonConvert.SerializeObject(regexRule, JsonSerializerSettings);
        }

        public static RegexRule DeserializeRegexRule(string serializedRegexRule)
        {
            return JsonConvert.DeserializeObject<RegexRule>(serializedRegexRule, JsonSerializerSettings);
        }
    }
}