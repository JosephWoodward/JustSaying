using System.Net;

namespace JustSaying.AwsTools.MessageHandling
{
    public class MessageResponse
    {
        public string ResponseMessageId { get; }
        public HttpStatusCode? ResponseHttpStatusCode { get; }

        public MessageResponse(string responseMessageId, HttpStatusCode? responseHttpStatusCode)
        {
            ResponseMessageId = responseMessageId;
            ResponseHttpStatusCode = responseHttpStatusCode;
        }
    }
}
