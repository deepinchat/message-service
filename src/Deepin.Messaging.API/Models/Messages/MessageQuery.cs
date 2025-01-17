using Deepin.Application.Pagination;

namespace DeepIn.Messaging.API.Models.Messages;

public class MessageQuery : PageQuery
{
    public string? Keywords { get; set; }
    public string? ChatId { get; set; }
    public string? From { get; set; }
}
