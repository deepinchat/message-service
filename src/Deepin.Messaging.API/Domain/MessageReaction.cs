namespace Deepin.Messaging.API.Domain;

public class MessageReaction
{
    public string UserId { get; set; }
    public string Emoji { get; set; }
    public bool IsAdded { get; set; }
    public long CreatedAt { get; set; }
}
