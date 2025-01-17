using Deepin.Application.Pagination;
using DeepIn.Messaging.API.Domain;
using DeepIn.Messaging.API.Models.Messages;
using MongoDB.Driver;

namespace DeepIn.Messaging.API.Services;

public interface IMessageService
{
    Task<IPagination<MessageDto>> GetMessages(int offset = 0, int limit = 20, string? chatId = null, string? from = null, string? keywords = null);
    Task<Message> InsertAsync(MessageRequest request, string userId, DateTimeOffset? createdAt = null);
    Task<MessageDto?> GetByIdAsync(string id);
    IFindFluent<Message, Message> Query(string? chatId = null, string? from = null, string? keywords = null);
}