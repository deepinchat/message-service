using Deepin.Messaging.API.Domain;
using Deepin.Messaging.API.Domain.Enums;

namespace DeepIn.Messaging.API.Models.Messages;

public class MessageRequest
{
    public MessageType Type { get; set; }
    public string ChatId { get; set; }
    public string ParentId { get; set; }
    public string ReplyToId { get; set; }
    public string Text { get; set; }
    public IEnumerable<MessageAttachment> Attachments { get; set; } = [];
    public IEnumerable<MessageMention> Mentions { get; set; } = [];
}
