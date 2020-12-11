using System.Collections.Generic;
using Avro.Generic;
using Confluent.Kafka;

namespace KafkaFacade.Avro
{
    public interface IHandleAvroConsumeResultEvent
    {
        void Handle(AvroConsumerClient AvroConsumerClient, AvroConsumeResultEvent avroConsumeResultEvent);

        void ErrorHandler(IConsumer<string, GenericRecord> consumer, Error error);

        void PartitionsRevokedHandleAction(IConsumer<string, GenericRecord> consumer, List<TopicPartitionOffset> topicPartitionOffsets);
    }
}