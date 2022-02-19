using Newtonsoft.Json;
using System;

namespace PpCalculatorTypes
{

    public class ColorHexConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color4)value;
            writer.WriteValue(color.ToHex());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color4);
        }
    }
}
