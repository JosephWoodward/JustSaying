namespace JustSaying.AwsTools.MessageHandling
{
    /// <summary>
    /// Handler allowing you to access the SQS or SNS message response.
    /// </summary>
    /// <param name="result">The result of the SNS notification or SQS enqueue.</param>
    /// <returns></returns>
    public delegate void MessageResultLogger(MessageResult result);
}
