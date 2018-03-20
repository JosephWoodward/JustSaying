using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
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
    public class WhenPublishingSqsResultLoggerCanBeCalled : XAsyncBehaviourTest<SqsPublisher>
    {
        private readonly IMessageSerialisationRegister _serialisationRegister = Substitute.For<IMessageSerialisationRegister>();
        private readonly IAmazonSQS _sqs = Substitute.For<IAmazonSQS>();
        private const string Url = "https://blablabla/" + QueueName;
        private readonly GenericMessage _message = new GenericMessage {Content = "Hello"};
        private const string QueueName = "queuename";
        private const string MessageId = "12345";

        private static MessageResult _result;

        private readonly MessageResultLogger _messageResultLogger = r =>
        {
            _result = r;
        };

        protected override SqsPublisher CreateSystemUnderTest()
        {
            var sqs = new SqsPublisher(RegionEndpoint.EUWest1, QueueName, _sqs, 0, _serialisationRegister, _messageResultLogger, Substitute.For<ILoggerFactory>());
            sqs.Exists();
            return sqs;
        }

        protected override void Given()
        {
            _sqs.GetQueueUrlAsync(Arg.Any<string>()).Returns(new GetQueueUrlResponse {QueueUrl = Url});
            _sqs.GetQueueAttributesAsync(Arg.Any<GetQueueAttributesRequest>()).Returns(new GetQueueAttributesResponse());
            _sqs.Received().SendMessageAsync(Arg.Is<SendMessageRequest>(x => x.MessageBody.Equals("serialized_contents")));
            _serialisationRegister.Serialise(_message, false).Returns("serialized_contents");
        }

        protected override async Task When()
        {
            await SystemUnderTest.PublishAsync(_message);
        }

        [Fact]
        public void MessageIsPublishedToQueue()
        {
            _result.ResponseMessageId.ShouldBe(MessageId);
            _result.ResponseHttpStatusCode.ShouldBe(HttpStatusCode.OK);
        }

    }
}
