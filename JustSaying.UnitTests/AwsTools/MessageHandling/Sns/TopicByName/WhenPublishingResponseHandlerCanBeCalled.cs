using System.Net;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using JustBehave;
using JustSaying.AwsTools.MessageHandling;
using JustSaying.AwsTools.QueueCreation;
using JustSaying.Messaging.MessageSerialisation;
using JustSaying.TestingFramework;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
using Shouldly;
using Xunit;

namespace JustSaying.UnitTests.AwsTools.MessageHandling.Sns.TopicByName
{
    public class WhenPublishingResponseHandlerCanBeCalled : XAsyncBehaviourTest<SnsTopicByName>
    {
        private readonly IMessageSerialisationRegister _serialisationRegister = Substitute.For<IMessageSerialisationRegister>();
        private readonly IAmazonSimpleNotificationService _sns = Substitute.For<IAmazonSimpleNotificationService>();
        private const string TopicArn = "topicarn";
        private const string MessageId = "12345";

        private static MessageResponse response;

        private readonly MessageResponseHandler _messageResponseHandler = r =>
        {
            response = r;
        };

        protected override SnsTopicByName CreateSystemUnderTest()
        {
            var topic = new SnsTopicByName("TopicName", _sns, _serialisationRegister, _messageResponseHandler, Substitute.For<ILoggerFactory>(), Substitute.For<SnsWriteConfiguration>());

            topic.Exists();
            return topic;
        }

        protected override void Given()
        {
            _sns.FindTopicAsync("TopicName")
                .Returns(new Topic { TopicArn = TopicArn });
        }

        protected override Task When()
        {
            _sns.PublishAsync(Arg.Any<PublishRequest>()).Returns(Result);
            return Task.CompletedTask;
        }

        private static Task<PublishResponse> Result(CallInfo arg)
        {
            return Task.FromResult(new PublishResponse
            {
                MessageId = MessageId,
                HttpStatusCode = HttpStatusCode.OK
            });
        }

        [Fact]
        public void FailSilently()
        {
            SystemUnderTest.PublishAsync(new GenericMessage()).Wait();
            response.ResponseMessageId.ShouldBe(MessageId);
            response.ResponseHttpStatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
