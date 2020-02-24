using System;
using Newtonsoft.Json;
using WomPlatform.Connector.Models;

namespace WomPlatform.Connector {

    class VoucherIdConverter : JsonConverter<VoucherId> {

        public override VoucherId ReadJson(JsonReader reader, Type objectType, VoucherId existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if(reader.TokenType == JsonToken.Integer) {
                return new VoucherId((long)reader.Value);
            }
            else if(reader.TokenType == JsonToken.String) {
                return new VoucherId((string)reader.Value);
            }
            else {
                throw new ArgumentException("Expected numeric or string value for voucher ID");
            }
        }

        public override void WriteJson(JsonWriter writer, VoucherId value, JsonSerializer serializer) {
            writer.WriteValue(value.Id);
        }

    }

}
