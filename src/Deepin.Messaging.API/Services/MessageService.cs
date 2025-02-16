using Deepin.Application.Pagination;
using Deepin.EventBus.Events;
using Deepin.Messaging.API.Domain;
using DeepIn.Messaging.API.Domain;
using DeepIn.Messaging.API.Models.Messages;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeepIn.Messaging.API.Services;

public class MessageService(
    IBus bus,
    IMongoRepository<Message> messageRepository,
    IMongoRepository<MessageSequence> sequenceRepository) : IMessageService
{
    private readonly IBus _bus = bus;
    private readonly IMongoRepository<Message> _messageRepository = messageRepository;
    private readonly IMongoRepository<MessageSequence> _sequenceRepository = sequenceRepository;
    public async Task<long> GetNextSequenceValueAsync(string chatId)
    {
        var filter = Builders<MessageSequence>.Filter.Eq(d => d.ChatId, chatId);
        var update = Builders<MessageSequence>.Update.Inc(d => d.Sequence, 1);
        var options = new FindOneAndUpdateOptions<MessageSequence, MessageSequence>()
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = true
        };
        var message = await _sequenceRepository.Collection.FindOneAndUpdateAsync(filter, update, options);
        return message.Sequence;
    }
    public async Task<Message> InsertAsync(MessageRequest request, string userId, DateTimeOffset? createdAt = null)
    {
        var sequence = await GetNextSequenceValueAsync(request.ChatId);
        var message = await _messageRepository.InsertAsync(new Message()
        {
            ChatId = request.ChatId,
            Content = request.Content,
            CreatedAt = createdAt.HasValue ? createdAt.Value.ToUnixTimeMilliseconds() : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            From = userId,
            ReplyTo = request.ReplyTo,
            Sequence = sequence
        });
        await _bus.Publish(new SendChatMessageIntegrationEvent(request.ChatId.ToString(), message.Id.ToString()));
        return message;
    }

    public async Task<MessageDto> GetByIdAsync(string id)

    {
        var message = await _messageRepository.FindByIdAsync(id);
        if (message == null) return null;
        return new MessageDto(message);
    }

    public IFindFluent<Message, Message> Query(string chatId = null, string from = null, string keywords = null)
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
    public async Task<IPagination<MessageDto>> GetMessages(int offset = 0, int limit = 20, string chatId = null, string from = null, string keywords = null)
    {
        var query = Query(chatId, from, keywords);
        var totalCount = await query.CountDocumentsAsync();
        var documents = await query.SortByDescending(s => s.CreatedAt).Skip(offset).Limit(limit).ToListAsync();

        return new Pagination<MessageDto>(documents.Select(e => new MessageDto(e)), offset, limit, (int)totalCount);
    }

    public async Task<MessageDto> GetLastMessage(string chatId)
    {
        var query = Query(chatId);
        var message = await query.SortByDescending(s => s.CreatedAt).FirstOrDefaultAsync();
        return message == null ? null : new MessageDto(message);
    }

    public async Task<IEnumerable<MessageDto>> GetMessages(string[] ids)
    {
        var messages = await _messageRepository.Collection.Find(d => ids.Contains(d.Id.ToString())).ToListAsync();
        return messages?.Select(e => new MessageDto(e));
    }
}
