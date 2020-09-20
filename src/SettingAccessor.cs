using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace VolumeShortcut
{
    using KeyCombination = System.Tuple<int, bool, bool, bool>;
    using KeyCombinationValue = System.ValueTuple<int, bool, bool, bool>;

    public class KeyCombinationConverter : JsonConverter<KeyCombination>
    {
        private readonly JsonEncodedText Item1Name = JsonEncodedText.Encode("Item1");
        private readonly JsonEncodedText Item2Name = JsonEncodedText.Encode("Item2");
        private readonly JsonEncodedText Item3Name = JsonEncodedText.Encode("Item3");
        private readonly JsonEncodedText Item4Name = JsonEncodedText.Encode("Item4");

        private readonly JsonConverter<int> intConverter;
        private readonly JsonConverter<bool> boolConverter;

        public KeyCombinationConverter(JsonSerializerOptions options)
        {
            if (options?.GetConverter(typeof(int)) is JsonConverter<int> intConverter)
            {
                this.intConverter = intConverter;
            }
            else
            {
                throw new InvalidOperationException();
            }
            
            if (options?.GetConverter(typeof(bool)) is JsonConverter<bool> boolConverter)
            {
                this.boolConverter = boolConverter;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override KeyCombination Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var value = new KeyCombinationValue();
            for (reader.Read(); reader.TokenType == JsonTokenType.PropertyName; reader.Read())
            {
                if (reader.ValueTextEquals(Item1Name.EncodedUtf8Bytes))
                {
                    value.Item1 = ReadIntProperty(ref reader, options);
                }
                else if (reader.ValueTextEquals(Item2Name.EncodedUtf8Bytes))
                {
                    value.Item2 = ReadBoolProperty(ref reader, options);
                }
                else if (reader.ValueTextEquals(Item3Name.EncodedUtf8Bytes))
                {
                    value.Item3 = ReadBoolProperty(ref reader, options);
                }
                else if (reader.ValueTextEquals(Item4Name.EncodedUtf8Bytes))
                {
                    value.Item4 = ReadBoolProperty(ref reader, options);
                }
                else
                {
                    throw new JsonException();
                }
            }

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return value.ToTuple();
        }

        private int ReadIntProperty(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            reader.Read();
            return intConverter.Read(ref reader, typeof(int), options);
        }

        private bool ReadBoolProperty(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            reader.Read();
            return boolConverter.Read(ref reader, typeof(bool), options);
        }

        // Writeは標準のものを使用し, 本メソッドは使用しない
        public override void Write(Utf8JsonWriter writer, KeyCombination value, JsonSerializerOptions options)
        {
             writer.WriteStartObject();
             writer.WriteEndObject();
        }
    }

    internal static class SettingAccessor
    {
        internal static (KeyCombinationValue up, KeyCombinationValue down) ReadSetting(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return (new KeyCombinationValue(), new KeyCombinationValue());
            }

            var readBytes = File.ReadAllBytes(filePath);
            var reader = new Utf8JsonReader(readBytes);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new KeyCombinationConverter(options));

            var readDict = JsonSerializer.Deserialize<Dictionary<string, KeyCombination>>(ref reader, options);

            return (readDict["VolumeUp"].ToValueTuple(), readDict["VolumeDown"].ToValueTuple());
        }

        internal static void WriteSetting(string filePath, in KeyCombinationValue up, in KeyCombinationValue down)
        {
            var data = JsonSerializer.SerializeToUtf8Bytes
            (
                new Dictionary<string, KeyCombination>()
                {
                    { "VolumeUp", up.ToTuple() },
                    { "VolumeDown", down.ToTuple() },
                }
            );
            File.WriteAllBytes(filePath, data);
        }
    }
}