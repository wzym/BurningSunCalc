using AssistantBot.Interfaces;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AssistantBot.Tests.IntegrationTests.MockHelpers;

internal class SecretTokenProviderMockHelper
{
    private readonly Faker _faker = new();

    private ITgBotSecretTokenProvider MockInstance { get; }

    internal string SecretTokenRnd { get; }

    internal SecretTokenProviderMockHelper()
    {
        SecretTokenRnd = _faker.Random.String2(3, 20);
        MockInstance = Substitute.For<ITgBotSecretTokenProvider>();
        MockInstance.Get.Returns(SecretTokenRnd);
    }

    internal void ReplaceTokenProviderDependency(IServiceCollection serviceDescriptors)
    {
        var tgBotSecretTokenProviderDescriptor = serviceDescriptors
            .Single(d => d.ServiceType == typeof(ITgBotSecretTokenProvider));
        serviceDescriptors.Remove(tgBotSecretTokenProviderDescriptor);
        serviceDescriptors.AddSingleton(MockInstance);
    }
}