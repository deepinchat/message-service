using Deepin.Messaging.API.Domain.Enums;
using Newtonsoft.Json.Linq;

namespace Deepin.Messaging.API.Domain;

public class MessageAttachment
{
    public AttachmentType Type { get; set; }
    public string FileId { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string ThumbnailFileId { get; set; }
    public string MimeType { get; set; }
    public object Metadata { get; set; }
}