using System.Collections.Generic;
using Avro.Generic;
using Confluent.Kafka;

namespace KafkaFacade.Avro
{
    public abstract class AvroConsumeResultEventHandler : IHandleAvroConsumeResultEvent
    {
        private readonly Dictionary<int, TopicPartitionOffset> _currentOffsets = new Dictionary<int, TopicPartitionOffset>();

        public abstract void HandleEvent(AvroConsumerClient avroConsumerClient, AvroConsumeResultEvent avroConsumeResultEvent);

        public virtual void Handle(AvroConsumerClient avroConsumerClient, AvroConsumeResultEvent avroConsumeResultEvent)
        {
            HandleEvent(avroConsumerClient, avroConsumeResultEvent);

            var consumerResult = avroConsumeResultEvent.ConsumeResult;

            _currentOffsets[consumerResult.Partition.Value]
                = new TopicPartitionOffset (
                    consumerResult.Topic,
                    consumerResult.Partition,
                    consumerResult.Offset);
        }

        public virtual void PartitionsRevokedHandleAction(IConsumer<string, GenericRecord> consumer, List<TopicPartitionOffset> topicPartitionOffsets)
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
        public virtual void ErrorHandler(IConsumer<string, GenericRecord> consumer, Error error)
        {
            System.Diagnostics.Debug.Fail($"Error Handler: {error}");
        }
    }
}