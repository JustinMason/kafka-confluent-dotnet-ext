using System.Collections.Generic;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Examples.AvroSpecific;
using Xunit;

namespace KafkaFacade.Tests
{
    public class AvroConsumeResultTestHandler : IHandleAvroConsumeResultEvent
    {
        public void ErrorHandler(IConsumer<string, GenericRecord> consumer, Error error)
        {
            Assert.True(false, error.Reason);
        }

        public void Handle(AvroConsumerClient avroConsumerClient, AvroConsumeResultEvent avroConsumeResultEvent)
        {
            System.Console.WriteLine($"CONSUME P: {avroConsumeResultEvent.ConsumeResult.Partition} OFFSET: {avroConsumeResultEvent.ConsumeResult.Offset} NAME: {avroConsumeResultEvent.ToSpecificAsync<User>().GetAwaiter().GetResult().name}"); 
            avroConsumerClient.Close();
        }

        public void PartitionsRevokedHandleAction(IConsumer<string, GenericRecord> consumer, List<TopicPartitionOffset> topicPartitionOffsets)
        {
            
        }
    }
}