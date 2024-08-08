using FaccToolkit.Examples.RichDomain.Aggregations.Authors;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Data.Serializers
{
    public class ContentSerializer : SerializerBase<Content>
    {
        public static readonly ContentSerializer Instance = new ContentSerializer();

        public override Content Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => Content.Create(context.Reader.ReadString());

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Content value)
            => context.Writer.WriteString(value.Value);
    }
}
