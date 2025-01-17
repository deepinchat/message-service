using Deepin.Application.Pagination;
using Deepin.EventBus.Events;
using DeepIn.Messaging.API.Domain;
using DeepIn.Messaging.API.Models.Messages;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeepIn.Messaging.API.Services;

public class MessageService(IBus bus, IMongoRepository<Message> messageRepository) : IMessageService
{
    private readonly IBus _bus = bus;
    private readonly IMongoRepository<Message> _messageRepository = messageRepository;
    public async Task<Message> InsertAsync(MessageRequest request, string userId, DateTimeOffset? createdAt = null)
    {
        var message = await _messageRepository.InsertAsync(new Message()
        {
            ChatId = request.ChatId.ToString(),
            Content = request.Content,
            CreatedAt = createdAt.HasValue ? createdAt.Value.ToUnixTimeMilliseconds() : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            From = userId,
            ReplyTo = request.ReplyTo
        });
        await _bus.Publish(new SendChatMessageIntegrationEvent(request.ChatId.ToString(), message.Id.ToString()));
        return message;
    }

    public async Task<MessageDto?> GetByIdAsync(string id)

    {
        var message = await _messageRepository.FindByIdAsync(id);
        if (message == null) return null;
        return new MessageDto(message);
    }

    public IFindFluent<Message, Message> Query(string? chatId = null, string? from = null, string? keywords = null)
    {
        var filterBuilder = Builders<Message>.Filter;
        var filters = new List<FilterDefinition<Message>>();
        if (!string.IsNullOrEmpty(chatId))
        {
            filters.Add(filterBuilder.Eq(d => d.ChatId, chatId));
        }
        if (!string.IsNullOrEmpty(from))
        {
            filters.Add(filterBuilder.Eq(d => d.From, from));
        }
        if (!string.IsNullOrEmpty(keywords))
        {
            filters.Add(filterBuilder.Regex(d => d.Content, new BsonRegularExpression(keywords, "i")));
        }
        var filter = filterBuilder.And(filters);

        var query = _messageRepository.Collection.Find(filter);
        return query;
    }
    public async Task<IPagination<MessageDto>> GetMessages(int offset = 0, int limit = 20, string? chatId = null, string? from = null, string? keywords = null)
    {
        var query = Query(chatId, from, keywords);
        var totalCount = await query.CountDocumentsAsync();
        var documents = await query.SortByDescending(s => s.CreatedAt).Skip(offset).Limit(limit).ToListAsync();

        return new Pagination<MessageDto>(documents.Select(e => new MessageDto(e)), offset, limit, (int)totalCount);
    }
}
