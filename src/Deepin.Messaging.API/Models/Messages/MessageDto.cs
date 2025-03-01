using DeepIn.Messaging.API.Domain;

namespace DeepIn.Messaging.API.Models.Messages;

public class MessageDto : MessageRequest
{
    public string Id { get; set; }
    public string From { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long Sequence { get; set; }
    public MessageDto(Message message)
    {
        this.Id = message.Id.ToString();
        this.From = message.From;
        this.CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(message.CreatedAt).UtcDateTime;
        if (message.ModifiedAt != 0)
            this.ModifiedAt = DateTimeOffset.FromUnixTimeMilliseconds(message.ModifiedAt).UtcDateTime;
        this.Content = message.Content;
        this.ChatId = message.ChatId;
        this.ReplyTo = message.ReplyTo;
        this.Sequence = message.Sequence;
        this.IsEdited = message.IsEdited;
        this.IsDeleted = message.IsDeleted;
        this.IsRead = message.IsRead;
    }
}
