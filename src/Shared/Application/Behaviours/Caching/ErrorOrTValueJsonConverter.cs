namespace Application.Behaviours.Caching;

/// <summary>
/// Custom json deserialiser for ErrorOr since there is no setter or init on 'Value' and factory methods must be used.
/// </summary>
public class ErrorOrTValueJsonConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(ErrorOr<>))
        {
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(
        Type type,
        JsonSerializerOptions options)
    {
        Type valueType = type.GetGenericArguments()[0];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(ErrorOrConverterInner<>).MakeGenericType(valueType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: [],
            culture: null)!;

        return converter;
    }

    private class ErrorOrConverterInner<TValue>() :
        JsonConverter<ErrorOr<TValue>>
    {
        private const string IsErrorPropertyName = "IsError";
        private const string ErrorsPropertyName = "Errors";
        private const string ValuePropertyName = "Value";

        public override ErrorOr<TValue> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty(IsErrorPropertyName, out JsonElement isErrorElement) &&
                isErrorElement.GetBoolean())
            {
                var t = root.GetProperty(ErrorsPropertyName).GetRawText();

                // Deserialise as error
                List<Error> errors =
                    JsonSerializer.Deserialize<List<Error>>(root.GetProperty(ErrorsPropertyName).GetRawText(), options)!;
                return ErrorOr<TValue>.From(errors);
            }
            else
            {
                // Deserialise as value.
                TValue value =
                    JsonSerializer.Deserialize<TValue>(root.GetProperty(ValuePropertyName).GetRawText(), options)!;
                return ErrorOrFactory.From(value);
            }
        }

        /// <inheritdoc/>
        public override void Write(
            Utf8JsonWriter writer,
            ErrorOr<TValue> value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteBoolean(IsErrorPropertyName, value.IsError);

            if (value.IsError)
            {
                writer.WritePropertyName(ErrorsPropertyName);
                JsonSerializer.Serialize(writer, value.Errors, options);
            }
            else
            {
                writer.WritePropertyName(ValuePropertyName);
                JsonSerializer.Serialize(writer, value.Value, options);
            }

            writer.WriteEndObject();
        }
    }
}
