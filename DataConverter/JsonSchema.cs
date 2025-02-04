﻿using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace DataConverter
{
    public class JsonSchema
    {

        private readonly JsonElement _raw;

        public Dictionary<string, JsonSchema> Object { get; }
        public List<JsonSchema> Array { get; }
        public string json;

        //constructor JsonSchema
        public JsonSchema(JsonElement raw)
        {

            switch (raw.ValueKind)
            {
                case JsonValueKind.Object:
                    Object = raw.EnumerateObject()
                        .ToDictionary(kvp => kvp.Name, kvp => new JsonSchema(kvp.Value));
                    break;
                case JsonValueKind.Array:
                    Array = raw.EnumerateArray()
                        .Select(v => new JsonSchema(v))
                        .ToList();
                    break;
                case JsonValueKind.Undefined:
                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    _raw = raw.Clone();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //jsonschema constructor from string
        public static JsonSchema Build(JsonElement element) => new JsonSchema(element);


        //build json element from string json
        public static JsonSchema Build(string json)
        {
            JsonDocument doc = JsonDocument.Parse(json);

            return new JsonSchema(doc.RootElement.Clone());
        }


        //convert to element
        public JsonElement ToElement()
        {
            using var doc = JsonDocument.Parse(ToString());
            return doc.RootElement.Clone();
        }

        //output to string 
        public override string ToString()
        {
            if (Object != null)
                return $"{{{string.Join(",", Object.Select(p => $"{JsonSerializer.Serialize(p.Key)}:{p.Value}"))}}}";

            if (Array != null)
                return $"[{string.Join(",", Array)}]";

            return _raw.ValueKind switch
            {
                JsonValueKind.String => _raw.GetRawText(),
                JsonValueKind.Number => _raw.ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => "null",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        //shortcut to json element deserialization
        public JsonElement ToJsonElement()
        {
            using var doc = JsonDocument.Parse(ToString());
            return doc.RootElement.Clone();
        }
       
    }


    /* JSON ELEMENT GENERATOR CLASS */
    public static class JsonElementGenerator
    {
        /* JSON ELEMENT FROM INT,DOUBLE,STRING,BOOL */


        public static JsonElement AsJsonElement(this bool value)
        {
            using var doc = JsonDocument.Parse(value ? "true" : "false");
            return doc.RootElement.Clone();
        }

        public static JsonElement AsJsonElement(this int value)
        {
            using var doc = JsonDocument.Parse(value.ToString(CultureInfo.InvariantCulture));
            return doc.RootElement.Clone();
        }

        public static JsonElement AsJsonElement(this double value)
        {
            using var doc = JsonDocument.Parse(value.ToString(CultureInfo.InvariantCulture));
            return doc.RootElement.Clone();
        }

        public static JsonElement AsJsonElement(this decimal value)
        {
            using var doc = JsonDocument.Parse(value.ToString(CultureInfo.InvariantCulture));
            return doc.RootElement.Clone();
        }

        public static JsonElement AsJsonElement(this string? value)
        {
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(value));
            return doc.RootElement.Clone();
        }
    }
}

