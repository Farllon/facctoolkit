using FaccToolkit.Examples.RichDomain.Aggregations.Authors;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace FaccToolkit.Examples.MongoDb.RichDomainApi.Infra.Data.Serializers
{
    public class NameSerializer : SerializerBase<Name>
    {
        public static readonly NameSerializer Instance = new NameSerializer();

        public override Name Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            => Name.Create(context.Reader.ReadString());

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Name value)
            => context.Writer.WriteString(value.Value);
    }
}
