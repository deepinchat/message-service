using Deepin.Messaging.API.Domain;
using Deepin.Messaging.API.Domain.Enums;
using DeepIn.Messaging.API.Domain;

namespace DeepIn.Messaging.API.Models.Messages;

public class MessageDto
{
    public string Id { get; set; }
    public MessageType Type { get; set; }
    public string ChatId { get; set; }
    public string UserId { get; set; }
    public string Text { get; set; }
    public string ParentId { get; set; }
    public string ReplyToId { get; set; }
    public long Sequence { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }
    public bool IsPinned { get; set; }
    public IEnumerable<MessageAttachment> Attachments { get; set; } = [];
    public IEnumerable<MessageReaction> Reactions { get; set; } = [];
    public IEnumerable<MessageMention> Mentions { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public MessageDto(Message message)
    {
        this.Id = message.Id.ToString();
        this.UserId = message.UserId;
        this.Text = message.Text;
        this.ChatId = message.ChatId;
        this.ReplyToId = message.ReplyToId;
        this.ParentId = message.ParentId;
        this.Type = message.Type;
        this.Sequence = message.Sequence;
        this.IsEdited = message.IsEdited;
        this.IsDeleted = message.IsDeleted;
        this.IsRead = message.IsRead;
        this.IsPinned = message.IsPinned;
        this.Attachments = message.Attachments;
        this.Mentions = message.Mentions;
        this.Reactions = message.Reactions;
        this.CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(message.CreatedAt).UtcDateTime;
        if (message.ModifiedAt != 0)
            this.ModifiedAt = DateTimeOffset.FromUnixTimeMilliseconds(message.ModifiedAt).UtcDateTime;
    }
}
