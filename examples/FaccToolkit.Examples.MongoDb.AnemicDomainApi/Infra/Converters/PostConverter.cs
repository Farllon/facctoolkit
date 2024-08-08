using FaccToolkit.Examples.AnemicDomain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Infra.Converters
{
    public class PostConverter : JsonConverter<Post>
    {
        public override Post Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            Guid id = Guid.Empty;
            string content = string.Empty;
            string title = string.Empty;
            Guid authorId = Guid.Empty;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new Post(id, title, content, authorId);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString()!;
                    reader.Read();

                    switch (propertyName)
                    {
                        case nameof(Post.Id):
                            id = reader.GetGuid();
                            break;
                        case nameof(Post.Title):
                            title = reader.GetString()!;
                            break;
                        case nameof(Post.Content):
                            content = reader.GetString()!;
                            break;
                        case nameof(Post.AuthorId):
                            authorId = reader.GetGuid();
                            break;
                        default:
                            throw new JsonException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Post value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(Post.Id), value.Id);
            writer.WriteString(nameof(Post.Title), value.Title);
            writer.WriteString(nameof(Post.Content), value.Content);
            writer.WriteString(nameof(Post.AuthorId), value.AuthorId);
            writer.WriteEndObject();
        }
    }
}
