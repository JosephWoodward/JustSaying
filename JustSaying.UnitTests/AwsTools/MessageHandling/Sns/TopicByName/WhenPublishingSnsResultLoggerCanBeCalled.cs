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
    public class WhenPublishingSnsResultLoggerCanBeCalled : XAsyncBehaviourTest<SnsTopicByName>
    {
        private readonly IMessageSerialisationRegister _serialisationRegister = Substitute.For<IMessageSerialisationRegister>();
        private readonly IAmazonSimpleNotificationService _sns = Substitute.For<IAmazonSimpleNotificationService>();
        private const string TopicArn = "topicarn";
        private const string MessageId = "12345";

        private static MessageResult _result;

        private readonly MessageResultLogger _messageResultLogger = r =>
        {
            _result = r;
        };

        protected override SnsTopicByName CreateSystemUnderTest()
        {
            var topic = new SnsTopicByName("TopicName", _sns, _serialisationRegister, _messageResultLogger, Substitute.For<ILoggerFactory>(), Substitute.For<SnsWriteConfiguration>());

            topic.Exists();
            return topic;
        }

        protected override void Given()
        {
            _sns.FindTopicAsync("TopicName")
                .Returns(new Topic { TopicArn = TopicArn });
            _sns.PublishAsync(Arg.Any<PublishRequest>())
                .Returns(PublishResult);
        }

        protected override Task When()
        {
            SystemUnderTest.PublishAsync(new GenericMessage()).Wait();

            return Task.CompletedTask;
        }

        private static Task<PublishResponse> PublishResult(CallInfo arg)
        {
            return Task.FromResult(new PublishResponse
            {
                MessageId = MessageId,
                HttpStatusCode = HttpStatusCode.OK
            });
        }

        [Fact]
        public void ResponseHandlerHasBeenInvoked()
        {
            _result.ResponseMessageId.ShouldBe(MessageId);
            _result.ResponseHttpStatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
