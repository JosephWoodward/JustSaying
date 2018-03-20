using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using JustSaying.Messaging;
using JustSaying.Messaging.MessageSerialisation;
using Microsoft.Extensions.Logging;
using Message = JustSaying.Models.Message;

namespace JustSaying.AwsTools.MessageHandling
{
    public class SqsPublisher : SqsQueueByName, IMessagePublisher
    {
        private readonly IAmazonSQS _client;
        private readonly IMessageSerialisationRegister _serialisationRegister;
        private readonly MessageResultLogger _messageResultLogger;

        public SqsPublisher(RegionEndpoint region, string queueName, IAmazonSQS client,
            int retryCountBeforeSendingToErrorQueue, IMessageSerialisationRegister serialisationRegister,
            MessageResultLogger messageResultLogger, ILoggerFactory loggerFactory)
            : base(region, queueName, client, retryCountBeforeSendingToErrorQueue, loggerFactory)
        {
            _client = client;
            _serialisationRegister = serialisationRegister;
            _messageResultLogger = messageResultLogger;
        }

#if AWS_SDK_HAS_SYNC
        public void Publish(Message message)
        {
            var request = BuildSendMessageRequest(message);
            SendMessageResponse response = null;

            try
            {
                _client.SendMessage(request);
            }
            catch (Exception ex)
            {
                throw new PublishException(
                    $"Failed to publish message to SQS. QueueUrl: {request.QueueUrl} MessageBody: {request.MessageBody}",
                    ex);
            }
            finally
            {
                _messageResultLogger?.Invoke(new MessageResult(response?.MessageId, response?.HttpStatusCode));
            }
        }
#endif

        public async Task PublishAsync(Message message)
        {
            var request = BuildSendMessageRequest(message);
            SendMessageResponse response = null;
            try
            {
                response = await _client.SendMessageAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PublishException(
                    $"Failed to publish message to SQS. QueueUrl: {request.QueueUrl} MessageBody: {request.MessageBody}",
                    ex);
            }
            finally
            {
                _messageResultLogger?.Invoke(new MessageResult(response?.MessageId, response?.HttpStatusCode));
            }
        }

        private SendMessageRequest BuildSendMessageRequest(Message message)
        {
            var request = new SendMessageRequest
            {
                MessageBody = GetMessageInContext(message),
                QueueUrl = Url
            };

            if (message.DelaySeconds.HasValue)
            {
                request.DelaySeconds = message.DelaySeconds.Value;
            }
            return request;
        }

        public string GetMessageInContext(Message message) => _serialisationRegister.Serialise(message, serializeForSnsPublishing: false);
    }
}
