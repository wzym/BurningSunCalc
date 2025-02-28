using AssistantBot.Api.AspHelpers;
using AssistantBot.Interfaces;
using AssistantBot.Logic.Services;
using AssistantBot.Types.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text;

namespace AssistantBot.Tests.UnitTests;

public class UpdateParsingTests
{
    private readonly IUpdateMessageParser<UpdateDto> _updateMessageParserMock;
    private readonly UpdateRequestMappingMiddleware _testedInstance;
    private readonly HttpContext _httpContextMock;
    private readonly UpdateModelHolder _updateModelHolder = new();

    private const string Sample1 = """
        {
        	"update_id": 136938069,
        	"message": {
        		"message_id": 403,
        		"from": {
        			"id": 332344563,
        			"is_bot": false,
        			"first_name": "Name",
        			"last_name": "Fam",
        			"username": "username",
        			"language_code": "ru",
        			"is_premium": true
        		},
        		"chat": {
        			"id": 332344563,
        			"first_name": "Name",
        			"last_name": "Fam",
        			"username": "username",
        			"type": "private"
        		},
        		"date": 1740220663,
        		"text": "/today",
        		"entities": [
        			{
        				"offset": 0,
        				"length": 6,
        				"type": "bot_command"
        			}
        		]
        	}
        }
        """;

    private const string Sampe2 = """
        {
        	"update_id": 136938249,
        	"callback_query": {
        		"id": "480177215498264440",
        		"from": {
        			"id": 332344563,
        			"is_bot": false,
        			"first_name": "Name",
        			"last_name": "Fam",
        			"username": "username",
        			"language_code": "ru",
        			"is_premium": true
        		},
        		"message": {
        			"message_id": 757,
        			"from": {
        				"id": 7715143184,
        				"is_bot": true,
        				"first_name": "wzymAlcoTrack",
        				"username": "WzymAlcoTrackBot"
        			},
        			"chat": {
        				"id": 332344563,
        				"first_name": "Name",
        				"last_name": "Fam",
        				"username": "username",
        				"type": "private"
        			},
        			"date": 1740745043,
        			"text": "Выберите минимальную мощность солнца, от которой начинается жгучесть",
        			"reply_markup": {
        				"inline_keyboard": [
        					[
        						{
        							"text": "60",
        							"callback_data": "60"
        						},
        						{
        							"text": "70",
        							"callback_data": "70"
        						},
        						{
        							"text": "80",
        							"callback_data": "80"
        						}
        					]
        				]
        			}
        		},
        		"chat_instance": "7077728298651111111",
        		"data": "70"
        	}
        }
        """;

    public UpdateParsingTests()
    {
        _updateMessageParserMock = Substitute.For<IUpdateMessageParser<UpdateDto>>();
        _testedInstance = new UpdateRequestMappingMiddleware(
            Substitute.For<ILogger<UpdateRequestMappingMiddleware>>(), _updateMessageParserMock,
            _updateModelHolder);

        _httpContextMock = new DefaultHttpContext();
    }

    [Fact]
    public async Task ParsesAMessageCorrectly()
    {
        PrepareRequestBody(Sample1);
        await _testedInstance.InvokeAsync(_httpContextMock, _ => Task.CompletedTask);

        _updateMessageParserMock.Parse(Arg.Is<UpdateDto>(ud => ud.Id == 332344563
            && ud.Message != null 
            && ud.Message.Text == "/today"));
    }

    [Fact]
    public async Task ParsesButtonResponseCorrectly()
    {
        PrepareRequestBody(Sampe2);
        await _testedInstance.InvokeAsync(_httpContextMock, _ => Task.CompletedTask);

        _updateMessageParserMock.Parse(Arg.Is<UpdateDto>(ud => ud.Id == 136938249 
            && ud.CallbackQuery != null
            && ud.CallbackQuery.Data == "70"));
    }

    private void PrepareRequestBody(string rawBody)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawBody));
        _httpContextMock.Request.Body = stream;
        _httpContextMock.Request.ContentLength = stream.Length;
    }
}