using BurningSunCalc.Types;
using System.Text.Json.Serialization;

namespace BurningSunCalc.TelegramBot;

[JsonSerializable(typeof(BurningSunCalcOptions))]
internal partial class JsonSourceGenerationContext : JsonSerializerContext;