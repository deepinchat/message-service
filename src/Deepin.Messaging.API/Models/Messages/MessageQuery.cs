using Deepin.Infrastructure.Pagination;

namespace DeepIn.Messaging.API.Models.Messages;

public class MessageQuery : PageQuery
{
    public string ChatId { get; set; }
    public string Keywords { get; set; }
    public string UserId { get; set; }
    public long AnchorSequence { get; set; }
    public MessageQueryDirection Direction { get; set; }
}
public enum MessageQueryDirection
{
    Backward,
    Forward
}