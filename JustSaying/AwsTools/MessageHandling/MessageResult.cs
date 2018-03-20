using System.Net;

namespace JustSaying.AwsTools.MessageHandling
{
    public class MessageResult
    {
        public string ResponseMessageId { get; }
        public HttpStatusCode? ResponseHttpStatusCode { get; }

        public MessageResult(string responseMessageId, HttpStatusCode? responseHttpStatusCode)
        {
            ResponseMessageId = responseMessageId;
            ResponseHttpStatusCode = responseHttpStatusCode;
        }
    }
}
