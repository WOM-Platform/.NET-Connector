using System;
using Newtonsoft.Json;
using WomPlatform.Connector.Models;

namespace WomPlatform.Connector {

    class IdentifierConverter : JsonConverter<Identifier> {

        public override Identifier ReadJson(JsonReader reader, Type objectType, Identifier existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if(reader.TokenType == JsonToken.Integer) {
                return new Identifier((long)reader.Value);
            }
            else if(reader.TokenType == JsonToken.String) {
                return new Identifier((string)reader.Value);
            }
            else {
                throw new ArgumentException("Expected numeric or string value for voucher ID");
            }
        }

        public override void WriteJson(JsonWriter writer, Identifier value, JsonSerializer serializer) {
            writer.WriteValue(value.Id);
        }

    }

}
