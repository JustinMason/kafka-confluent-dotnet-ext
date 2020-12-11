using System.Collections.Generic;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Examples.Protobuf;
using Xunit;
using KafkaFacade.Protobuf;
using Google.Protobuf;

namespace KafkaFacade.Tests
{
    /// <summary>
    /// Mock Test Handler to Validate Consumer Interactions
    /// </summary>
    public class ProtobufConsumeResultTestHandler<T> : IHandleProtobufConsumeResultEvent<User>
    {
        int _handled = 0;
        List<long> _offsets = new List<long>();
        public ProtobufConsumeResultTestHandler(List<long> offsets, int handleCount = 1)
        {
            HandleCount = handleCount;
            _offsets = offsets;
        }

        public ProtobufConsumeResultTestHandler(int handleCount = 1)
        {
            HandleCount = handleCount;
        }

        public int HandleCount { get; }

        public void ErrorHandler(IConsumer<string, User> consumer, Error error)
        {
            Assert.True(false, error.Reason);
        }

        public void Handle(ProtobufConsumerClient<User> protobufConsumerClient, ProtobufConsumeResultEvent<User> protobufConsumeResultEvent)
        {
            System.Console.WriteLine($"CONSUME P: {protobufConsumeResultEvent.ConsumeResult.Partition} OFFSET: {protobufConsumeResultEvent.ConsumeResult.Offset} NAME: {protobufConsumeResultEvent.ConsumeResult.Message.Value.Name} "); 
    
            _handled++;

            if(_handled == HandleCount)
                protobufConsumerClient.Close();
        }

        public void PartitionsRevokedHandleAction(IConsumer<string, User> consumer, List<TopicPartitionOffset> topicPartitionOffsets)
        {
            
        }
    }
}