using Deepin.Domain;
using DeepIn.Messaging.API.Models.Messages;
using DeepIn.Messaging.API.Services; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace DeepIn.Messaging.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IUserContext _userContext;
    public MessagesController(
        IMessageService messageService,
        IUserContext userContext)
    {
        _messageService = messageService;
        _userContext = userContext;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _messageService.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] MessageQuery query)
    {
        var list = await _messageService.GetMessages(query.Offset, query.Limit, query.ChatId, query.From, query.Keywords);
        return Ok(list);
    }
    [HttpGet("last")]
    public async Task<IActionResult> GetLastMessage(string chatId)
    {
        var result = await _messageService.GetLastMessage(chatId);
        return Ok(result);
    }
    [HttpGet("batch")]
    public async Task<IActionResult> GetMessages([FromQuery] string[] ids)
    {
        var result = await _messageService.GetMessages(ids);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MessageRequest request)
    {
        var result = await _messageService.InsertAsync(request, _userContext.UserId);
        return Ok(new MessageDto(result));
    }

}
