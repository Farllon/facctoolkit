using FaccToolkit.Examples.RichDomain.Aggregations.Authors;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Data.Serializers
{
    public class TitleSerializer : SerializerBase<Title>
    {
        public static readonly TitleSerializer Instance = new TitleSerializer();
        
        public override Title Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => Title.Create(context.Reader.ReadString());

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Title value)
            => context.Writer.WriteString(value.Value);
    }
}
