using DeepIn.Messaging.API.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Deepin.Messaging.API.Domain;

public class MessageSequence: IDocument
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string ChatId { get; set; }
    public long Sequence { get; set; }
}
