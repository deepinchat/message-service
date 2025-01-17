using DeepIn.Messaging.API.Domain;

namespace DeepIn.Messaging.API.Models.Messages;

public class MessageDto : MessageRequest
{
    public string Id { get; set; }
    public string From { get; set; }
    public long CreatedAt { get; set; }
    public long ModifiedAt { get; set; }
    public MessageDto() { }
    public MessageDto(Message message)
    {
        this.Id = message.Id.ToString();
        this.From = message.From;
        this.CreatedAt = message.CreatedAt;
        this.ModifiedAt = message.ModifiedAt;
        this.Content = message.Content;
        this.ChatId = message.ChatId;
        this.ReplyTo = message.ReplyTo;
    }
}
