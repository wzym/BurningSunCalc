using AssistantBot.Interfaces;
using AssistantBot.Tests.IntegrationTests.MockHelpers;
using AssistantBot.Types;
using Bogus;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Text.Json;
using AssistantBot.Logic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IntegrationTests;

public class AspInfrastructureTests
{
    private readonly Faker _faker = new();
    private readonly SecretTokenProviderMockHelper _secretTokenProviderMock = new();

    [Fact]
    public async Task CorrectlyMapsChatIdFromUpdateToUpdateModel()
    {
        var chatId = _faker.Random.Long(1, int.MaxValue);
        var messageText = _faker.Random.String2(3, 100);
        var tgUpdateHandlerMock = Substitute.For<ITgUpdateHandler>();

        await using var factory = new CustomWebAppliucationFactory<Program>()
            .WithWebHostBuilder(b => b.ConfigureTestServices(s => 
            {
                Replace<ITgUpdateHandler, ITgUpdateHandler>(s, tgUpdateHandlerMock);
                _secretTokenProviderMock.ReplaceTokenProviderDependency(s);
            }));
        var httpClient = factory.CreateClient();

        var request = new Update()
        {
            Message = new Message()
            {
                Chat = new Chat()
                {
                    Id = chatId,
                    Type = _faker.PickRandom<ChatType>()
                },
                Text = "/in_days " + messageText
            }
        };
        httpClient.DefaultRequestHeaders.Add(Constants.TelegramBotSecretKeyHeader, _secretTokenProviderMock.SecretTokenRnd);
        var response = await httpClient.PostAsync("bot/update", new StringContent(JsonSerializer.Serialize(request)));

        await tgUpdateHandlerMock.Handle(Arg.Is<UpdateModel>(um => um.ChatId == chatId));
    }

    [Fact]
    public async Task CorrectlyMapsCommandFromUpdateToUpdateModel()
    {
        var messageText = "/in_days";
        var tgUpdateHandlerMock = Substitute.For<ITgUpdateHandler>();

        using var factory = new CustomWebAppliucationFactory<Program>()
            .WithWebHostBuilder(b => b.ConfigureTestServices(s =>
            {
                Replace<ITgUpdateHandler, ITgUpdateHandler>(s, tgUpdateHandlerMock);
                _secretTokenProviderMock.ReplaceTokenProviderDependency(s);
            }));
        var httpClient = factory.CreateClient();

        var request = new Update()
        {
            Message = new Message()
            {
                Chat = new Chat()
                {
                    Id = _faker.Random.Long(1, int.MaxValue),
                    Type = _faker.PickRandom<ChatType>()
                },
                Text = messageText
            }
        };
        httpClient.DefaultRequestHeaders.Add(Constants.TelegramBotSecretKeyHeader, _secretTokenProviderMock.SecretTokenRnd);
        var response = await httpClient.PostAsync("bot/update", new StringContent(JsonSerializer.Serialize(request)));

        await tgUpdateHandlerMock.Handle(Arg.Is<UpdateModel>(um => um.IsCommand && um.Command == AssistantBotCommand.InDays));
    }

    private static void Replace<TReplaced, TMock>(IServiceCollection s, TMock mockType)
        where TMock : class, TReplaced
    {
        var founcDependencyDescrpiptor = s.Single(d => d.ServiceType == typeof(TReplaced));
        s.Remove(founcDependencyDescrpiptor);
        s.AddTransient(_ => mockType);
    }
}