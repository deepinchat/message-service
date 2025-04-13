using Deepin.Messaging.API.Domain;
using Deepin.Messaging.API.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeepIn.Messaging.API.Domain;

public class Message : IDocument
{
    [BsonId]
    public ObjectId Id { get; set; }
    public MessageType Type { get; set; }
    public string ChatId { get; set; }
    public string UserId { get; set; }
    public string Text { get; set; }
    public string ParentId { get; set; }
    public string ReplyToId { get; set; }
    public long CreatedAt { get; set; }
    public long ModifiedAt { get; set; }
    public long Sequence { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }
    public bool IsPinned { get; set; }
    public IEnumerable<MessageAttachment> Attachments { get; set; } = [];
    public IEnumerable<MessageReaction> Reactions { get; set; } = [];
    public IEnumerable<MessageMention> Mentions { get; set; } = [];
}
