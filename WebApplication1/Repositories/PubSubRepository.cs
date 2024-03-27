using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Core;

namespace WebApplication1.Repositories
{
    public class PubSubRepository
    {

        private string _projectId;
        private string _topicId;
        public PubSubRepository(string projectId, string topicId) {
            _projectId = projectId;
            _topicId = topicId;
        }

        public async Task PublishMessage(string messageText)
        {
            TopicName topicName = TopicName.FromProjectTopic(_projectId, _topicId);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            var pubsubMessage = new PubsubMessage
            {
                // The data is any arbitrary ByteString. Here, we're using text.
                Data = ByteString.CopyFromUtf8(messageText)   
            };
            string message = await publisher.PublishAsync(pubsubMessage);
        }

        public string PullMessagesSync(string subscriptionId, bool acknowledge)
        {
            string blogId = "";

            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_projectId, subscriptionId);
            SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
            int messageCount = 0;
            try
            {
                // Pull messages from server,
                // allowing an immediate response if there are no messages.
                PullResponse response = subscriberClient.Pull(subscriptionName, maxMessages: 1);
                // Print out each received message.
                ReceivedMessage msg = response.ReceivedMessages.FirstOrDefault();

                if (msg != null) 
                {
                    blogId = msg.Message.Data.ToStringUtf8();
                    Interlocked.Increment(ref messageCount);
                }
                // If acknowledgement required, send to server.
                if (acknowledge && messageCount > 0)
                {
                    subscriberClient.Acknowledge(subscriptionName, response.ReceivedMessages.Select(msg => msg.AckId));
                }
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
            {
                // UNAVAILABLE due to too many concurrent pull requests pending for the given subscription.
            }
            return blogId;
        }
    }
}
