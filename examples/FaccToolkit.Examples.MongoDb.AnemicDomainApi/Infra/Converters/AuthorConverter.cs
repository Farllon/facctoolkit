using FaccToolkit.Examples.AnemicDomain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Converters
{
    public class AuthorConverter : JsonConverter<Author>
    {
        public override Author Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            Guid id = Guid.Empty;
            string name = string.Empty;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new Author(id, name);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString()!;
                    reader.Read();

                    switch (propertyName)
                    {
                        case nameof(Author.Id):
                            id = reader.GetGuid();
                            break;
                        case nameof(Author.Name):
                            name = reader.GetString()!;
                            break;
                        default:
                            throw new JsonException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Author value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(Author.Id), value.Id);
            writer.WriteString(nameof(Author.Name), value.Name);
            writer.WriteEndObject();
        }
    }
}
