using AssistantBot.Interfaces;
using AssistantBot.Tests.IntegrationTests.MockHelpers;
using AssistantBot.Types.Dtos;
using Bogus;
using BurningSunCalc.Types;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net;
using System.Text.Json;
using TgBotAbstractions;

namespace IntegrationTests;

public class BurningSunCalcFlowTests
{
    private readonly Faker _faker = new();
    private readonly SecretTokenProviderMockHelper _secretTokenProviderMock = new();
    private readonly ITgBotClient _tgBotClient = Substitute.For<ITgBotClient>();
    private readonly long _chatId;

    public BurningSunCalcFlowTests()
    {
        _chatId = _faker.Random.Long(1, int.MaxValue);
    }

    [Fact]
    public async Task ProcessesFullBurningSunFlowCorrectly()
    {
        using var factory = new CustomWebAppliucationFactory<Program>()
            .WithWebHostBuilder(b => b.ConfigureTestServices(s =>
            {
                _secretTokenProviderMock.ReplaceTokenProviderDependency(s);

                var tgBotClientDescriptor = s.Single(d => d.ServiceType == typeof(ITgBotClient));
                s.Remove(tgBotClientDescriptor);
                s.AddSingleton(_tgBotClient);
            }));
        var httpClient = factory.CreateClient();
        httpClient.DefaultRequestHeaders.Add(Constants.TelegramBotSecretKeyHeader, _secretTokenProviderMock.SecretTokenRnd);

        var responseMessage = await httpClient.PostAsync("/bot/update", new StringContent(GetNewStringUpdate("/in_days")));
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);

        responseMessage = await httpClient.PostAsync("/bot/update", 
            new StringContent(GetNewCoordinatesUpdate(new Coordinates
            {
                Latitude = 3,
                Longitude = 4
            })));
        Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        
        await _tgBotClient.Received(1).RequestCoordinates(Arg.Is<long>(ci => ci == _chatId), Arg.Any<string>());
        await _tgBotClient.Received(1).SendTextMessageAsync(Arg.Is<long>(ci => ci == _chatId), Arg.Any<string>());
    }

    private string GetNewStringUpdate(string updateMessage)
    {
        var newUpdate = new UpdateDto()
        {
            Id = _faker.Random.Int(),
            Message = new MessageDto()
            {
                Chat = new ChatDto()
                {
                    Id = _chatId
                },
                Text = updateMessage,
                From = new FromDto()
                {
                    Id = 332344563,
                    IsBot = _faker.Random.Bool(),
                    FirstName = _faker.Name.FirstName(),
                    LastName = _faker.Name.LastName(),
                    Username = _faker.Random.String2(4, 14)
                }
            }
        };

        var result = JsonSerializer.Serialize(newUpdate);
        return result;
    }

    private string GetNewCoordinatesUpdate(Coordinates coordinates)
    {
        var newUpdate = new UpdateDto()
        {
            Id = _faker.Random.Int(),
            Message = new MessageDto()
            {
                Chat = new ChatDto()
                {
                    Id = _chatId
                },
                From = new FromDto()
                {
                    Id = 332344563,
                    IsBot = _faker.Random.Bool(),
                    FirstName = _faker.Name.FirstName(),
                    LastName = _faker.Name.LastName(),
                    Username = _faker.Random.String2(4, 14)
                },
                Location = new LocationDto()
                {
                    Latitude = coordinates.Latitude,
                    Longitude = coordinates.Longitude
                }
            }
        };

        var result = JsonSerializer.Serialize(newUpdate);
        return result;
    }
}