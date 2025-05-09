﻿using Deepin.EventBus.Events;
using Deepin.Infrastructure.Pagination;
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
            CreatedAt = createdAt.HasValue ? createdAt.Value.ToUnixTimeMilliseconds() : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UserId = userId,
            Sequence = sequence,
            Attachments = request.Attachments,
            Mentions = request.Mentions,
            ParentId = request.ParentId,
            ReplyToId = request.ReplyToId,
            Type = request.Type,
            Text = request.Text
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

    public IFindFluent<Message, Message> Query(string chatId = null, string userId = null, string keywords = null, MessageQueryDirection direction = MessageQueryDirection.Backward, long anchorSqeuence = 0)
    {
        var filterBuilder = Builders<Message>.Filter;
        var filters = new List<FilterDefinition<Message>>();
        if (!string.IsNullOrEmpty(chatId))
        {
            filters.Add(filterBuilder.Eq(d => d.ChatId, chatId));
        }
        if (!string.IsNullOrEmpty(userId))
        {
            filters.Add(filterBuilder.Eq(d => d.UserId, userId));
        }
        if (!string.IsNullOrEmpty(keywords))
        {
            filters.Add(filterBuilder.Regex(d => d.Text, new BsonRegularExpression(keywords, "i")));
        }
        if (anchorSqeuence > 0)
        {
            if (direction == MessageQueryDirection.Backward)
            {
                filters.Add(filterBuilder.Lte(d => d.Sequence, anchorSqeuence));
            }
            else
            {
                filters.Add(filterBuilder.Gte(d => d.Sequence, anchorSqeuence));
            }
        }
        var filter = filterBuilder.And(filters);
        var query = _messageRepository.Collection.Find(filter);
        return query;
    }
    public async Task<IPagination<MessageDto>> GetMessages(int offset = 0, int limit = 20, string chatId = null, string from = null, string keywords = null, MessageQueryDirection direction = MessageQueryDirection.Backward, long anchorSqeuence = 0)
    {
        var query = Query(chatId, from, keywords, direction, anchorSqeuence);
        var totalCount = await query.CountDocumentsAsync();
        List<Message> messages = [];
        if (direction == MessageQueryDirection.Backward)
        {
            messages = await query.SortByDescending(s => s.Sequence).Skip(offset).Limit(limit).ToListAsync();
        }
        else
        {
            messages = await query.SortBy(s => s.Sequence).Skip(offset).Limit(limit).ToListAsync();
            messages.Reverse();
        }
        return new Pagination<MessageDto>(messages.Select(e => new MessageDto(e)), offset, limit, (int)totalCount);
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
