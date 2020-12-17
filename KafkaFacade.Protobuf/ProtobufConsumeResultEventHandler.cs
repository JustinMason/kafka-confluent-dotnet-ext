using System.Collections.Generic;
using Confluent.Kafka;
using Google.Protobuf;

namespace KafkaFacade.Protobuf
{
    public abstract class ProtobufConsumeResultEventHandler<T> : IHandleProtobufConsumeResultEvent<T>
        where T : class, IMessage<T>, new()
    {
        private readonly Dictionary<int, TopicPartitionOffset> _currentOffsets = new Dictionary<int, TopicPartitionOffset>();
        
        public abstract void HandleEvent(ProtobufConsumerClient<T> ProtobufConsumerClient, ProtobufConsumeResultEvent<T> ProtobufConsumeResultEvent);

        public virtual void Handle(ProtobufConsumerClient<T> ProtobufConsumerClient, ProtobufConsumeResultEvent<T> ProtobufConsumeResultEvent)
        {
            HandleEvent(ProtobufConsumerClient, ProtobufConsumeResultEvent);

            var consumerResult = ProtobufConsumeResultEvent.ConsumeResult;

            _currentOffsets[consumerResult.Partition.Value]
                = new TopicPartitionOffset (
                    consumerResult.Topic,
                    consumerResult.Partition,
                    consumerResult.Offset);
        }

        public virtual void PartitionsRevokedHandleAction(IConsumer<string, T> consumer, List<TopicPartitionOffset> topicPartitionOffsets)
        {
            try
            {
                consumer.Commit(_currentOffsets.Values);
            }
            catch (System.Exception err)
            {
                System.Diagnostics.Debug.Fail(err.ToString());
            }
        }
        public virtual void ErrorHandler(IConsumer<string, T> consumer, Error error)
        {
            System.Diagnostics.Debug.Fail($"Error Handler: {error}");
        }

    }
}