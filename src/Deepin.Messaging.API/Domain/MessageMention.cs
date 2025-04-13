using Deepin.Messaging.API.Domain.Enums;

namespace Deepin.Messaging.API.Domain;

public class MessageMention
{
    public MentionType Type { get; set; }
    public string UserId { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
}
