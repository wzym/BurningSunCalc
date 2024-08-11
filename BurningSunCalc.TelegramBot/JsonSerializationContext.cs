using System.Text.Json.Serialization;

namespace BurningSunCalc.TelegramBot;

[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class JsonSourceGenSerializerContext : JsonSerializerContext;