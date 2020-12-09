using System.Collections.Generic;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Examples.AvroSpecific;
using Xunit;

namespace KafkaFacade.Tests
{
    /// <summary>
    /// Mock Test Handler to Validate Consumer Interactions
    /// </summary>
    public class AvroConsumeResultTestHandler : IHandleAvroConsumeResultEvent
    {
        int _handled = 0;
        public AvroConsumeResultTestHandler(int handleCount = 1)
        {
            HandleCount = handleCount;
        }

        public int HandleCount { get; }

        public void ErrorHandler(IConsumer<string, GenericRecord> consumer, Error error)
        {
            Assert.True(false, error.Reason);
        }

        public void Handle(AvroConsumerClient avroConsumerClient, AvroConsumeResultEvent avroConsumeResultEvent)
        {
            if(avroConsumeResultEvent.ConsumeResult.Message.Value.Schema.Fullname =="Confluent.Kafka.Examples.AvroSpecific.User" )
                System.Console.WriteLine($"CONSUME P: {avroConsumeResultEvent.ConsumeResult.Partition} OFFSET: {avroConsumeResultEvent.ConsumeResult.Offset} NAME: {avroConsumeResultEvent.ToSpecificAsync<User>().GetAwaiter().GetResult().name} Standard RATE: {avroConsumeResultEvent.ToSpecificAsync<User>().GetAwaiter().GetResult().hourly_rate }"); 
            else if(avroConsumeResultEvent.ConsumeResult.Message.Value.Schema.Fullname =="Confluent.Kafka.Examples.AvroSpecific.HourBilled" )
                System.Console.WriteLine($"CONSUME P: {avroConsumeResultEvent.ConsumeResult.Partition} OFFSET: {avroConsumeResultEvent.ConsumeResult.Offset} NAME: {avroConsumeResultEvent.ToSpecificAsync<HourBilled>().GetAwaiter().GetResult().name} RATE: {avroConsumeResultEvent.ToSpecificAsync<HourBilled>().GetAwaiter().GetResult().rate_billed }"); 
    
            _handled++;

            if(_handled == HandleCount)
                avroConsumerClient.Close();
        }

        public void PartitionsRevokedHandleAction(IConsumer<string, GenericRecord> consumer, List<TopicPartitionOffset> topicPartitionOffsets)
        {
            
        }
    }
}