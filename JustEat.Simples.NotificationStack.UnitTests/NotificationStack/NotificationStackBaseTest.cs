using JustEat.Simples.NotificationStack.Messaging.Monitoring;
using JustEat.Testing;
using JustEat.Simples.NotificationStack.Messaging;
using NSubstitute;

namespace Stack.UnitTests.NotificationStack
{
    public abstract class NotificationStackBaseTest : BehaviourTest<JustEat.Simples.NotificationStack.Stack.NotificationStack>
    {
        protected readonly IMessagingConfig Config = Substitute.For<IMessagingConfig>();
        protected readonly IMessageMonitor Monitor = Substitute.For<IMessageMonitor>();

        protected override JustEat.Simples.NotificationStack.Stack.NotificationStack CreateSystemUnderTest()
        {
            return new JustEat.Simples.NotificationStack.Stack.NotificationStack(Config, null) {Monitor = Monitor};
        }
    }
}